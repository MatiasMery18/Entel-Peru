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

        void btnCosteIrregular_Click(object sender, EventArgs e)
        {
            using (var f = new CosteEbsIrregularForm(connStr))
            {
                f.ShowDialog(this);
            }
        }

        void btnSabanaEbsKardex_Click(object sender, EventArgs e)
        {
            using (var f = new ReporteSabanaEbsKardexForm(connStr))
            {
                f.ShowDialog(this);
            }
        }

        void btnSubfamilia_Click(object sender, EventArgs e)
        {
            using (var f = new ReporteSubfamiliaForm(connStr))
            {
                f.ShowDialog(this);
            }
        }

        void btnOrdenAbierta_Click(object sender, EventArgs e)
        {
            using (var f = new ReporteOrdenAbiertaForm(connStr))
            {
                f.ShowDialog(this);
            }
        }

        void btnOrdenCerrada_Click(object sender, EventArgs e)
        {
            using (var f = new ReporteOrdenCerradaForm(connStr))
            {
                f.ShowDialog(this);
            }
        }
    }
}