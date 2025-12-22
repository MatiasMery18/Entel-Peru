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
    public partial class CosteEbsIrregularForm : Form
    {
        readonly string connStr;
        DataTable currentData;
        string dateColumn;
        string storedProcName = "dbo.usp_SabanaIngreso_CosteIrregular_V3";

        public CosteEbsIrregularForm(string connectionString)
        {
            InitializeComponent();
            connStr = connectionString;
            dtDesde.Value = DateTime.Today.AddDays(-7);
            dtHasta.Value = DateTime.Today;
            
            cmbOrden.Items.AddRange(new object[] { "Ascendente", "Descendente" });
            cmbOrden.SelectedIndex = 0;

            dateColumn = "fecha_cierre";
            
            if (!IsDesignMode())
            {
                btnBuscar.Enabled = ColumnExists(dateColumn);
                if (!btnBuscar.Enabled)
                {
                    MessageBox.Show("La tabla 'SabanaIngreso' no posee la columna 'fecha_cierre'.", "Reporte Coste Irregular", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                EnsureStoredProcedure();
            }
            else
            {
                btnBuscar.Enabled = true;
            }
        }

        bool ColumnExists(string columnName)
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var cmd = new SqlCommand(@"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='SabanaIngreso' AND COLUMN_NAME=@c", con))
                {
                    cmd.Parameters.AddWithValue("@c", columnName);
                    var o = cmd.ExecuteScalar();
                    return o != null;
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
            var hasta = dtHasta.Value.Date.AddDays(1).AddTicks(-1);
            
            btnBuscar.Enabled = false;
            btnBuscar.Text = "Buscando...";
            Cursor = Cursors.WaitCursor;

            // Clear previous data and force GC
            currentData = null;
            dgvSabana.DataSource = null;
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
                            cmd.Parameters.AddWithValue("@top", 1000); // SQL level limit
                            
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
                
                string sortOrder = "ASC";
                if (cmbOrden.SelectedItem != null && cmbOrden.SelectedItem.ToString() == "Descendente")
                {
                    sortOrder = "DESC";
                }
                currentData.DefaultView.Sort = "fecha_cierre " + sortOrder;

                dgvSabana.DataSource = currentData.DefaultView;
                
                if (dgvSabana.Columns.Contains("id_sabana_ingreso"))
                    dgvSabana.Columns["id_sabana_ingreso"].Visible = false;
                if (dgvSabana.Columns.Contains("fecha_cierre"))
                    dgvSabana.Columns["fecha_cierre"].Visible = false;

                // Ajustar columnas para que todas sean visibles y tengan ancho basado en contenido y cabecera
                dgvSabana.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

                if (dgvSabana.Columns.Contains("costo_EBS"))
                {
                    var col = dgvSabana.Columns["costo_EBS"];
                    col.HeaderText = "Costo EBS";
                    col.MinimumWidth = 100; // Ancho mínimo garantizado
                    col.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow; // Resaltar columna importante
                    col.DefaultCellStyle.Font = new System.Drawing.Font(dgvSabana.Font, System.Drawing.FontStyle.Bold);
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
            if (dgvSabana.DataSource == null && (dtDesde.Value > dtHasta.Value))
            {
                MessageBox.Show("Rango de fechas inválido.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "CosteIrregular_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    var path = sfd.FileName;
                    var desde = dtDesde.Value.Date;
                    var hasta = dtHasta.Value.Date.AddDays(1).AddTicks(-1);

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
                        GC.Collect(); // Force GC
                    }
                }
            }
        }

        static void ExportDataTableToCsv(DataGridView grid, string path)
        {
            var sb = new StringBuilder();
            var visibleCols = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
            var cols = visibleCols.Select(c => EscapeCsv(c.HeaderText));
            sb.AppendLine(string.Join(",", cols));
            foreach (DataGridViewRow r in grid.Rows)
            {
                if (r.IsNewRow) continue;
                var cells = visibleCols.Select(c => EscapeCsv(Convert.ToString(r.Cells[c.Index].Value)));
                sb.AppendLine(string.Join(",", cells));
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        static string EscapeCsv(string input)
        {
            if (input == null) return "";
            var s = input.Replace("\"", "\"\"");
            return "\"" + s + "\"";
        }

        void EnsureStoredProcedure()
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                
                // Drop if exists to ensure definition is up to date
                using (var cmdDrop = new SqlCommand($"IF OBJECT_ID('{storedProcName}', 'P') IS NOT NULL DROP PROCEDURE {storedProcName}", con))
                {
                    cmdDrop.ExecuteNonQuery();
                }

                var create = $@"CREATE PROCEDURE {storedProcName}
@d1 DATETIME2,
@d2 DATETIME2,
@top INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Filter logic:
    -- 1. Date range
    -- 2. Bad Costo_EBS: NULL, Empty, Zero, or Non-Numeric (Letters)
    -- TRY_CAST(costo_EBS AS FLOAT) IS NULL covers Non-Numeric (and real NULLs if passed)
    -- TRY_CAST(costo_EBS AS FLOAT) = 0 covers '0', '0.00', and potentially empty strings depending on config, so we check '' explicitly too.

    IF @top IS NOT NULL
    BEGIN
        SELECT TOP (@top) *
        FROM dbo.SabanaIngreso 
        WHERE TRY_CAST(fecha_cierre AS DATE) BETWEEN @d1 AND @d2
        AND (
               [costo_EBS] IS NULL 
            OR LTRIM(RTRIM([costo_EBS])) = '' 
            OR TRY_CAST([costo_EBS] AS FLOAT) = 0
            OR TRY_CAST([costo_EBS] AS FLOAT) IS NULL
        )
        ORDER BY TRY_CAST(fecha_cierre AS DATE);
    END
    ELSE
    BEGIN
        SELECT *
        FROM dbo.SabanaIngreso 
        WHERE TRY_CAST(fecha_cierre AS DATE) BETWEEN @d1 AND @d2
        AND (
               [costo_EBS] IS NULL 
            OR LTRIM(RTRIM([costo_EBS])) = '' 
            OR TRY_CAST([costo_EBS] AS FLOAT) = 0
            OR TRY_CAST([costo_EBS] AS FLOAT) IS NULL
        )
        ORDER BY TRY_CAST(fecha_cierre AS DATE);
    END
END";
                using (var cmdCreate = new SqlCommand(create, con))
                {
                    cmdCreate.ExecuteNonQuery();
                }
            }
        }

        void btnFiltro_Click(object sender, EventArgs e)
        {
            if (currentData == null) return;
            using (var dlg = new ColumnFilterForm(dgvSabana, currentData))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var rows = dgvSabana.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow);
                    lblRows.Text = "Filas: " + rows;
                }
            }
        }

        static bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
        }
    }
}
