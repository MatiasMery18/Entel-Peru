namespace WindowsFormsApp1
{
    partial class SabanaIngresoForm
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
            this.dtDesde = new System.Windows.Forms.DateTimePicker();
            this.dtHasta = new System.Windows.Forms.DateTimePicker();
            this.lblDesde = new System.Windows.Forms.Label();
            this.lblHasta = new System.Windows.Forms.Label();
            
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.dgvSabana = new System.Windows.Forms.DataGridView();
            this.lblRows = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSabana)).BeginInit();
            this.SuspendLayout();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sabana Ingreso";
            
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(20, 20);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(38, 13);
            this.lblDesde.Text = "Desde";
            this.dtDesde.Location = new System.Drawing.Point(70, 16);
            this.dtDesde.Name = "dtDesde";
            this.dtDesde.Size = new System.Drawing.Size(200, 20);
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(290, 20);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(35, 13);
            this.lblHasta.Text = "Hasta";
            this.dtHasta.Location = new System.Drawing.Point(335, 16);
            this.dtHasta.Name = "dtHasta";
            this.dtHasta.Size = new System.Drawing.Size(200, 20);
            this.btnBuscar.Location = new System.Drawing.Point(20, 50);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(100, 28);
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.btnExportar.Location = new System.Drawing.Point(130, 50);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(140, 28);
            this.btnExportar.Text = "Exportar a Excel (CSV)";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            this.dgvSabana.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSabana.Location = new System.Drawing.Point(20, 90);
            this.dgvSabana.Name = "dgvSabana";
            this.dgvSabana.Size = new System.Drawing.Size(860, 470);
            this.dgvSabana.ReadOnly = true;
            this.dgvSabana.AllowUserToAddRows = false;
            this.dgvSabana.AllowUserToDeleteRows = false;
            this.dgvSabana.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.lblRows.Location = new System.Drawing.Point(290, 55);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.dtDesde);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtHasta);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.dgvSabana);
            this.Controls.Add(this.lblRows);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSabana)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.DateTimePicker dtDesde;
        private System.Windows.Forms.DateTimePicker dtHasta;
        private System.Windows.Forms.Label lblDesde;
        private System.Windows.Forms.Label lblHasta;
        
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.DataGridView dgvSabana;
        private System.Windows.Forms.Label lblRows;
    }
}