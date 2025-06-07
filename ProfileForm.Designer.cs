namespace gizindir
{
    partial class ProfileForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProfileForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveContinue = new System.Windows.Forms.Button();
            this.btnBrowsePhoto = new System.Windows.Forms.Button();
            this.pbProfileImage = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpBirthDate = new System.Windows.Forms.DateTimePicker();
            this.cmbInterestedGender = new System.Windows.Forms.ComboBox();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.pbProfileIcon = new System.Windows.Forms.PictureBox();
            this.txtBio = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.fullName = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProfileImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbProfileIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label4.Location = new System.Drawing.Point(265, 159);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(181, 25);
            label4.TabIndex = 10;
            label4.Text = "İlgi Duyduğu Cinsiyet";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.btnSaveContinue);
            this.panel1.Controls.Add(this.btnBrowsePhoto);
            this.panel1.Controls.Add(this.pbProfileImage);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.dtpBirthDate);
            this.panel1.Controls.Add(this.cmbInterestedGender);
            this.panel1.Controls.Add(label4);
            this.panel1.Controls.Add(this.cmbGender);
            this.panel1.Controls.Add(this.pbProfileIcon);
            this.panel1.Controls.Add(this.txtBio);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtFullName);
            this.panel1.Controls.Add(this.fullName);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Location = new System.Drawing.Point(147, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(484, 476);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnSaveContinue
            // 
            this.btnSaveContinue.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnSaveContinue.FlatAppearance.BorderSize = 0;
            this.btnSaveContinue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveContinue.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnSaveContinue.ForeColor = System.Drawing.Color.White;
            this.btnSaveContinue.Location = new System.Drawing.Point(38, 426);
            this.btnSaveContinue.Name = "btnSaveContinue";
            this.btnSaveContinue.Size = new System.Drawing.Size(408, 35);
            this.btnSaveContinue.TabIndex = 16;
            this.btnSaveContinue.Text = "KAYDET VE DEVAM ET";
            this.btnSaveContinue.UseVisualStyleBackColor = false;
            this.btnSaveContinue.Click += new System.EventHandler(this.btnSaveContinue_Click);
            // 
            // btnBrowsePhoto
            // 
            this.btnBrowsePhoto.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnBrowsePhoto.Location = new System.Drawing.Point(299, 333);
            this.btnBrowsePhoto.Name = "btnBrowsePhoto";
            this.btnBrowsePhoto.Size = new System.Drawing.Size(97, 38);
            this.btnBrowsePhoto.TabIndex = 15;
            this.btnBrowsePhoto.Text = "Gözat";
            this.btnBrowsePhoto.UseVisualStyleBackColor = true;
            this.btnBrowsePhoto.Click += new System.EventHandler(this.btnBrowsePhoto_Click);
            // 
            // pbProfileImage
            // 
            this.pbProfileImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbProfileImage.Location = new System.Drawing.Point(321, 266);
            this.pbProfileImage.Name = "pbProfileImage";
            this.pbProfileImage.Size = new System.Drawing.Size(64, 64);
            this.pbProfileImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbProfileImage.TabIndex = 14;
            this.pbProfileImage.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(265, 225);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label5.Size = new System.Drawing.Size(131, 25);
            this.label5.TabIndex = 13;
            this.label5.Text = "Profil Fotoğrafı";
            // 
            // dtpBirthDate
            // 
            this.dtpBirthDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBirthDate.Location = new System.Drawing.Point(38, 280);
            this.dtpBirthDate.Name = "dtpBirthDate";
            this.dtpBirthDate.Size = new System.Drawing.Size(200, 22);
            this.dtpBirthDate.TabIndex = 12;
            // 
            // cmbInterestedGender
            // 
            this.cmbInterestedGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterestedGender.FormattingEnabled = true;
            this.cmbInterestedGender.Items.AddRange(new object[] {
            "Erkek",
            "Kadın",
            "Diğer"});
            this.cmbInterestedGender.Location = new System.Drawing.Point(270, 187);
            this.cmbInterestedGender.Name = "cmbInterestedGender";
            this.cmbInterestedGender.Size = new System.Drawing.Size(166, 24);
            this.cmbInterestedGender.TabIndex = 11;
            // 
            // cmbGender
            // 
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Items.AddRange(new object[] {
            "Erkek",
            "Kadın",
            "Diğer"});
            this.cmbGender.Location = new System.Drawing.Point(38, 187);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(120, 24);
            this.cmbGender.TabIndex = 9;
            // 
            // pbProfileIcon
            // 
            this.pbProfileIcon.Image = ((System.Drawing.Image)(resources.GetObject("pbProfileIcon.Image")));
            this.pbProfileIcon.Location = new System.Drawing.Point(59, 3);
            this.pbProfileIcon.Name = "pbProfileIcon";
            this.pbProfileIcon.Size = new System.Drawing.Size(64, 64);
            this.pbProfileIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbProfileIcon.TabIndex = 8;
            this.pbProfileIcon.TabStop = false;
            // 
            // txtBio
            // 
            this.txtBio.BackColor = System.Drawing.Color.White;
            this.txtBio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBio.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtBio.Location = new System.Drawing.Point(38, 333);
            this.txtBio.Multiline = true;
            this.txtBio.Name = "txtBio";
            this.txtBio.Size = new System.Drawing.Size(243, 69);
            this.txtBio.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(33, 305);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 25);
            this.label3.TabIndex = 6;
            this.label3.Text = "Biyografi";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(33, 250);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Doğum Tarihi";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cinsiyet";
            // 
            // txtFullName
            // 
            this.txtFullName.BackColor = System.Drawing.Color.White;
            this.txtFullName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFullName.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtFullName.Location = new System.Drawing.Point(38, 113);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new System.Drawing.Size(314, 31);
            this.txtFullName.TabIndex = 3;
            // 
            // fullName
            // 
            this.fullName.AutoSize = true;
            this.fullName.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fullName.Location = new System.Drawing.Point(33, 85);
            this.fullName.Name = "fullName";
            this.fullName.Size = new System.Drawing.Size(90, 25);
            this.fullName.TabIndex = 2;
            this.fullName.Text = "Ad Soyad";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(87)))), ((int)(((byte)(194)))));
            this.lblTitle.Location = new System.Drawing.Point(150, 21);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(296, 38);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "👤 Profilini Tamamla";
            // 
            // ProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ProfileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProfileForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbProfileImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbProfileIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label fullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.TextBox txtBio;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbInterestedGender;
        private System.Windows.Forms.ComboBox cmbGender;
        private System.Windows.Forms.PictureBox pbProfileIcon;
        private System.Windows.Forms.Button btnBrowsePhoto;
        private System.Windows.Forms.PictureBox pbProfileImage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpBirthDate;
        private System.Windows.Forms.Button btnSaveContinue;
    }
}