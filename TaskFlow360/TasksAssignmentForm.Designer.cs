namespace TaskFlow360
{
    partial class TasksAssignmentForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblBaslik = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.txtBaslik = new System.Windows.Forms.TextBox();
            this.txtAciklama = new System.Windows.Forms.TextBox();
            this.txtCagriID = new System.Windows.Forms.TextBox();
            this.lblpnl1baslik = new System.Windows.Forms.Label();
            this.lblpnl1Aciklama = new System.Windows.Forms.Label();
            this.lblpnl1Cagriid = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnFiltrele = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.lblpnl2bolum = new System.Windows.Forms.Label();
            this.lblpnl2Departman = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblpnl3DGVEkipBaslik = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(103)))), ((int)(((byte)(195)))));
            this.panel2.Controls.Add(this.lblBaslik);
            this.panel2.Location = new System.Drawing.Point(0, -25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1920, 114);
            this.panel2.TabIndex = 12;
            // 
            // lblBaslik
            // 
            this.lblBaslik.AutoSize = true;
            this.lblBaslik.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBaslik.ForeColor = System.Drawing.Color.White;
            this.lblBaslik.Location = new System.Drawing.Point(28, 51);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.Size = new System.Drawing.Size(279, 34);
            this.lblBaslik.TabIndex = 0;
            this.lblBaslik.Text = "Çağrı Atama Formu";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.btnKaydet);
            this.panel1.Controls.Add(this.txtBaslik);
            this.panel1.Controls.Add(this.txtAciklama);
            this.panel1.Controls.Add(this.txtCagriID);
            this.panel1.Controls.Add(this.lblpnl1baslik);
            this.panel1.Controls.Add(this.lblpnl1Aciklama);
            this.panel1.Controls.Add(this.lblpnl1Cagriid);
            this.panel1.Location = new System.Drawing.Point(34, 129);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1874, 111);
            this.panel1.TabIndex = 13;
            // 
            // btnKaydet
            // 
            this.btnKaydet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(34)))), ((int)(((byte)(158)))));
            this.btnKaydet.FlatAppearance.BorderSize = 0;
            this.btnKaydet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKaydet.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnKaydet.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnKaydet.Location = new System.Drawing.Point(1721, 36);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(119, 46);
            this.btnKaydet.TabIndex = 25;
            this.btnKaydet.Text = "Kaydet";
            this.btnKaydet.UseVisualStyleBackColor = false;
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            // 
            // txtBaslik
            // 
            this.txtBaslik.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtBaslik.Location = new System.Drawing.Point(930, 10);
            this.txtBaslik.Name = "txtBaslik";
            this.txtBaslik.Size = new System.Drawing.Size(751, 30);
            this.txtBaslik.TabIndex = 5;
            // 
            // txtAciklama
            // 
            this.txtAciklama.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtAciklama.Location = new System.Drawing.Point(121, 70);
            this.txtAciklama.Name = "txtAciklama";
            this.txtAciklama.Size = new System.Drawing.Size(1560, 30);
            this.txtAciklama.TabIndex = 4;
            // 
            // txtCagriID
            // 
            this.txtCagriID.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.txtCagriID.Location = new System.Drawing.Point(121, 10);
            this.txtCagriID.Name = "txtCagriID";
            this.txtCagriID.Size = new System.Drawing.Size(442, 30);
            this.txtCagriID.TabIndex = 3;
            // 
            // lblpnl1baslik
            // 
            this.lblpnl1baslik.AutoSize = true;
            this.lblpnl1baslik.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblpnl1baslik.Location = new System.Drawing.Point(865, 13);
            this.lblpnl1baslik.Name = "lblpnl1baslik";
            this.lblpnl1baslik.Size = new System.Drawing.Size(59, 21);
            this.lblpnl1baslik.TabIndex = 2;
            this.lblpnl1baslik.Text = "Başlık:";
            // 
            // lblpnl1Aciklama
            // 
            this.lblpnl1Aciklama.AutoSize = true;
            this.lblpnl1Aciklama.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblpnl1Aciklama.Location = new System.Drawing.Point(17, 73);
            this.lblpnl1Aciklama.Name = "lblpnl1Aciklama";
            this.lblpnl1Aciklama.Size = new System.Drawing.Size(98, 21);
            this.lblpnl1Aciklama.TabIndex = 1;
            this.lblpnl1Aciklama.Text = "Açıklama:";
            // 
            // lblpnl1Cagriid
            // 
            this.lblpnl1Cagriid.AutoSize = true;
            this.lblpnl1Cagriid.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblpnl1Cagriid.Location = new System.Drawing.Point(30, 13);
            this.lblpnl1Cagriid.Name = "lblpnl1Cagriid";
            this.lblpnl1Cagriid.Size = new System.Drawing.Size(85, 21);
            this.lblpnl1Cagriid.TabIndex = 0;
            this.lblpnl1Cagriid.Text = "Çağrı ID:";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.btnFiltrele);
            this.panel3.Controls.Add(this.textBox5);
            this.panel3.Controls.Add(this.textBox4);
            this.panel3.Controls.Add(this.lblpnl2bolum);
            this.panel3.Controls.Add(this.lblpnl2Departman);
            this.panel3.Location = new System.Drawing.Point(34, 262);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1874, 95);
            this.panel3.TabIndex = 14;
            // 
            // btnFiltrele
            // 
            this.btnFiltrele.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(34)))), ((int)(((byte)(158)))));
            this.btnFiltrele.FlatAppearance.BorderSize = 0;
            this.btnFiltrele.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFiltrele.Font = new System.Drawing.Font("Century Gothic", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnFiltrele.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnFiltrele.Location = new System.Drawing.Point(1623, 32);
            this.btnFiltrele.Name = "btnFiltrele";
            this.btnFiltrele.Size = new System.Drawing.Size(94, 30);
            this.btnFiltrele.TabIndex = 24;
            this.btnFiltrele.Text = "Filtrele";
            this.btnFiltrele.UseVisualStyleBackColor = false;
            // 
            // textBox5
            // 
            this.textBox5.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.textBox5.Location = new System.Drawing.Point(939, 32);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(628, 30);
            this.textBox5.TabIndex = 5;
            // 
            // textBox4
            // 
            this.textBox4.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.textBox4.Location = new System.Drawing.Point(139, 32);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(424, 30);
            this.textBox4.TabIndex = 4;
            // 
            // lblpnl2bolum
            // 
            this.lblpnl2bolum.AutoSize = true;
            this.lblpnl2bolum.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblpnl2bolum.Location = new System.Drawing.Point(865, 35);
            this.lblpnl2bolum.Name = "lblpnl2bolum";
            this.lblpnl2bolum.Size = new System.Drawing.Size(68, 21);
            this.lblpnl2bolum.TabIndex = 2;
            this.lblpnl2bolum.Text = "Bölüm:";
            // 
            // lblpnl2Departman
            // 
            this.lblpnl2Departman.AutoSize = true;
            this.lblpnl2Departman.Font = new System.Drawing.Font("Century Gothic", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblpnl2Departman.Location = new System.Drawing.Point(17, 35);
            this.lblpnl2Departman.Name = "lblpnl2Departman";
            this.lblpnl2Departman.Size = new System.Drawing.Size(116, 21);
            this.lblpnl2Departman.TabIndex = 1;
            this.lblpnl2Departman.Text = "Departman:";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Controls.Add(this.lblpnl3DGVEkipBaslik);
            this.panel4.Location = new System.Drawing.Point(34, 380);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1874, 668);
            this.panel4.TabIndex = 15;
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(33, 64);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1816, 575);
            this.dataGridView1.TabIndex = 2;
            // 
            // lblpnl3DGVEkipBaslik
            // 
            this.lblpnl3DGVEkipBaslik.AutoSize = true;
            this.lblpnl3DGVEkipBaslik.Font = new System.Drawing.Font("Century Gothic", 13.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblpnl3DGVEkipBaslik.Location = new System.Drawing.Point(29, 34);
            this.lblpnl3DGVEkipBaslik.Name = "lblpnl3DGVEkipBaslik";
            this.lblpnl3DGVEkipBaslik.Size = new System.Drawing.Size(82, 26);
            this.lblpnl3DGVEkipBaslik.TabIndex = 1;
            this.lblpnl3DGVEkipBaslik.Text = "Ekibim";
            // 
            // TasksAssignmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1102);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TasksAssignmentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CagriAtamaForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.TasksAssignmentForm_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblBaslik;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblpnl1baslik;
        private System.Windows.Forms.Label lblpnl1Aciklama;
        private System.Windows.Forms.Label lblpnl1Cagriid;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblpnl2bolum;
        private System.Windows.Forms.Label lblpnl2Departman;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblpnl3DGVEkipBaslik;
        private System.Windows.Forms.TextBox txtCagriID;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtBaslik;
        private System.Windows.Forms.TextBox txtAciklama;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button btnFiltrele;
        private System.Windows.Forms.Button btnKaydet;
    }
}