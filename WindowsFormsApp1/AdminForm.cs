using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class AdminForm : Form
    {
        readonly string connStr;

        public AdminForm(string connectionString)
        {
            InitializeComponent();
            connStr = connectionString;
            this.Load += (s, e) => LoadUsers();
        }

        void LoadUsers()
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var da = new SqlDataAdapter(@"SELECT u.id_usuario, u.nombre_usuario, u.nombres, u.apellido_paterno, u.apellido_materno, u.email,
                                                            u.fecha_creacion, u.fecha_modificacion,
                                                            ISNULL(p.nombre_perfil,'') AS Rol
                                                     FROM dbo.Usuario u
                                                     LEFT JOIN dbo.Perfilado pf ON pf.id_usuario=u.id_usuario
                                                     LEFT JOIN dbo.Perfil p ON p.id_perfil=pf.id_perfil
                                                     ORDER BY u.nombre_usuario", con))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgvUsers.DataSource = dt;
                }
            }
        }

        void btnCreate_Click(object sender, EventArgs e)
        {
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var tx = con.BeginTransaction())
                {
                    var rolNombre = cmbAdminRole.SelectedItem?.ToString() ?? "User";
                    int perfilId;
                    using (var cmdPerfil = new SqlCommand("SELECT id_perfil FROM dbo.Perfil WHERE nombre_perfil=@r", con, tx))
                    {
                        cmdPerfil.Parameters.AddWithValue("@r", rolNombre);
                        var o = cmdPerfil.ExecuteScalar();
                        if (o == null)
                        {
                            using (var ins = new SqlCommand("INSERT INTO dbo.Perfil(nombre_perfil) VALUES(@r); SELECT SCOPE_IDENTITY();", con, tx))
                            {
                                ins.Parameters.AddWithValue("@r", rolNombre);
                                perfilId = Convert.ToInt32(ins.ExecuteScalar());
                            }
                        }
                        else perfilId = Convert.ToInt32(o);
                    }

                    using (var exists = new SqlCommand("SELECT COUNT(1) FROM dbo.Usuario WHERE nombre_usuario=@u", con, tx))
                    {
                        exists.Parameters.AddWithValue("@u", txtAdminUser.Text.Trim());
                        var cnt = (int)exists.ExecuteScalar();
                        if (cnt > 0)
                        {
                            MessageBox.Show("El nombre de usuario ya existe", "Crear usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            tx.Rollback();
                            return;
                        }
                    }

                    int usuarioId;
                    using (var cmd = new SqlCommand("INSERT INTO dbo.Usuario(nombre_usuario,password,nombres,apellido_paterno,apellido_materno,email) OUTPUT INSERTED.id_usuario VALUES(@u,@p,@n,@ap,@am,@e)", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@u", txtAdminUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@p", Hash(txtAdminPass.Text));
                        cmd.Parameters.AddWithValue("@n", txtAdminName.Text.Trim());
                        cmd.Parameters.AddWithValue("@ap", txtApellidoPat.Text.Trim());
                        cmd.Parameters.AddWithValue("@am", txtApellidoMat.Text.Trim());
                        cmd.Parameters.AddWithValue("@e", txtAdminEmail.Text.Trim());
                        usuarioId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    using (var link = new SqlCommand("INSERT INTO dbo.Perfilado(id_perfil,id_usuario) VALUES(@pid,@uid)", con, tx))
                    {
                        link.Parameters.AddWithValue("@pid", perfilId);
                        link.Parameters.AddWithValue("@uid", usuarioId);
                        link.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
            }
            LoadUsers();
            MessageBox.Show("Usuario creado", "Crear usuario", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;
            var id = (int)dgvUsers.CurrentRow.Cells["id_usuario"].Value;
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("UPDATE dbo.Usuario SET nombre_usuario=@u, password=@p, nombres=@n, apellido_paterno=@ap, apellido_materno=@am, email=@e, fecha_modificacion=SYSUTCDATETIME() WHERE id_usuario=@id", con))
                {
                    cmd.Parameters.AddWithValue("@u", txtAdminUser.Text.Trim());
                    cmd.Parameters.AddWithValue("@p", Hash(txtAdminPass.Text));
                    cmd.Parameters.AddWithValue("@n", txtAdminName.Text.Trim());
                    cmd.Parameters.AddWithValue("@ap", txtApellidoPat.Text.Trim());
                    cmd.Parameters.AddWithValue("@am", txtApellidoMat.Text.Trim());
                    cmd.Parameters.AddWithValue("@e", txtAdminEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                int perfilId;
                var rolNombre = cmbAdminRole.SelectedItem?.ToString() ?? "User";
                using (var cmdPerfil = new SqlCommand("SELECT id_perfil FROM dbo.Perfil WHERE nombre_perfil=@r", con))
                {
                    cmdPerfil.Parameters.AddWithValue("@r", rolNombre);
                    var o = cmdPerfil.ExecuteScalar();
                    if (o == null)
                    {
                        using (var ins = new SqlCommand("INSERT INTO dbo.Perfil(nombre_perfil) VALUES(@r); SELECT SCOPE_IDENTITY();", con))
                        {
                            ins.Parameters.AddWithValue("@r", rolNombre);
                            perfilId = Convert.ToInt32(ins.ExecuteScalar());
                        }
                    }
                    else perfilId = Convert.ToInt32(o);
                }
                using (var del = new SqlCommand("DELETE FROM dbo.Perfilado WHERE id_usuario=@id", con))
                {
                    del.Parameters.AddWithValue("@id", id);
                    del.ExecuteNonQuery();
                }
                using (var link = new SqlCommand("INSERT INTO dbo.Perfilado(id_perfil,id_usuario) VALUES(@pid,@uid)", con))
                {
                    link.Parameters.AddWithValue("@pid", perfilId);
                    link.Parameters.AddWithValue("@uid", id);
                    link.ExecuteNonQuery();
                }
            }
            LoadUsers();
        }

        void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;
            var row = dgvUsers.CurrentRow;
            txtAdminUser.Text = Convert.ToString(row.Cells["nombre_usuario"].Value);
            txtAdminName.Text = Convert.ToString(row.Cells["nombres"].Value);
            txtApellidoPat.Text = Convert.ToString(row.Cells["apellido_paterno"].Value);
            txtApellidoMat.Text = Convert.ToString(row.Cells["apellido_materno"].Value);
            txtAdminEmail.Text = Convert.ToString(row.Cells["email"].Value);
            var rol = Convert.ToString(row.Cells["Rol"].Value);
            if (!string.IsNullOrEmpty(rol)) cmbAdminRole.SelectedItem = rol;
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;
            var id = (int)dgvUsers.CurrentRow.Cells["id_usuario"].Value;
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                using (var cmd1 = new SqlCommand("DELETE FROM dbo.Perfilado WHERE id_usuario=@id", con))
                {
                    cmd1.Parameters.AddWithValue("@id", id);
                    cmd1.ExecuteNonQuery();
                }
                using (var cmd2 = new SqlCommand("DELETE FROM dbo.Usuario WHERE id_usuario=@id", con))
                {
                    cmd2.Parameters.AddWithValue("@id", id);
                    cmd2.ExecuteNonQuery();
                }
            }
            LoadUsers();
        }

        void btnReload_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        static string Hash(string input)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? ""));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            dgvUsers.SelectionChanged += dgvUsers_SelectionChanged;
        }
    }
}