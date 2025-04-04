using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

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

            Size panelBoyut = new Size(200, 100); // Panel boyutları
            Padding panelMargin = new Padding(15, 0, 15, 0); // Panel aralarındaki mesafe

            void PanelEkle(Color arkaPlan, string sayi, string metin)
            {
                Panel panel = new Panel
                {
                    Size = panelBoyut,
                    BackColor = arkaPlan,
                    Margin = panelMargin
                };

                Label lblSayi = new Label
                {
                    Text = sayi,
                    Font = new Font("Arial", 18, FontStyle.Bold),
                    AutoSize = false,
                    Size = new Size(panel.Width, 30), // Genişlik panel kadar
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };

                Label lblMetin = new Label
                {
                    Text = metin,
                    Font = new Font("Arial", 10),
                    AutoSize = false,
                    Size = new Size(panel.Width, 30), // Genişlik panel kadar
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };

                // Label'ları dikey olarak ortalamak için konum ayarla
                lblSayi.Location = new Point(0, 20);
                lblMetin.Location = new Point(0, 60);

                panel.Controls.Add(lblSayi);
                panel.Controls.Add(lblMetin);
                flowLayoutPanel1.Controls.Add(panel);
            }

            // Panelleri ekleyelim
            PanelEkle(Color.LightBlue, "24", "Tamamlanan");
            PanelEkle(Color.LightCoral, "12", "Devam Eden");
            PanelEkle(Color.MediumAquamarine, "3", "Geciken");
            PanelEkle(Color.Khaki, "39", "Toplam");
        }




        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerProfile officerProfile = new OfficerProfile();
            officerProfile.Show();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {

        }
    }
}
