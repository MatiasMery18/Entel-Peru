using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class ReportesForm : Form
    {
        readonly string connStr;

        public ReportesForm(string connectionString)
        {
            InitializeComponent();
            connStr = connectionString;
        }

        void btnSabana_Click(object sender, EventArgs e)
        {
            using (var f = new SabanaIngresoForm(connStr))
            {
                f.ShowDialog(this);
            }
        }
    }
}