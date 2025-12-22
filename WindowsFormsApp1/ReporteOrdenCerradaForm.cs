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
    public partial class ReporteOrdenCerradaForm : Form
    {
        readonly string connStr;
        DataTable currentData;
        string storedProcName = "dbo.usp_Reporte_OrdenCerrada_V1";

        public ReporteOrdenCerradaForm(string connectionString)
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

                // Optimization:
                // 1. Added WITH (NOLOCK) to prevent blocking
                // 2. Removed LTRIM(RTRIM) from JOIN to enable index usage (CRITICAL for performance)
                // 3. Removed ORDER BY from TOP query for instant preview
                var create = $@"CREATE PROCEDURE {storedProcName}
@d1 DATETIME2,
@d2 DATETIME2,
@top INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @top IS NOT NULL
    BEGIN
        -- Preview Mode: Fast, no sorting, WITH (NOLOCK), Direct Join
        SELECT TOP (@top)
            S.*, 
            O.fecha_solicitud,
            O.estado AS estado_oc
        FROM dbo.SabanaIngreso S WITH (NOLOCK)
        INNER JOIN dbo.OrdenCerrada O WITH (NOLOCK) ON S.nro_orden_item = O.numero_orden_item
        WHERE TRY_CAST(S.fecha_cierre AS DATE) BETWEEN @d1 AND @d2
        OPTION (HASH JOIN, RECOMPILE);
    END
    ELSE
    BEGIN
        -- Full Mode: WITH (NOLOCK), Direct Join
        SELECT 
            S.*, 
            O.fecha_solicitud,
            O.estado AS estado_oc
        FROM dbo.SabanaIngreso S WITH (NOLOCK)
        INNER JOIN dbo.OrdenCerrada O WITH (NOLOCK) ON S.nro_orden_item = O.numero_orden_item
        WHERE TRY_CAST(S.fecha_cierre AS DATE) BETWEEN @d1 AND @d2
        ORDER BY TRY_CAST(S.fecha_cierre AS DATE)
        OPTION (HASH JOIN, RECOMPILE);
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
                    MessageBox.Show("Error al crear el procedimiento almacenado (Verifique que la tabla 'OrdenCerrada' existe): " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Clear previous data and force GC
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
                            cmd.Parameters.AddWithValue("@top", 1000); // SQL Limit for Preview
                            
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
                
                // Adjust columns for visibility
                dgvReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

                // Highlight new columns
                string[] newCols = { "fecha_solicitud", "estado_oc" };
                foreach (var colName in newCols)
                {
                    if (dgvReporte.Columns.Contains(colName))
                    {
                        var col = dgvReporte.Columns[colName];
                        col.DefaultCellStyle.BackColor = System.Drawing.Color.LightPink;
                        col.DefaultCellStyle.Font = new System.Drawing.Font(dgvReporte.Font, System.Drawing.FontStyle.Bold);
                    }
                }

                lblRows.Text = "Filas: " + currentData.Rows.Count + (currentData.Rows.Count >= 1000 ? " (Vista Previa - Limitado a 1000)" : "");
                
                if (currentData.Rows.Count >= 1000)
                {
                    MessageBox.Show("Se muestran los primeros 1000 registros para una visualización rápida.\nUse 'Exportar' para obtener todos los datos.", "Vista Previa Limitada", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                sfd.FileName = "ReporteOrdenCerrada_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
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
                                    cmd.Parameters.AddWithValue("@top", DBNull.Value); // No limit

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
                        GC.Collect(); // Force GC
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
