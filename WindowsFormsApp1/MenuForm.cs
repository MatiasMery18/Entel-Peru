using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MenuForm : Form
    {
        readonly string connStr;
        readonly string username;
        readonly string role;

        public MenuForm(string connectionString, string user, string userRole)
        {
            InitializeComponent();
            connStr = connectionString;
            username = user;
            role = userRole;
            lblWelcome.Text = "Bienvenido: " + username + " (" + role + ")";
            btnAdmin.Enabled = role == "Admin";
        }

        void btnAdmin_Click(object sender, EventArgs e)
        {
            using (var admin = new AdminForm(connStr))
            {
                admin.ShowDialog(this);
            }
        }

        void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        void btnReportes_Click(object sender, EventArgs e)
        {
            using (var rep = new ReportesForm(connStr))
            {
                rep.ShowDialog(this);
            }
        }
    }
}