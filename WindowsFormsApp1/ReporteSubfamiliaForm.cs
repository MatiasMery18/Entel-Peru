using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ReporteSubfamiliaForm : Form
    {
        readonly string connStr;
        DataTable currentData;
        string storedProcName = "dbo.usp_Reporte_Subfamilia_V1";

        public ReporteSubfamiliaForm(string connectionString)
        {
            InitializeComponent();
            connStr = connectionString;
            dtDesde.Value = DateTime.Today.AddDays(-7);
            dtHasta.Value = DateTime.Today;

            if (!IsDesignMode())
            {
                EnsureStoredProcedure();
            }
        }

        static bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
        }

        void EnsureStoredProcedure()
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();

                // Drop if exists to update definition
                using (var cmdDrop = new SqlCommand($"IF OBJECT_ID('{storedProcName}', 'P') IS NOT NULL DROP PROCEDURE {storedProcName}", con))
                {
                    cmdDrop.ExecuteNonQuery();
                }

                // Create new SP
                // Identifies subfamilies using SabanaIngreso and MaestraTipoArticulo
                // Joins on SKU
                // Filters by Date Range (fecha_cierre)
                var create = $@"CREATE PROCEDURE {storedProcName}
@d1 DATETIME2,
@d2 DATETIME2,
@top INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @top IS NOT NULL
    BEGIN
        SELECT TOP (@top)
            S.*, 
            M.tipo_articulo AS subfamilia
        FROM dbo.SabanaIngreso S
        LEFT JOIN dbo.MaestraTipoArticulo M ON LTRIM(RTRIM(S.sku)) = LTRIM(RTRIM(M.sku))
        WHERE TRY_CAST(S.fecha_cierre AS DATE) BETWEEN @d1 AND @d2
        ORDER BY TRY_CAST(S.fecha_cierre AS DATE);
    END
    ELSE
    BEGIN
        SELECT 
            S.*, 
            M.tipo_articulo AS subfamilia
        FROM dbo.SabanaIngreso S
        LEFT JOIN dbo.MaestraTipoArticulo M ON LTRIM(RTRIM(S.sku)) = LTRIM(RTRIM(M.sku))
        WHERE TRY_CAST(S.fecha_cierre AS DATE) BETWEEN @d1 AND @d2
        ORDER BY TRY_CAST(S.fecha_cierre AS DATE);
    END
END";
                try 
                {
                    using (var cmdCreate = new SqlCommand(create, con))
                    {
                        cmdCreate.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al crear el procedimiento almacenado (Verifique que la tabla 'MaestraTipoArticulo' existe y tiene las columnas 'sku' y 'tipo_articulo'): " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        async void btnBuscar_Click(object sender, EventArgs e)
        {
             if (dtDesde.Value > dtHasta.Value)
            {
                MessageBox.Show("La fecha 'Desde' no puede ser mayor a la fecha 'Hasta'.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var desde = dtDesde.Value.Date;
            var hasta = dtHasta.Value.Date;

            btnBuscar.Enabled = false;
            btnBuscar.Text = "Buscando...";
            Cursor = Cursors.WaitCursor;

            // Clear previous data
            currentData = null;
            dgvReporte.DataSource = null;
            GC.Collect();

            try
            {
                DataTable dt = await Task.Run(() =>
                {
                    using (var con = new SqlConnection(connStr))
                    {
                        con.Open();
                        using (var cmd = new SqlCommand(storedProcName, con))
                        {
                            cmd.CommandTimeout = 300; 
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@d1", desde);
                            cmd.Parameters.AddWithValue("@d2", hasta);
                            cmd.Parameters.AddWithValue("@top", 1000); // Preview Limit (Memory Optimization)
                            
                            using (var da = new SqlDataAdapter(cmd))
                            {
                                var table = new DataTable();
                                da.Fill(table);
                                return table;
                            }
                        }
                    }
                });

                currentData = dt;
                dgvReporte.DataSource = currentData;
                
                // Highlight Subfamilia column if exists
                if (dgvReporte.Columns.Contains("subfamilia"))
                {
                    var col = dgvReporte.Columns["subfamilia"];
                    col.HeaderText = "Subfamilia";
                    col.DefaultCellStyle.BackColor = System.Drawing.Color.LightCyan; 
                    col.DefaultCellStyle.Font = new System.Drawing.Font(dgvReporte.Font, System.Drawing.FontStyle.Bold);
                    col.DisplayIndex = 0; // Move to front for visibility
                }

                lblRows.Text = "Filas: " + currentData.Rows.Count + (currentData.Rows.Count >= 1000 ? " (Vista Previa - Limitado a 1000)" : "");
                
                if (currentData.Rows.Count >= 1000)
                {
                    MessageBox.Show("Se muestran los primeros 1000 registros para una visualización rápida.\nUse 'Exportar' para obtener todos los datos.", "Vista Previa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBuscar.Enabled = true;
                btnBuscar.Text = "Buscar";
                Cursor = Cursors.Default;
            }
        }

        async void btnExportar_Click(object sender, EventArgs e)
        {
            if (dtDesde.Value > dtHasta.Value)
            {
                 MessageBox.Show("Rango de fechas inválido.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "ReporteSubfamilia_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    var path = sfd.FileName;
                    var desde = dtDesde.Value.Date;
                    var hasta = dtHasta.Value.Date;

                    btnExportar.Enabled = false;
                    btnExportar.Text = "Exportando...";
                    Cursor = Cursors.WaitCursor;

                    try
                    {
                        await Task.Run(() =>
                        {
                            using (var con = new SqlConnection(connStr))
                            {
                                con.Open();
                                using (var cmd = new SqlCommand(storedProcName, con))
                                {
                                    cmd.CommandTimeout = 600; 
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@d1", desde);
                                    cmd.Parameters.AddWithValue("@d2", hasta);
                                    cmd.Parameters.AddWithValue("@top", DBNull.Value); // No limit for export

                                    using (var reader = cmd.ExecuteReader())
                                    {
                                        using (var sw = new StreamWriter(path, false, Encoding.UTF8))
                                        {
                                            // Write Header
                                            for (int i = 0; i < reader.FieldCount; i++)
                                            {
                                                sw.Write(EscapeCsv(reader.GetName(i)));
                                                if (i < reader.FieldCount - 1) sw.Write(",");
                                            }
                                            sw.WriteLine();

                                            // Write Rows
                                            while (reader.Read())
                                            {
                                                for (int i = 0; i < reader.FieldCount; i++)
                                                {
                                                    if (!reader.IsDBNull(i))
                                                    {
                                                        sw.Write(EscapeCsv(reader[i].ToString()));
                                                    }
                                                    if (i < reader.FieldCount - 1) sw.Write(",");
                                                }
                                                sw.WriteLine();
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        MessageBox.Show("Exportación completada exitosamente.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnExportar.Enabled = true;
                        btnExportar.Text = "Exportar a Excel (CSV)";
                        Cursor = Cursors.Default;
                        GC.Collect();
                    }
                }
            }
        }

        static string EscapeCsv(string input)
        {
            if (input == null) return "";
            var s = input.Replace("\"", "\"\"");
            return "\"" + s + "\"";
        }
    }
}
