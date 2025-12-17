using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
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

        void btnBuscar_Click(object sender, EventArgs e)
        {
            if (dtDesde.Value > dtHasta.Value)
            {
                MessageBox.Show("La fecha 'Desde' no puede ser mayor a la fecha 'Hasta'.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var desde = dtDesde.Value.Date;
            var hasta = dtHasta.Value.Date.AddDays(1).AddTicks(-1);
            
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var cmd = new SqlCommand(storedProcName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@d1", SqlDbType.Date).Value = desde;
                    cmd.Parameters.Add("@d2", SqlDbType.Date).Value = hasta;
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        currentData = new DataTable();
                        da.Fill(currentData);
                        
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
                            
                        if (dgvSabana.Columns.Contains("costo_EBS"))
                        {
                            dgvSabana.Columns["costo_EBS"].Visible = true;
                            dgvSabana.Columns["costo_EBS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dgvSabana.Columns["costo_EBS"].HeaderText = "Costo EBS";
                        }

                        lblRows.Text = "Filas: " + currentData.Rows.Count;
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
                sfd.FileName = "CosteIrregular.csv";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    ExportDataTableToCsv(dgvSabana, sfd.FileName);
                    MessageBox.Show("Exportado", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        var create = @"CREATE PROCEDURE dbo.usp_SabanaIngreso_CosteIrregular_V3
@d1 DATE,
@d2 DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [id_sabana_ingreso], [fecha_cierre], [costo_EBS]
    FROM dbo.SabanaIngreso 
    WHERE TRY_CONVERT(DATE, fecha_cierre, 23) BETWEEN @d1 AND @d2
    ORDER BY TRY_CONVERT(DATE, fecha_cierre, 23);
END";
                        using (var cmdCreate = new SqlCommand(create, con))
                        {
                            cmdCreate.ExecuteNonQuery();
                        }
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

        static bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
        }
    }
}
