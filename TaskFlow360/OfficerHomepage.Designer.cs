namespace TaskFlow360
{
    partial class OfficerHomepage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OfficerHomepage));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRaporlar = new System.Windows.Forms.Button();
            this.btnCikis = new System.Windows.Forms.Button();
            this.btnGorevler = new System.Windows.Forms.Button();
            this.btnProfil = new System.Windows.Forms.Button();
            this.btnAnasayfa = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblHosgeldiniz = new System.Windows.Forms.Label();
            this.lblAtananGorevler = new System.Windows.Forms.Label();
            this.PnlGorevler = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTarihSaat = new System.Windows.Forms.Label();
            this.lblMerhaba = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.ikonCikis = new System.Windows.Forms.PictureBox();
            this.ikonRaporlar = new System.Windows.Forms.PictureBox();
            this.ikonProfil = new System.Windows.Forms.PictureBox();
            this.ikonGorevler = new System.Windows.Forms.PictureBox();
            this.ikonAnasayfa = new System.Windows.Forms.PictureBox();
            this.pctrLogo = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ikonCikis)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ikonRaporlar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ikonProfil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ikonGorevler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ikonAnasayfa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctrLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(87)))), ((int)(((byte)(194)))));
            this.panel1.Controls.Add(this.ikonCikis);
            this.panel1.Controls.Add(this.ikonRaporlar);
            this.panel1.Controls.Add(this.btnRaporlar);
            this.panel1.Controls.Add(this.ikonProfil);
            this.panel1.Controls.Add(this.ikonGorevler);
            this.panel1.Controls.Add(this.ikonAnasayfa);
            this.panel1.Controls.Add(this.btnCikis);
            this.panel1.Controls.Add(this.btnGorevler);
            this.panel1.Controls.Add(this.btnProfil);
            this.panel1.Controls.Add(this.btnAnasayfa);
            this.panel1.Controls.Add(this.pctrLogo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(295, 1033);
            this.panel1.TabIndex = 4;
            // 
            // btnRaporlar
            // 
            this.btnRaporlar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(87)))), ((int)(((byte)(194)))));
            this.btnRaporlar.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRaporlar.FlatAppearance.BorderSize = 0;
            this.btnRaporlar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRaporlar.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnRaporlar.ForeColor = System.Drawing.Color.White;
            this.btnRaporlar.Location = new System.Drawing.Point(0, 368);
            this.btnRaporlar.Name = "btnRaporlar";
            this.btnRaporlar.Padding = new System.Windows.Forms.Padding(80, 0, 0, 0);
            this.btnRaporlar.Size = new System.Drawing.Size(295, 60);
            this.btnRaporlar.TabIndex = 9;
            this.btnRaporlar.Text = "Raporlar";
            this.btnRaporlar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRaporlar.UseVisualStyleBackColor = false;
            // 
            // btnCikis
            // 
            this.btnCikis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(87)))), ((int)(((byte)(194)))));
            this.btnCikis.FlatAppearance.BorderSize = 0;
            this.btnCikis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCikis.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnCikis.ForeColor = System.Drawing.Color.White;
            this.btnCikis.Location = new System.Drawing.Point(0, 966);
            this.btnCikis.Name = "btnCikis";
            this.btnCikis.Padding = new System.Windows.Forms.Padding(80, 0, 0, 0);
            this.btnCikis.Size = new System.Drawing.Size(295, 60);
            this.btnCikis.TabIndex = 5;
            this.btnCikis.Text = "Çıkış";
            this.btnCikis.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCikis.UseVisualStyleBackColor = false;
            this.btnCikis.Click += new System.EventHandler(this.button5_Click);
            // 
            // btnGorevler
            // 
            this.btnGorevler.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(87)))), ((int)(((byte)(194)))));
            this.btnGorevler.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnGorevler.FlatAppearance.BorderSize = 0;
            this.btnGorevler.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGorevler.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnGorevler.ForeColor = System.Drawing.Color.White;
            this.btnGorevler.Location = new System.Drawing.Point(0, 308);
            this.btnGorevler.Name = "btnGorevler";
            this.btnGorevler.Padding = new System.Windows.Forms.Padding(80, 0, 0, 0);
            this.btnGorevler.Size = new System.Drawing.Size(295, 60);
            this.btnGorevler.TabIndex = 3;
            this.btnGorevler.Text = "Görevler";
            this.btnGorevler.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGorevler.UseVisualStyleBackColor = false;
            this.btnGorevler.Click += new System.EventHandler(this.btnGorevler_Click);
            // 
            // btnProfil
            // 
            this.btnProfil.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(87)))), ((int)(((byte)(194)))));
            this.btnProfil.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnProfil.FlatAppearance.BorderSize = 0;
            this.btnProfil.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProfil.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnProfil.ForeColor = System.Drawing.Color.White;
            this.btnProfil.Location = new System.Drawing.Point(0, 248);
            this.btnProfil.Name = "btnProfil";
            this.btnProfil.Padding = new System.Windows.Forms.Padding(80, 0, 0, 0);
            this.btnProfil.Size = new System.Drawing.Size(295, 60);
            this.btnProfil.TabIndex = 2;
            this.btnProfil.Text = "Profil";
            this.btnProfil.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProfil.UseVisualStyleBackColor = false;
            this.btnProfil.Click += new System.EventHandler(this.btnProfil_Click);
            // 
            // btnAnasayfa
            // 
            this.btnAnasayfa.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(126)))), ((int)(((byte)(87)))), ((int)(((byte)(194)))));
            this.btnAnasayfa.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAnasayfa.FlatAppearance.BorderSize = 0;
            this.btnAnasayfa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnasayfa.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnAnasayfa.ForeColor = System.Drawing.Color.White;
            this.btnAnasayfa.Location = new System.Drawing.Point(0, 188);
            this.btnAnasayfa.Name = "btnAnasayfa";
            this.btnAnasayfa.Padding = new System.Windows.Forms.Padding(80, 0, 0, 0);
            this.btnAnasayfa.Size = new System.Drawing.Size(295, 60);
            this.btnAnasayfa.TabIndex = 1;
            this.btnAnasayfa.Text = "Anasayfa";
            this.btnAnasayfa.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAnasayfa.UseVisualStyleBackColor = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(103)))), ((int)(((byte)(195)))));
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.lblHosgeldiniz);
            this.panel2.Location = new System.Drawing.Point(294, 108);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1836, 114);
            this.panel2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(29, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(561, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Bugün 3 yeni göreviniz ve 12 devam eden göreviniz bulunmaktadır.";
            // 
            // lblHosgeldiniz
            // 
            this.lblHosgeldiniz.AutoSize = true;
            this.lblHosgeldiniz.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblHosgeldiniz.ForeColor = System.Drawing.Color.White;
            this.lblHosgeldiniz.Location = new System.Drawing.Point(28, 24);
            this.lblHosgeldiniz.Name = "lblHosgeldiniz";
            this.lblHosgeldiniz.Size = new System.Drawing.Size(151, 27);
            this.lblHosgeldiniz.TabIndex = 0;
            this.lblHosgeldiniz.Text = "Hoş Geldiniz";
            // 
            // lblAtananGorevler
            // 
            this.lblAtananGorevler.AutoSize = true;
            this.lblAtananGorevler.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAtananGorevler.Location = new System.Drawing.Point(346, 248);
            this.lblAtananGorevler.Name = "lblAtananGorevler";
            this.lblAtananGorevler.Size = new System.Drawing.Size(199, 22);
            this.lblAtananGorevler.TabIndex = 6;
            this.lblAtananGorevler.Text = "Yeni Atanan Görevler";
            // 
            // PnlGorevler
            // 
            this.PnlGorevler.Location = new System.Drawing.Point(326, 294);
            this.PnlGorevler.Name = "PnlGorevler";
            this.PnlGorevler.Size = new System.Drawing.Size(1590, 486);
            this.PnlGorevler.TabIndex = 7;
            // 
            // lblTarihSaat
            // 
            this.lblTarihSaat.AutoSize = true;
            this.lblTarihSaat.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTarihSaat.Location = new System.Drawing.Point(327, 86);
            this.lblTarihSaat.Name = "lblTarihSaat";
            this.lblTarihSaat.Size = new System.Drawing.Size(0, 20);
            this.lblTarihSaat.TabIndex = 8;
            // 
            // lblMerhaba
            // 
            this.lblMerhaba.AutoSize = true;
            this.lblMerhaba.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblMerhaba.Location = new System.Drawing.Point(1763, 84);
            this.lblMerhaba.Name = "lblMerhaba";
            this.lblMerhaba.Size = new System.Drawing.Size(94, 21);
            this.lblMerhaba.TabIndex = 9;
            this.lblMerhaba.Text = "Merhaba,";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(1863, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 21);
            this.label2.TabIndex = 10;
            this.label2.Text = "İsim Soyisim";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(326, 854);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1590, 140);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.Location = new System.Drawing.Point(349, 805);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(162, 22);
            this.label3.TabIndex = 12;
            this.label3.Text = "Görev İstatistikleri";
            // 
            // ikonCikis
            // 
            this.ikonCikis.BackColor = System.Drawing.Color.Transparent;
            this.ikonCikis.Image = ((System.Drawing.Image)(resources.GetObject("ikonCikis.Image")));
            this.ikonCikis.Location = new System.Drawing.Point(25, 977);
            this.ikonCikis.Name = "ikonCikis";
            this.ikonCikis.Size = new System.Drawing.Size(40, 40);
            this.ikonCikis.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ikonCikis.TabIndex = 11;
            this.ikonCikis.TabStop = false;
            // 
            // ikonRaporlar
            // 
            this.ikonRaporlar.BackColor = System.Drawing.Color.Transparent;
            this.ikonRaporlar.Image = ((System.Drawing.Image)(resources.GetObject("ikonRaporlar.Image")));
            this.ikonRaporlar.Location = new System.Drawing.Point(25, 378);
            this.ikonRaporlar.Name = "ikonRaporlar";
            this.ikonRaporlar.Size = new System.Drawing.Size(40, 40);
            this.ikonRaporlar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ikonRaporlar.TabIndex = 10;
            this.ikonRaporlar.TabStop = false;
            // 
            // ikonProfil
            // 
            this.ikonProfil.BackColor = System.Drawing.Color.Transparent;
            this.ikonProfil.Image = ((System.Drawing.Image)(resources.GetObject("ikonProfil.Image")));
            this.ikonProfil.Location = new System.Drawing.Point(25, 259);
            this.ikonProfil.Name = "ikonProfil";
            this.ikonProfil.Size = new System.Drawing.Size(40, 40);
            this.ikonProfil.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ikonProfil.TabIndex = 8;
            this.ikonProfil.TabStop = false;
            // 
            // ikonGorevler
            // 
            this.ikonGorevler.BackColor = System.Drawing.Color.Transparent;
            this.ikonGorevler.Image = ((System.Drawing.Image)(resources.GetObject("ikonGorevler.Image")));
            this.ikonGorevler.Location = new System.Drawing.Point(25, 319);
            this.ikonGorevler.Name = "ikonGorevler";
            this.ikonGorevler.Size = new System.Drawing.Size(40, 40);
            this.ikonGorevler.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ikonGorevler.TabIndex = 7;
            this.ikonGorevler.TabStop = false;
            // 
            // ikonAnasayfa
            // 
            this.ikonAnasayfa.BackColor = System.Drawing.Color.Transparent;
            this.ikonAnasayfa.Image = ((System.Drawing.Image)(resources.GetObject("ikonAnasayfa.Image")));
            this.ikonAnasayfa.Location = new System.Drawing.Point(25, 200);
            this.ikonAnasayfa.Name = "ikonAnasayfa";
            this.ikonAnasayfa.Size = new System.Drawing.Size(40, 40);
            this.ikonAnasayfa.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ikonAnasayfa.TabIndex = 6;
            this.ikonAnasayfa.TabStop = false;
            // 
            // pctrLogo
            // 
            this.pctrLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pctrLogo.Image = ((System.Drawing.Image)(resources.GetObject("pctrLogo.Image")));
            this.pctrLogo.Location = new System.Drawing.Point(0, 0);
            this.pctrLogo.Name = "pctrLogo";
            this.pctrLogo.Size = new System.Drawing.Size(295, 188);
            this.pctrLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pctrLogo.TabIndex = 0;
            this.pctrLogo.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(1834, 12);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(35, 35);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 3;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(1873, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(35, 35);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 2;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // OfficerHomepage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1920, 1033);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblMerhaba);
            this.Controls.Add(this.lblTarihSaat);
            this.Controls.Add(this.PnlGorevler);
            this.Controls.Add(this.lblAtananGorevler);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OfficerHomepage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OfficerHomepage";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.OfficerHomepage_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ikonCikis)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ikonRaporlar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ikonProfil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ikonGorevler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ikonAnasayfa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctrLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pctrLogo;
        private System.Windows.Forms.Label lblHosgeldiniz;
        private System.Windows.Forms.Label lblAtananGorevler;
        private System.Windows.Forms.Button btnCikis;
        private System.Windows.Forms.PictureBox ikonRaporlar;
        private System.Windows.Forms.Button btnRaporlar;
        private System.Windows.Forms.PictureBox ikonProfil;
        private System.Windows.Forms.PictureBox ikonGorevler;
        private System.Windows.Forms.PictureBox ikonAnasayfa;
        private System.Windows.Forms.Button btnGorevler;
        private System.Windows.Forms.Button btnProfil;
        private System.Windows.Forms.Button btnAnasayfa;
        private System.Windows.Forms.PictureBox ikonCikis;
        private System.Windows.Forms.FlowLayoutPanel PnlGorevler;
        private System.Windows.Forms.Label lblTarihSaat;
        private System.Windows.Forms.Label lblMerhaba;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label3;
    }
}