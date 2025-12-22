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
            this.btnFiltro = new System.Windows.Forms.Button();
            this.dgvSabana = new System.Windows.Forms.DataGridView();
            this.lblRows = new System.Windows.Forms.Label();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.cmbPeriodo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSabana)).BeginInit();
            this.SuspendLayout();
            // 
            // dtDesde
            // 
            this.dtDesde.Location = new System.Drawing.Point(70, 16);
            this.dtDesde.Name = "dtDesde";
            this.dtDesde.Size = new System.Drawing.Size(200, 20);
            this.dtDesde.TabIndex = 1;
            this.dtDesde.Visible = false;
            // 
            // dtHasta
            // 
            this.dtHasta.Location = new System.Drawing.Point(335, 16);
            this.dtHasta.Name = "dtHasta";
            this.dtHasta.Size = new System.Drawing.Size(200, 20);
            this.dtHasta.TabIndex = 3;
            this.dtHasta.Visible = false;
            // 
            // lblDesde
            // 
            this.lblDesde.AutoSize = true;
            this.lblDesde.Location = new System.Drawing.Point(20, 20);
            this.lblDesde.Name = "lblDesde";
            this.lblDesde.Size = new System.Drawing.Size(38, 13);
            this.lblDesde.TabIndex = 0;
            this.lblDesde.Text = "Desde";
            this.lblDesde.Visible = false;
            // 
            // lblHasta
            // 
            this.lblHasta.AutoSize = true;
            this.lblHasta.Location = new System.Drawing.Point(290, 20);
            this.lblHasta.Name = "lblHasta";
            this.lblHasta.Size = new System.Drawing.Size(35, 13);
            this.lblHasta.TabIndex = 2;
            this.lblHasta.Text = "Hasta";
            this.lblHasta.Visible = false;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(280, 15);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(100, 28);
            this.btnBuscar.TabIndex = 4;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // btnExportar
            // 
            this.btnExportar.Location = new System.Drawing.Point(390, 15);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(140, 28);
            this.btnExportar.TabIndex = 5;
            this.btnExportar.Text = "Exportar a Excel (CSV)";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // btnFiltro
            // 
            this.btnFiltro.Location = new System.Drawing.Point(540, 15);
            this.btnFiltro.Name = "btnFiltro";
            this.btnFiltro.Size = new System.Drawing.Size(100, 28);
            this.btnFiltro.TabIndex = 6;
            this.btnFiltro.Text = "Filtro";
            this.btnFiltro.UseVisualStyleBackColor = true;
            this.btnFiltro.Click += new System.EventHandler(this.btnFiltro_Click);
            // 
            // dgvSabana
            // 
            this.dgvSabana.AllowUserToAddRows = false;
            this.dgvSabana.AllowUserToDeleteRows = false;
            this.dgvSabana.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSabana.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvSabana.Location = new System.Drawing.Point(20, 60);
            this.dgvSabana.Name = "dgvSabana";
            this.dgvSabana.ReadOnly = true;
            this.dgvSabana.Size = new System.Drawing.Size(860, 520);
            this.dgvSabana.TabIndex = 7;
            // 
            // lblRows
            // 
            this.lblRows.Location = new System.Drawing.Point(660, 20);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new System.Drawing.Size(200, 20);
            this.lblRows.TabIndex = 8;
            this.lblRows.Text = "";
            // 
            // lblPeriodo
            // 
            this.lblPeriodo.AutoSize = true;
            this.lblPeriodo.Location = new System.Drawing.Point(20, 23);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(43, 13);
            this.lblPeriodo.TabIndex = 9;
            this.lblPeriodo.Text = "Periodo";
            // 
            // cmbPeriodo
            // 
            this.cmbPeriodo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPeriodo.FormattingEnabled = true;
            this.cmbPeriodo.Location = new System.Drawing.Point(70, 19);
            this.cmbPeriodo.Name = "cmbPeriodo";
            this.cmbPeriodo.Size = new System.Drawing.Size(180, 21);
            this.cmbPeriodo.TabIndex = 10;
            // 
            // SabanaIngresoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.cmbPeriodo);
            this.Controls.Add(this.lblPeriodo);
            this.Controls.Add(this.lblDesde);
            this.Controls.Add(this.dtDesde);
            this.Controls.Add(this.lblHasta);
            this.Controls.Add(this.dtHasta);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnFiltro);
            this.Controls.Add(this.dgvSabana);
            this.Controls.Add(this.lblRows);
            this.Name = "SabanaIngresoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sabana Ingreso";
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
        private System.Windows.Forms.Button btnFiltro;
        private System.Windows.Forms.DataGridView dgvSabana;
        private System.Windows.Forms.Label lblRows;
        private System.Windows.Forms.Label lblPeriodo;
        private System.Windows.Forms.ComboBox cmbPeriodo;
    }
}
