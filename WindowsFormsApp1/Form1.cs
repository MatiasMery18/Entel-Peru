using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string connMaster;
        string connStr;

        public Form1()
        {
            InitializeComponent();
            connMaster = ConfigurationManager.ConnectionStrings["DbMaster"]?.ConnectionString ?? @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
            connStr = ConfigurationManager.ConnectionStrings["Db"]?.ConnectionString ?? @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Entel Peru;Integrated Security=True";
            EnsureDatabaseAndTables();
        }

        void EnsureDatabaseAndTables()
        {
            using (var con = new SqlConnection(connMaster))
            {
                con.Open();
                using (var cmd = new SqlCommand("IF DB_ID('Entel Peru') IS NULL CREATE DATABASE [Entel Peru];", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                var sql = @"
IF OBJECT_ID('dbo.Perfil','U') IS NULL
BEGIN
    CREATE TABLE dbo.Perfil(
        id_perfil INT IDENTITY(1,1) PRIMARY KEY,
        nombre_perfil NVARCHAR(50) UNIQUE NOT NULL
    );
END
IF OBJECT_ID('dbo.Usuario','U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuario(
        id_usuario INT IDENTITY(1,1) PRIMARY KEY,
        nombres NVARCHAR(100) NULL,
        apellido_paterno NVARCHAR(50) NULL,
        apellido_materno NVARCHAR(50) NULL,
        nombre_usuario NVARCHAR(25) UNIQUE NOT NULL,
        password NVARCHAR(255) NOT NULL,
        email NVARCHAR(70) NULL,
        fecha_creacion DATETIME2 NOT NULL CONSTRAINT DF_Usuario_fecha_creacion DEFAULT SYSUTCDATETIME(),
        fecha_modificacion DATETIME2 NULL
    );
END
IF OBJECT_ID('dbo.Perfilado','U') IS NULL
BEGIN
    CREATE TABLE dbo.Perfilado(
        id_perfilado INT IDENTITY(1,1) PRIMARY KEY,
        id_perfil INT NOT NULL,
        id_usuario INT NOT NULL,
        CONSTRAINT FK_Perfilado_Perfil FOREIGN KEY (id_perfil) REFERENCES dbo.Perfil(id_perfil),
        CONSTRAINT FK_Perfilado_Usuario FOREIGN KEY (id_usuario) REFERENCES dbo.Usuario(id_usuario)
    );
END
IF NOT EXISTS(SELECT 1 FROM dbo.Perfil WHERE nombre_perfil='Admin')
BEGIN
    INSERT INTO dbo.Perfil(nombre_perfil) VALUES('Admin');
END
IF NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE nombre_usuario='admin')
BEGIN
    INSERT INTO dbo.Usuario(nombre_usuario,password,nombres) VALUES('admin', @AdminHash, 'Administrador');
    DECLARE @uid INT = SCOPE_IDENTITY();
    DECLARE @pid INT = (SELECT id_perfil FROM dbo.Perfil WHERE nombre_perfil='Admin');
    INSERT INTO dbo.Perfilado(id_perfil,id_usuario) VALUES(@pid,@uid);
END
ELSE
BEGIN
    DECLARE @uid2 INT = (SELECT id_usuario FROM dbo.Usuario WHERE nombre_usuario='admin');
    DECLARE @pid2 INT = (SELECT id_perfil FROM dbo.Perfil WHERE nombre_perfil='Admin');
    IF NOT EXISTS(SELECT 1 FROM dbo.Perfilado WHERE id_usuario=@uid2 AND id_perfil=@pid2)
        INSERT INTO dbo.Perfilado(id_perfil,id_usuario) VALUES(@pid2,@uid2);
END";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@AdminHash", Hash("admin"));
                    cmd.ExecuteNonQuery();
                }

                
            }
        }

        void btnLogin_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            using (var con = new SqlConnection(connStr))
            {
                con.Open();
                var sql = @"SELECT TOP 1 p.nombre_perfil
                             FROM dbo.Usuario u
                             LEFT JOIN dbo.Perfilado pf ON pf.id_usuario=u.id_usuario
                             LEFT JOIN dbo.Perfil p ON p.id_perfil=pf.id_perfil
                             WHERE u.nombre_usuario=@u AND u.password=@p";
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@u", txtUser.Text.Trim());
                    cmd.Parameters.AddWithValue("@p", Hash(txtPass.Text));
                    var role = cmd.ExecuteScalar() as string;
                    if (role == null)
                    {
                        lblMessage.Text = "Usuario o contraseña inválidos";
                        return;
                    }
                    using (var menu = new MenuForm(connStr, txtUser.Text.Trim(), role))
                    {
                        Hide();
                        menu.ShowDialog(this);
                        Show();
                        txtPass.Text = "";
                    }
                }
            }
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

        private void txtUser_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
