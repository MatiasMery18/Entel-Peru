namespace WindowsFormsApp1
{
    partial class AdminForm
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
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.txtAdminUser = new System.Windows.Forms.TextBox();
            this.txtAdminPass = new System.Windows.Forms.TextBox();
            this.txtAdminName = new System.Windows.Forms.TextBox();
            this.txtAdminEmail = new System.Windows.Forms.TextBox();
            this.cmbAdminRole = new System.Windows.Forms.ComboBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblPass = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblApellidoPat = new System.Windows.Forms.Label();
            this.lblApellidoMat = new System.Windows.Forms.Label();
            this.txtApellidoPat = new System.Windows.Forms.TextBox();
            this.txtApellidoMat = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.SuspendLayout();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 520);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Panel de Administración";
            this.dgvUsers.Location = new System.Drawing.Point(20, 20);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.Size = new System.Drawing.Size(600, 220);
            this.txtAdminUser.Location = new System.Drawing.Point(90, 260);
            this.txtAdminUser.Name = "txtAdminUser";
            this.txtAdminUser.Size = new System.Drawing.Size(160, 20);
            this.txtAdminPass.Location = new System.Drawing.Point(90, 290);
            this.txtAdminPass.Name = "txtAdminPass";
            this.txtAdminPass.Size = new System.Drawing.Size(160, 20);
            this.txtAdminPass.UseSystemPasswordChar = true;
            this.txtAdminName.Location = new System.Drawing.Point(90, 320);
            this.txtAdminName.Name = "txtAdminName";
            this.txtAdminName.Size = new System.Drawing.Size(160, 20);
            this.txtAdminEmail.Location = new System.Drawing.Point(90, 410);
            this.txtAdminEmail.Name = "txtAdminEmail";
            this.txtAdminEmail.Size = new System.Drawing.Size(160, 20);
            this.cmbAdminRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdminRole.Items.AddRange(new object[] { "Admin", "User" });
            this.cmbAdminRole.Location = new System.Drawing.Point(90, 440);
            this.cmbAdminRole.Name = "cmbAdminRole";
            this.cmbAdminRole.Size = new System.Drawing.Size(160, 21);
            this.btnCreate.Location = new System.Drawing.Point(280, 255);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(90, 28);
            this.btnCreate.Text = "Crear";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            this.btnUpdate.Location = new System.Drawing.Point(280, 288);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(90, 28);
            this.btnUpdate.Text = "Actualizar";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            this.btnDelete.Location = new System.Drawing.Point(280, 321);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 28);
            this.btnDelete.Text = "Eliminar";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnReload.Location = new System.Drawing.Point(380, 255);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(90, 28);
            this.btnReload.Text = "Recargar";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(20, 263);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(46, 13);
            this.lblUser.Text = "Usuario";
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(20, 293);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(64, 13);
            this.lblPass.Text = "Contraseña";
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(20, 443);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(23, 13);
            this.lblRole.Text = "Rol";
            this.lblName = new System.Windows.Forms.Label();
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 323);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(47, 13);
            this.lblName.Text = "Nombre";
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(20, 413);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(32, 13);
            this.lblEmail.Text = "Email";
            this.lblApellidoPat.AutoSize = true;
            this.lblApellidoPat.Location = new System.Drawing.Point(20, 353);
            this.lblApellidoPat.Name = "lblApellidoPat";
            this.lblApellidoPat.Size = new System.Drawing.Size(87, 13);
            this.lblApellidoPat.Text = "Apellido Paterno";
            this.lblApellidoMat.AutoSize = true;
            this.lblApellidoMat.Location = new System.Drawing.Point(20, 383);
            this.lblApellidoMat.Name = "lblApellidoMat";
            this.lblApellidoMat.Size = new System.Drawing.Size(90, 13);
            this.lblApellidoMat.Text = "Apellido Materno";
            this.txtApellidoPat.Location = new System.Drawing.Point(90, 350);
            this.txtApellidoPat.Name = "txtApellidoPat";
            this.txtApellidoPat.Size = new System.Drawing.Size(160, 20);
            this.txtApellidoMat.Location = new System.Drawing.Point(90, 380);
            this.txtApellidoMat.Name = "txtApellidoMat";
            this.txtApellidoMat.Size = new System.Drawing.Size(160, 20);
            this.Controls.Add(this.dgvUsers);
            this.Controls.Add(this.txtAdminUser);
            this.Controls.Add(this.txtAdminPass);
            this.Controls.Add(this.txtAdminName);
            this.Controls.Add(this.txtApellidoPat);
            this.Controls.Add(this.txtApellidoMat);
            this.Controls.Add(this.txtAdminEmail);
            this.Controls.Add(this.cmbAdminRole);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.lblPass);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.lblApellidoPat);
            this.Controls.Add(this.lblApellidoMat);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.TextBox txtAdminUser;
        private System.Windows.Forms.TextBox txtAdminPass;
        private System.Windows.Forms.ComboBox cmbAdminRole;
        private System.Windows.Forms.TextBox txtAdminName;
        private System.Windows.Forms.TextBox txtAdminEmail;
        private System.Windows.Forms.TextBox txtApellidoPat;
        private System.Windows.Forms.TextBox txtApellidoMat;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblApellidoPat;
        private System.Windows.Forms.Label lblApellidoMat;
    }
}