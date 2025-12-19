using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class SabanaIngresoForm : Form
    {
        readonly string connStr;
        DataTable currentData;
        string dateColumn;
        string storedProcName = "dbo.usp_SabanaIngreso_RangoFecha";

        public SabanaIngresoForm(string connectionString)
        {
            InitializeComponent();
            connStr = connectionString;
            dtDesde.Value = DateTime.Today.AddDays(-7);
            dtHasta.Value = DateTime.Today;
            dateColumn = "fecha_cierre";
            if (!IsDesignMode())
            {
                btnBuscar.Enabled = ColumnExists(dateColumn);
                if (!btnBuscar.Enabled)
                {
                    MessageBox.Show("La tabla 'SabanaIngreso' no posee la columna 'fecha_cierre'.", "Sabana Ingreso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                EnsureStoredProcedure();
            }
            else
            {
                btnBuscar.Enabled = true;
            }
        }

        static bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
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
            var col = dateColumn;
            var desde = dtDesde.Value.Date;
            var hasta = dtHasta.Value.Date.AddDays(1).AddTicks(-1);

            btnBuscar.Enabled = false;
            btnBuscar.Text = "Buscando...";
            Cursor = Cursors.WaitCursor;

            try
            {
                DataTable dt = await System.Threading.Tasks.Task.Run(() =>
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
                            cmd.Parameters.AddWithValue("@top", 1000); // Always limit preview to 1000 at SQL level
                            
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
                dgvSabana.DataSource = currentData;
                lblRows.Text = "Filas: " + currentData.Rows.Count + (currentData.Rows.Count >= 1000 ? " (Vista Previa - Limitado a 1000)" : "");
                
                if (currentData.Rows.Count >= 1000)
                {
                    MessageBox.Show("Se muestran los primeros 1000 registros para una visualización rápida.\nUse 'Exportar' para obtener todos los datos.", "Vista Previa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBuscar.Enabled = true;
                btnBuscar.Text = "Buscar";
                Cursor = Cursors.Default;
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

                // Drop if exists
                using (var cmdDrop = new SqlCommand("IF OBJECT_ID('dbo.usp_SabanaIngreso_RangoFecha', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_SabanaIngreso_RangoFecha", con))
                {
                    cmdDrop.ExecuteNonQuery();
                }

                // Recreate with logic to handle potential varchar dates AND limit parameter
                var create = @"CREATE PROCEDURE dbo.usp_SabanaIngreso_RangoFecha
@d1 DATETIME2,
@d2 DATETIME2,
@top INT = NULL -- Optional limit
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @top IS NOT NULL
    BEGIN
        SELECT TOP (@top) * 
        FROM dbo.SabanaIngreso 
        WHERE TRY_CAST([fecha_solicitud ] AS DATE) BETWEEN @d1 AND @d2 
        ORDER BY TRY_CAST([fecha_solicitud ] AS DATE);
    END
    ELSE
    BEGIN
        SELECT * 
        FROM dbo.SabanaIngreso 
        WHERE TRY_CAST([fecha_solicitud ] AS DATE) BETWEEN @d1 AND @d2 
        ORDER BY TRY_CAST([fecha_solicitud ] AS DATE);
    END
END";
                using (var cmd = new SqlCommand(create, con)) cmd.ExecuteNonQuery();
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
                sfd.FileName = "SabanaIngreso.csv";
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
                        await System.Threading.Tasks.Task.Run(() =>
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
                    }
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

        private void dtDesde_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lblDesde_Click(object sender, EventArgs e)
        {

        }
    }
}
