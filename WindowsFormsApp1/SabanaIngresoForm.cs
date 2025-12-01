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
            btnBuscar.Enabled = ColumnExists(dateColumn);
            if (!btnBuscar.Enabled)
            {
                MessageBox.Show("La tabla 'SabanaIngreso' no posee la columna 'fecha_cierre'.", "Sabana Ingreso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            EnsureStoredProcedure();
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

        void btnBuscar_Click(object sender, EventArgs e)
        {
            var col = dateColumn;
            var desde = dtDesde.Value.Date;
            var hasta = dtHasta.Value.Date.AddDays(1).AddTicks(-1);
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var cmd = new SqlCommand(storedProcName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@d1", desde);
                    cmd.Parameters.AddWithValue("@d2", hasta);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        currentData = new DataTable();
                        da.Fill(currentData);
                        dgvSabana.DataSource = currentData;
                        lblRows.Text = "Filas: " + currentData.Rows.Count;
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
                using (var check = new SqlCommand("SELECT OBJECT_ID(@name,'P')", con))
                {
                    check.Parameters.AddWithValue("@name", storedProcName);
                    var exists = check.ExecuteScalar();
                    if (exists == null || exists == DBNull.Value)
                    {
                        var create = @"CREATE PROCEDURE dbo.usp_SabanaIngreso_RangoFecha
@d1 DATETIME2,
@d2 DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.SabanaIngreso WHERE [fecha_cierre] BETWEEN @d1 AND @d2 ORDER BY [fecha_cierre];
END";
                        using (var cmd = new SqlCommand(create, con)) cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        void btnExportar_Click(object sender, EventArgs e)
        {
            if (dgvSabana.DataSource == null || dgvSabana.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "SabanaIngreso.csv";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    ExportDataTableToCsv(dgvSabana, sfd.FileName);
                    MessageBox.Show("Exportado", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}
