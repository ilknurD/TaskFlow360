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
    public partial class OfficerHomepage : Form
    {
        public OfficerHomepage()
        {
            InitializeComponent();
            TarihVeSaatGoster();
            IstatislikleriGoster();
        }

        private void TarihVeSaatGoster()
        {
            DateTime suAn = DateTime.Now;
            string formatliTarihSaat = suAn.ToString("dd MMMM yyyy, dddd HH:mm");
            lblTarihSaat.Text = formatliTarihSaat;
        }

        private void OfficerHomepage_Load(object sender, EventArgs e)
        {
            List<Gorev> gorevListesi = new List<Gorev>()
            {
                new Gorev()
                {
                    GorevAdi = "Haftalık raporun hazırlanması",
                    IlgiliKisi = "Mehmet Demir",
                    TeslimTarihi = DateTime.Now.AddHours(5),
                    Oncelik = "Yüksek",
                    Durum = "Atandı",
                    BaslangicTarihi = DateTime.Now,
                    BitisTarihi = DateTime.Now.AddHours(5)
                },
                new Gorev()
                {
                    GorevAdi = "Müşteri toplantısı için sunum hazırlama",
                    IlgiliKisi = "Ayşe Kaya",
                    TeslimTarihi = DateTime.Now.AddHours(3),
                    Oncelik = "Orta",
                    Durum = "Beklemede",
                    BaslangicTarihi = DateTime.Now,
                    BitisTarihi = DateTime.Now.AddHours(2)
                },
                new Gorev()
                {
                    GorevAdi = "Proje dökümanlarının gözden geçirilmesi",
                    IlgiliKisi = "Mustafa Şahin",
                    TeslimTarihi = DateTime.Now.AddDays(1),
                    Oncelik = "Normal",
                    Durum = "Çözüm Bekliyor",
                    BaslangicTarihi = DateTime.Now.AddDays(-1),
                    BitisTarihi = DateTime.Now.AddDays(1)
                }
            };

            GorevleriGoster(gorevListesi);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void GorevleriGoster(List<Gorev> gorevListesi)
        {
            PnlGorevler.Controls.Clear();
            PnlGorevler.FlowDirection = FlowDirection.TopDown; // Dikey 
            PnlGorevler.WrapContents = false;
            PnlGorevler.AutoScroll = true;

            // Her bir görev için bir panel oluştur
            foreach (var gorev in gorevListesi)
            {
                Panel gorevPaneli = new Panel();
                int panelYuksekligi = 120; // Varsayılan yükseklik
                int panelGenisligi = PnlGorevler.Width - 20; // PnlGorevler genişliğine göre ayarlayın
                gorevPaneli.Size = new Size(panelGenisligi, panelYuksekligi);
                gorevPaneli.BorderStyle = BorderStyle.FixedSingle;
                gorevPaneli.BackColor = Color.White;

                Label lblGorevAdi = new Label();
                lblGorevAdi.Text = gorev.GorevAdi;
                lblGorevAdi.Location = new Point(10, 10);
                lblGorevAdi.Font = new Font("Century Gothic", 12, FontStyle.Bold);
                lblGorevAdi.AutoSize = true; 

                // İlgili kişiyi gösteren label
                Label lblIlgiliKisi = new Label();
                lblIlgiliKisi.Text = "İlgili Kişi: " + gorev.IlgiliKisi;
                lblIlgiliKisi.Location = new Point(10, 30);
                lblIlgiliKisi.Font = new Font("Century Gothic", 10);
                lblIlgiliKisi.AutoSize = true; 

                // Teslim tarihini gösteren label
                Label lblTeslimTarihi = new Label();
                lblTeslimTarihi.Text = "Teslim: " + gorev.TeslimTarihi.ToString("dd MMMM yyyy HH:mm");
                lblTeslimTarihi.Location = new Point(10, 50);
                lblTeslimTarihi.Font = new Font("Century Gothic", 9);
                lblTeslimTarihi.AutoSize = true; 

                // Öncelik durumu için label
                Label lblOncelik = new Label();
                lblOncelik.Text = gorev.Oncelik;
                lblOncelik.Location = new Point(gorevPaneli.Width - 80, 10);
                lblOncelik.Font = new Font("Century Gothic", 10, FontStyle.Bold);
                lblOncelik.AutoSize = true;

                // Önceliğe göre arka plan rengi
                if (gorev.Oncelik == "Yüksek")
                {
                    lblOncelik.ForeColor = Color.Red;
                }
                else if (gorev.Oncelik == "Orta")
                {
                    lblOncelik.ForeColor = Color.Orange;
                }
                else
                {
                    lblOncelik.ForeColor = Color.Green;
                }

                gorevPaneli.Controls.Add(lblGorevAdi);
                gorevPaneli.Controls.Add(lblIlgiliKisi);
                gorevPaneli.Controls.Add(lblTeslimTarihi);
                gorevPaneli.Controls.Add(lblOncelik);

                PnlGorevler.Controls.Add(gorevPaneli);
            }
        }

        private void IstatislikleriGoster()
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
            tamamlananPanel.BackColor = Color.LightGreen;
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
            devamEdenPanel.BackColor = Color.LightSalmon;
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
            gecikenPanel.BackColor = Color.LightCoral;
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
            toplamPanel.BackColor = Color.LightBlue;
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

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.ShowDialog();
        }
    }
}
