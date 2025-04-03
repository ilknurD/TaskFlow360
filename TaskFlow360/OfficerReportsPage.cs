using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class OfficerReportsPage : Form
    {
        public OfficerReportsPage()
        {
            InitializeComponent();
            RaporlariGoster();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerHomepage officerHomepage = new OfficerHomepage();
            officerHomepage.Show();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerProfile officerProfile = new OfficerProfile();   
            officerProfile.Show();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerReportsPage officerReportsPage = new OfficerReportsPage();
            officerReportsPage.Show();
        }

        private void RaporlariGoster()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.AutoScroll = true;

            Size panelBoyut = new Size(270, 100);
            Padding panelMargin = new Padding(15, 0, 15, 0); // Sol, üst, sağ, alt boşluk

            // Tamamlanan Görevler Kutucuğu
            Panel tamamlananPanel = new Panel();
            tamamlananPanel.Size = panelBoyut;
            tamamlananPanel.BackColor = Color.FromArgb(99, 62, 187);
            tamamlananPanel.Margin = panelMargin; // Panel aralarındaki mesafe
            Label lblTamamlananSayisi = new Label();
            lblTamamlananSayisi.Text = "24";
            lblTamamlananSayisi.Font = new Font("Arial", 18, FontStyle.Bold);
            lblTamamlananSayisi.Location = new Point(50, 20);
            Label lblTamamlanan = new Label();
            lblTamamlanan.Text = "Tamamlanan";
            lblTamamlanan.Location = new Point(30, 60);
            lblTamamlanan.Font = new Font("Arial", 10);

            // Devam Eden Görevler Kutucuğu
            Panel devamEdenPanel = new Panel();
            devamEdenPanel.Size = panelBoyut;
            devamEdenPanel.BackColor = Color.FromArgb(241, 60, 89);
            devamEdenPanel.Margin = panelMargin;
            Label lblDevamEdenSayisi = new Label();
            lblDevamEdenSayisi.Text = "12";
            lblDevamEdenSayisi.Font = new Font("Arial", 18, FontStyle.Bold);
            lblDevamEdenSayisi.Location = new Point(50, 20);
            Label lblDevamEden = new Label();
            lblDevamEden.Text = "Devam Eden";
            lblDevamEden.Location = new Point(30, 60);
            lblDevamEden.Font = new Font("Arial", 10);

            // Geciken Görevler Kutucuğu
            Panel gecikenPanel = new Panel();
            gecikenPanel.Size = panelBoyut; 
            gecikenPanel.BackColor = Color.FromArgb(242, 179, 96);
            gecikenPanel.Margin = panelMargin;
            Label lblGecikenSayisi = new Label();
            lblGecikenSayisi.Text = "3";
            lblGecikenSayisi.Font = new Font("Arial", 18, FontStyle.Bold);
            lblGecikenSayisi.Location = new Point(50, 20);
            Label lblGeciken = new Label();
            lblGeciken.Text = "Geciken";
            lblGeciken.Location = new Point(50, 60);
            lblGeciken.Font = new Font("Arial", 10);

            // Toplam Görevler Kutucuğu
            Panel toplamPanel = new Panel();
            toplamPanel.Size = panelBoyut; 
            toplamPanel.BackColor = Color.FromArgb(188, 108, 202);
            toplamPanel.Margin = panelMargin;
            Label lblToplamSayisi = new Label();
            lblToplamSayisi.Text = "39";
            lblToplamSayisi.Font = new Font("Arial", 18, FontStyle.Bold);
            lblToplamSayisi.Location = new Point(50, 20);
            Label lblToplam = new Label();
            lblToplam.Text = "Toplam";
            lblToplam.Location = new Point(50, 60);
            lblToplam.Font = new Font("Arial", 10);

            // Kutucukları FlowLayoutPanel'e ekleyin
            tamamlananPanel.Controls.Add(lblTamamlananSayisi);
            tamamlananPanel.Controls.Add(lblTamamlanan);
            flowLayoutPanel1.Controls.Add(tamamlananPanel);

            devamEdenPanel.Controls.Add(lblDevamEdenSayisi);
            devamEdenPanel.Controls.Add(lblDevamEden);
            flowLayoutPanel1.Controls.Add(devamEdenPanel);

            gecikenPanel.Controls.Add(lblGecikenSayisi);
            gecikenPanel.Controls.Add(lblGeciken);
            flowLayoutPanel1.Controls.Add(gecikenPanel);

            toplamPanel.Controls.Add(lblToplamSayisi);
            toplamPanel.Controls.Add(lblToplam);
            flowLayoutPanel1.Controls.Add(toplamPanel);
        }
    }
}
