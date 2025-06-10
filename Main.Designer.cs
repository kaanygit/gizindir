namespace gizindir
{
    partial class Main
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
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.panelProfileCard = new System.Windows.Forms.Panel();
            this.btnDislike = new System.Windows.Forms.Button();
            this.btnLike = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblUniversity = new System.Windows.Forms.Label();
            this.lblProfession = new System.Windows.Forms.Label();
            this.lblNameAge = new System.Windows.Forms.Label();
            this.pbMainProfile = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelSidebar.SuspendLayout();
            this.panelProfileCard.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMainProfile)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelSidebar.Controls.Add(this.label1);
            this.panelSidebar.Controls.Add(this.flowLayoutPanel1);
            this.panelSidebar.Controls.Add(this.btnLogout);
            this.panelSidebar.Location = new System.Drawing.Point(12, 12);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(353, 529);
            this.panelSidebar.TabIndex = 0;
            // 
            // panelProfileCard
            // 
            this.panelProfileCard.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelProfileCard.Controls.Add(this.btnDislike);
            this.panelProfileCard.Controls.Add(this.btnLike);
            this.panelProfileCard.Controls.Add(this.panel1);
            this.panelProfileCard.Controls.Add(this.pbMainProfile);
            this.panelProfileCard.Controls.Add(this.lblTitle);
            this.panelProfileCard.Location = new System.Drawing.Point(371, 12);
            this.panelProfileCard.Name = "panelProfileCard";
            this.panelProfileCard.Size = new System.Drawing.Size(599, 529);
            this.panelProfileCard.TabIndex = 1;
            // 
            // btnDislike
            // 
            this.btnDislike.BackColor = System.Drawing.Color.LightGray;
            this.btnDislike.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDislike.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnDislike.ForeColor = System.Drawing.Color.Black;
            this.btnDislike.Location = new System.Drawing.Point(344, 503);
            this.btnDislike.Name = "btnDislike";
            this.btnDislike.Size = new System.Drawing.Size(75, 23);
            this.btnDislike.TabIndex = 5;
            this.btnDislike.Text = "X ";
            this.btnDislike.UseVisualStyleBackColor = false;
            // 
            // btnLike
            // 
            this.btnLike.BackColor = System.Drawing.Color.LightGreen;
            this.btnLike.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLike.ForeColor = System.Drawing.Color.White;
            this.btnLike.Location = new System.Drawing.Point(197, 503);
            this.btnLike.Name = "btnLike";
            this.btnLike.Size = new System.Drawing.Size(75, 23);
            this.btnLike.TabIndex = 4;
            this.btnLike.Text = "✓";
            this.btnLike.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblUniversity);
            this.panel1.Controls.Add(this.lblProfession);
            this.panel1.Controls.Add(this.lblNameAge);
            this.panel1.Location = new System.Drawing.Point(134, 405);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(349, 97);
            this.panel1.TabIndex = 3;
            // 
            // lblUniversity
            // 
            this.lblUniversity.AutoSize = true;
            this.lblUniversity.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblUniversity.ForeColor = System.Drawing.Color.Black;
            this.lblUniversity.Location = new System.Drawing.Point(3, 63);
            this.lblUniversity.Name = "lblUniversity";
            this.lblUniversity.Size = new System.Drawing.Size(145, 25);
            this.lblUniversity.TabIndex = 6;
            this.lblUniversity.Text = "University of Ege";
            // 
            // lblProfession
            // 
            this.lblProfession.AutoSize = true;
            this.lblProfession.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblProfession.ForeColor = System.Drawing.Color.Black;
            this.lblProfession.Location = new System.Drawing.Point(3, 38);
            this.lblProfession.Name = "lblProfession";
            this.lblProfession.Size = new System.Drawing.Size(71, 25);
            this.lblProfession.TabIndex = 5;
            this.lblProfession.Text = "Gizindir";
            // 
            // lblNameAge
            // 
            this.lblNameAge.AutoSize = true;
            this.lblNameAge.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblNameAge.ForeColor = System.Drawing.Color.Black;
            this.lblNameAge.Location = new System.Drawing.Point(3, 0);
            this.lblNameAge.Name = "lblNameAge";
            this.lblNameAge.Size = new System.Drawing.Size(177, 38);
            this.lblNameAge.TabIndex = 4;
            this.lblNameAge.Text = "Gizem Topal";
            // 
            // pbMainProfile
            // 
            this.pbMainProfile.Location = new System.Drawing.Point(134, 70);
            this.pbMainProfile.Name = "pbMainProfile";
            this.pbMainProfile.Size = new System.Drawing.Size(349, 329);
            this.pbMainProfile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbMainProfile.TabIndex = 2;
            this.pbMainProfile.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(466, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(119, 38);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Gizindir";
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.Red;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(3, 491);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(347, 35);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "Çıkış Yap";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 51);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(353, 434);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 38);
            this.label1.TabIndex = 6;
            this.label1.Text = "Konuşmalar";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(982, 553);
            this.Controls.Add(this.panelProfileCard);
            this.Controls.Add(this.panelSidebar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gizİndir";
            this.Load += new System.EventHandler(this.Main_Load);
            this.panelSidebar.ResumeLayout(false);
            this.panelSidebar.PerformLayout();
            this.panelProfileCard.ResumeLayout(false);
            this.panelProfileCard.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMainProfile)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelProfileCard;
        private System.Windows.Forms.Button btnDislike;
        private System.Windows.Forms.Button btnLike;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblUniversity;
        private System.Windows.Forms.Label lblProfession;
        private System.Windows.Forms.Label lblNameAge;
        private System.Windows.Forms.PictureBox pbMainProfile;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}