namespace WindowsFormsApp1
{
    partial class ReportesForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnSabana = new System.Windows.Forms.Button();
            this.btnCosteIrregular = new System.Windows.Forms.Button();
            this.btnSabanaEbsKardex = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Menú de Reportes";
            this.btnSabana.Location = new System.Drawing.Point(190, 30);
            this.btnSabana.Name = "btnSabana";
            this.btnSabana.Size = new System.Drawing.Size(120, 40);
            this.btnSabana.Text = "Sabana Ingreso";
            this.btnSabana.UseVisualStyleBackColor = true;
            this.btnSabana.Click += new System.EventHandler(this.btnSabana_Click);
            this.btnCosteIrregular.Location = new System.Drawing.Point(190, 80);
            this.btnCosteIrregular.Name = "btnCosteIrregular";
            this.btnCosteIrregular.Size = new System.Drawing.Size(120, 40);
            this.btnCosteIrregular.Text = "Coste EBS Irregular";
            this.btnCosteIrregular.UseVisualStyleBackColor = true;
            this.btnCosteIrregular.Click += new System.EventHandler(this.btnCosteIrregular_Click);
            this.btnSabanaEbsKardex.Location = new System.Drawing.Point(190, 130);
            this.btnSabanaEbsKardex.Name = "btnSabanaEbsKardex";
            this.btnSabanaEbsKardex.Size = new System.Drawing.Size(120, 40);
            this.btnSabanaEbsKardex.Text = "Reporte 3 Tablas";
            this.btnSabanaEbsKardex.UseVisualStyleBackColor = true;
            this.btnSabanaEbsKardex.Click += new System.EventHandler(this.btnSabanaEbsKardex_Click);
            
            this.btnSubfamilia = new System.Windows.Forms.Button();
            this.btnSubfamilia.Location = new System.Drawing.Point(190, 180);
            this.btnSubfamilia.Name = "btnSubfamilia";
            this.btnSubfamilia.Size = new System.Drawing.Size(120, 40);
            this.btnSubfamilia.Text = "Reporte Subfamilia";
            this.btnSubfamilia.UseVisualStyleBackColor = true;
            this.btnSubfamilia.Click += new System.EventHandler(this.btnSubfamilia_Click);
            
            this.btnOrdenAbierta = new System.Windows.Forms.Button();
            this.btnOrdenAbierta.Location = new System.Drawing.Point(190, 230);
            this.btnOrdenAbierta.Name = "btnOrdenAbierta";
            this.btnOrdenAbierta.Size = new System.Drawing.Size(120, 40);
            this.btnOrdenAbierta.Text = "Reporte Orden Abierta";
            this.btnOrdenAbierta.UseVisualStyleBackColor = true;
            this.btnOrdenAbierta.Click += new System.EventHandler(this.btnOrdenAbierta_Click);

            this.btnOrdenCerrada = new System.Windows.Forms.Button();
            this.btnOrdenCerrada.Location = new System.Drawing.Point(190, 280);
            this.btnOrdenCerrada.Name = "btnOrdenCerrada";
            this.btnOrdenCerrada.Size = new System.Drawing.Size(120, 40);
            this.btnOrdenCerrada.Text = "Reporte Orden Cerrada";
            this.btnOrdenCerrada.UseVisualStyleBackColor = true;
            this.btnOrdenCerrada.Click += new System.EventHandler(this.btnOrdenCerrada_Click);

            this.Controls.Add(this.btnSabana);
            this.Controls.Add(this.btnCosteIrregular);
            this.Controls.Add(this.btnSabanaEbsKardex);
            this.Controls.Add(this.btnSubfamilia);
            this.Controls.Add(this.btnOrdenAbierta);
            this.Controls.Add(this.btnOrdenCerrada);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnSabana;
        private System.Windows.Forms.Button btnCosteIrregular;
        private System.Windows.Forms.Button btnSabanaEbsKardex;
        private System.Windows.Forms.Button btnSubfamilia;
        private System.Windows.Forms.Button btnOrdenAbierta;
        private System.Windows.Forms.Button btnOrdenCerrada;
    }
}