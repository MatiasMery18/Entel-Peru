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
                var sql = $"SELECT * FROM dbo.SabanaIngreso WHERE [{col}] BETWEEN @d1 AND @d2 ORDER BY [{col}]";
                using (var cmd = new SqlCommand(sql, con))
                {
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

        void btnExportar_Click(object sender, EventArgs e)
        {
            if (currentData == null || currentData.Rows.Count == 0)
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
                    ExportDataTableToCsv(currentData, sfd.FileName);
                    MessageBox.Show("Exportado", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        static void ExportDataTableToCsv(DataTable table, string path)
        {
            var sb = new StringBuilder();
            var cols = table.Columns.Cast<DataColumn>().Select(c => EscapeCsv(c.ColumnName));
            sb.AppendLine(string.Join(",", cols));
            foreach (DataRow r in table.Rows)
            {
                var cells = table.Columns.Cast<DataColumn>().Select(c => EscapeCsv(Convert.ToString(r[c])));
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
    }
}