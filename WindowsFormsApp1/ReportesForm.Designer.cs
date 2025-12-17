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
            this.ClientSize = new System.Drawing.Size(500, 260);
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
            this.Controls.Add(this.btnSabana);
            this.Controls.Add(this.btnCosteIrregular);
            this.Controls.Add(this.btnSabanaEbsKardex);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnSabana;
        private System.Windows.Forms.Button btnCosteIrregular;
        private System.Windows.Forms.Button btnSabanaEbsKardex;
    }
}