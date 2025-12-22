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
    public partial class SabanaIngresoForm : Form
    {
        readonly string connStr;
        DataTable currentData;
        string storedProcName = "dbo.usp_SabanaIngreso_Periodo_V1";

        public SabanaIngresoForm(string connectionString)
        {
            InitializeComponent();
            connStr = connectionString;
            
            if (!IsDesignMode())
            {
                EnsureStoredProcedure();
                LoadPeriods();
            }
        }

        static bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
        }

        void LoadPeriods()
        {
            try
            {
                using (var con = new SqlConnection(connStr))
                {
                    con.Open();
                    // Verify if column exists first to avoid crashing if schema is old
                    if (!ColumnExists("periodo_cierre"))
                    {
                        MessageBox.Show("La columna 'periodo_cierre' no existe en la tabla SabanaIngreso.", "Error de Esquema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    using (var cmd = new SqlCommand("SELECT DISTINCT periodo_cierre FROM dbo.SabanaIngreso WHERE periodo_cierre IS NOT NULL ORDER BY periodo_cierre DESC", con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            cmbPeriodo.Items.Clear();
                            while (reader.Read())
                            {
                                cmbPeriodo.Items.Add(reader[0].ToString());
                            }
                        }
                    }
                }
                
                if (cmbPeriodo.Items.Count > 0) 
                    cmbPeriodo.SelectedIndex = 0;
                else
                    cmbPeriodo.Items.Add("No hay periodos");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar periodos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        void EnsureStoredProcedure()
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();

                // Drop if exists (update definition)
                using (var cmdDrop = new SqlCommand($"IF OBJECT_ID('{storedProcName}', 'P') IS NOT NULL DROP PROCEDURE {storedProcName}", con))
                {
                    cmdDrop.ExecuteNonQuery();
                }

                // Create new SP
                var create = $@"CREATE PROCEDURE {storedProcName}
@periodo VARCHAR(MAX),
@top INT = NULL -- Optional limit
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @top IS NOT NULL
    BEGIN
        SELECT TOP (@top) * 
        FROM dbo.SabanaIngreso 
        WHERE periodo_cierre = @periodo
        -- ORDER BY clause might be slow if not indexed, removing default sort for speed or sorting by something indexed if possible
    END
    ELSE
    BEGIN
        SELECT * 
        FROM dbo.SabanaIngreso 
        WHERE periodo_cierre = @periodo
    END
END";
                using (var cmd = new SqlCommand(create, con)) cmd.ExecuteNonQuery();
            }
        }

        async void btnBuscar_Click(object sender, EventArgs e)
        {
            if (cmbPeriodo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un periodo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var periodo = cmbPeriodo.SelectedItem.ToString();

            btnBuscar.Enabled = false;
            btnBuscar.Text = "Buscando...";
            Cursor = Cursors.WaitCursor;

            // Force GC to clear previous data memory if possible
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
                            cmd.Parameters.AddWithValue("@periodo", periodo);
                            cmd.Parameters.AddWithValue("@top", 1000); // Always limit preview to 1000
                            
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

        async void btnExportar_Click(object sender, EventArgs e)
        {
            if (cmbPeriodo.SelectedItem == null)
            {
                 MessageBox.Show("Seleccione un periodo.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "SabanaIngreso_" + cmbPeriodo.SelectedItem.ToString() + ".csv";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    var path = sfd.FileName;
                    var periodo = cmbPeriodo.SelectedItem.ToString();

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
                                    cmd.Parameters.AddWithValue("@periodo", periodo);
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
                        GC.Collect(); // Suggest garbage collection after large export
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

        static string EscapeCsv(string input)
        {
            if (input == null) return "";
            var s = input.Replace("\"", "\"\"");
            return "\"" + s + "\"";
        }
    }
}
