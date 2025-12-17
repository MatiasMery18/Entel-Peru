using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ReporteSabanaEbsKardexForm : Form
    {
        readonly string connStr;
        DataTable currentData;
        string storedProcName = "dbo.usp_Reporte_SabanaEbsKardex_V3";

        public ReporteSabanaEbsKardexForm(string connectionString)
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
                using (var check = new SqlCommand("SELECT OBJECT_ID(@name,'P')", con))
                {
                    check.Parameters.AddWithValue("@name", storedProcName);
                    var exists = check.ExecuteScalar();
                    if (exists == null || exists == DBNull.Value)
                    {
                        var create = @"CREATE PROCEDURE dbo.usp_Reporte_SabanaEbsKardex_V3
@d1 DATE,
@d2 DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        S.*, 
        E.codigo_almacen, 
        K.organizacion AS nombre_almacen
    FROM dbo.SabanaIngreso S
    LEFT JOIN dbo.Ebs E ON LTRIM(RTRIM(S.nro_orden)) = LTRIM(RTRIM(E.nro_folio)) 
                        OR LTRIM(RTRIM(S.nro_orden)) = LTRIM(RTRIM(E.nro_orden))
    LEFT JOIN dbo.Kardex K ON LTRIM(RTRIM(S.sku)) = LTRIM(RTRIM(K.cod_articulo_sku))
    WHERE TRY_CONVERT(DATE, S.fecha_cierre, 23) BETWEEN @d1 AND @d2
    ORDER BY TRY_CONVERT(DATE, S.fecha_cierre, 23);
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
                            MessageBox.Show("Error al crear el procedimiento almacenado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
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
            var hasta = dtHasta.Value.Date;

            try
            {
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
                            dgvReporte.DataSource = currentData;
                            lblRows.Text = "Filas: " + currentData.Rows.Count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void btnExportar_Click(object sender, EventArgs e)
        {
            if (dgvReporte.DataSource == null || dgvReporte.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "ReporteSabanaEbsKardex.csv";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    ExportDataTableToCsv(dgvReporte, sfd.FileName);
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
    }
}
