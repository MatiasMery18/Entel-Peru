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
            this.btnSubfamilia = new System.Windows.Forms.Button();
            this.btnOrdenAbierta = new System.Windows.Forms.Button();
            this.btnOrdenCerrada = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSabana
            // 
            this.btnSabana.Location = new System.Drawing.Point(36, 70);
            this.btnSabana.Name = "btnSabana";
            this.btnSabana.Size = new System.Drawing.Size(120, 40);
            this.btnSabana.TabIndex = 0;
            this.btnSabana.Text = "Sabana Ingreso";
            this.btnSabana.UseVisualStyleBackColor = true;
            this.btnSabana.Click += new System.EventHandler(this.btnSabana_Click);
            // 
            // btnCosteIrregular
            // 
            this.btnCosteIrregular.Location = new System.Drawing.Point(36, 253);
            this.btnCosteIrregular.Name = "btnCosteIrregular";
            this.btnCosteIrregular.Size = new System.Drawing.Size(120, 40);
            this.btnCosteIrregular.TabIndex = 1;
            this.btnCosteIrregular.Text = "Coste EBS Irregular";
            this.btnCosteIrregular.UseVisualStyleBackColor = true;
            this.btnCosteIrregular.Click += new System.EventHandler(this.btnCosteIrregular_Click);
            // 
            // btnSabanaEbsKardex
            // 
            this.btnSabanaEbsKardex.Location = new System.Drawing.Point(36, 299);
            this.btnSabanaEbsKardex.Name = "btnSabanaEbsKardex";
            this.btnSabanaEbsKardex.Size = new System.Drawing.Size(120, 40);
            this.btnSabanaEbsKardex.TabIndex = 2;
            this.btnSabanaEbsKardex.Text = "Reporte 3 Tablas";
            this.btnSabanaEbsKardex.UseVisualStyleBackColor = true;
            this.btnSabanaEbsKardex.Click += new System.EventHandler(this.btnSabanaEbsKardex_Click);
            // 
            // btnSubfamilia
            // 
            this.btnSubfamilia.Location = new System.Drawing.Point(162, 70);
            this.btnSubfamilia.Name = "btnSubfamilia";
            this.btnSubfamilia.Size = new System.Drawing.Size(120, 40);
            this.btnSubfamilia.TabIndex = 3;
            this.btnSubfamilia.Text = "Reporte Subfamilia";
            this.btnSubfamilia.UseVisualStyleBackColor = true;
            this.btnSubfamilia.Click += new System.EventHandler(this.btnSubfamilia_Click);
            // 
            // btnOrdenAbierta
            // 
            this.btnOrdenAbierta.Location = new System.Drawing.Point(166, 253);
            this.btnOrdenAbierta.Name = "btnOrdenAbierta";
            this.btnOrdenAbierta.Size = new System.Drawing.Size(120, 40);
            this.btnOrdenAbierta.TabIndex = 4;
            this.btnOrdenAbierta.Text = "Reporte Orden Abierta";
            this.btnOrdenAbierta.UseVisualStyleBackColor = true;
            this.btnOrdenAbierta.Click += new System.EventHandler(this.btnOrdenAbierta_Click);
            // 
            // btnOrdenCerrada
            // 
            this.btnOrdenCerrada.Location = new System.Drawing.Point(166, 299);
            this.btnOrdenCerrada.Name = "btnOrdenCerrada";
            this.btnOrdenCerrada.Size = new System.Drawing.Size(120, 40);
            this.btnOrdenCerrada.TabIndex = 5;
            this.btnOrdenCerrada.Text = "Reporte Orden Cerrada";
            this.btnOrdenCerrada.UseVisualStyleBackColor = true;
            this.btnOrdenCerrada.Click += new System.EventHandler(this.btnOrdenCerrada_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(107, 175);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 27);
            this.label1.TabIndex = 6;
            this.label1.Text = "Proceso de Validación";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(345, 27);
            this.label2.TabIndex = 7;
            this.label2.Text = "Procesamiento y Homologación";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(337, 240);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 47);
            this.button1.TabIndex = 8;
            this.button1.Text = "L. Venta Vs L. Mayor";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(337, 293);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(111, 45);
            this.button2.TabIndex = 9;
            this.button2.Text = "L. Venta Vs Bi New";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // ReportesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 394);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSabana);
            this.Controls.Add(this.btnCosteIrregular);
            this.Controls.Add(this.btnSabanaEbsKardex);
            this.Controls.Add(this.btnSubfamilia);
            this.Controls.Add(this.btnOrdenAbierta);
            this.Controls.Add(this.btnOrdenCerrada);
            this.Name = "ReportesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Menú de Reportes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnSabana;
        private System.Windows.Forms.Button btnCosteIrregular;
        private System.Windows.Forms.Button btnSabanaEbsKardex;
        private System.Windows.Forms.Button btnSubfamilia;
        private System.Windows.Forms.Button btnOrdenAbierta;
        private System.Windows.Forms.Button btnOrdenCerrada;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}