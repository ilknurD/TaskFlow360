using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
        Baglanti baglanti = new Baglanti();
        public OfficerHomepage()
        {
            InitializeComponent();
            TarihVeSaatGoster();
            IstatislikleriGoster();
            YeniVeDevamEdenGorevSayilariniGoster();
            ZamanliGuncelleme();
        }

        private void TarihVeSaatGoster()
        {
            DateTime suAn = DateTime.Now;
            string formatliTarihSaat = suAn.ToString("dd MMMM yyyy, dddd HH:mm");
            lblTarihSaat.Text = formatliTarihSaat;
        }

        private void OfficerHomepage_Load(object sender, EventArgs e)
        {
            List<Cagri> cagriListesi = CagrilariVeritabanindanGetir();
            CagrilariGoster(cagriListesi);
 
            string girisYapanID = KullaniciBilgi.KullaniciID;

            try
            {
                baglanti.BaglantiAc();

                string query = "SELECT Ad, Soyad FROM Kullanici WHERE KullaniciID = @ID";
                SqlCommand cmd = new SqlCommand(query, baglanti.conn);
                cmd.Parameters.AddWithValue("@ID", girisYapanID);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblAdSoyad.Text = $"{dr["Ad"]} {dr["Soyad"]}";
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private List<Cagri> CagrilariVeritabanindanGetir()
        {
            List<Cagri> cagriler = new List<Cagri>();
            Baglanti baglanti = new Baglanti();

            try
            {
                baglanti.BaglantiAc();

                string query = @"SELECT c.CagriID, c.CagriAciklama, c.Baslik,c.CagriKategori, c.Oncelik, 
                c.Durum, c.OlusturmaTarihi, c.TeslimTarihi, c.CevapTarihi,
                c.TalepEden AS TalepEden,
                ISNULL(k2.Ad + ' ' + k2.Soyad, 'Bilinmiyor') AS AtananKullanici,
                ISNULL(k3.Ad + ' ' + k3.Soyad, 'Bilinmiyor') AS OlusturanKullanici,
                c.HedefSure,
                c.AtananKullaniciID, c.OlusturanKullaniciID
         FROM Cagri c
         LEFT JOIN Kullanici k2 ON c.AtananKullaniciID = k2.KullaniciID
         LEFT JOIN Kullanici k3 ON c.OlusturanKullaniciID = k3.KullaniciID";


                SqlCommand cmd = new SqlCommand(query, baglanti.conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Cagri cagri = new Cagri()
                    {
                        CagriID = int.TryParse(reader["CagriID"]?.ToString(), out int cagriId) ? cagriId : 0,
                        Baslik = reader["Baslik"]?.ToString(),
                        CagriAciklama = reader["CagriAciklama"]?.ToString(),
                        CagriKategori = reader["CagriKategori"]?.ToString(),
                        Oncelik = reader["Oncelik"]?.ToString(),
                        Durum = reader["Durum"]?.ToString(),

                        OlusturmaTarihi = DateTime.TryParse(reader["OlusturmaTarihi"]?.ToString(), out DateTime ot) ? ot : DateTime.MinValue,
                        TeslimTarihi = DateTime.TryParse(reader["TeslimTarihi"]?.ToString(), out DateTime tt) ? tt : (DateTime?)null,
                        CevapTarihi = DateTime.TryParse(reader["CevapTarihi"]?.ToString(), out DateTime ct) ? ct : (DateTime?)null,

                        TalepEden = reader["TalepEden"]?.ToString(),

                        AtananKullaniciID = int.TryParse(reader["AtananKullaniciID"]?.ToString(), out int akid) ? akid : 0,
                        OlusturanKullaniciID = int.TryParse(reader["OlusturanKullaniciID"]?.ToString(), out int okid) ? okid : 0,
                        HedefSure = int.TryParse(reader["HedefSure"]?.ToString(), out int hs) ? hs : (int?)null
                    };

                    cagriler.Add(cagri);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Çağrıları çekerken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat(); 
            }

            return cagriler;
        }


        private void IstatislikleriGoster()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.AutoScroll = true;

            Size panelBoyut = new Size(177, 100);
            Padding panelMargin = new Padding(10, 0, 10, 0);

            int beklemede = 0;
            int tamamlandı = 0;
            int gecikti = 0;
            int iptalEdildi = 0;
            int atandi = 0;
            int toplam = 0;

            Baglanti baglanti = new Baglanti();

            try
            {
                baglanti.BaglantiAc();

                string query = @"SELECT
               SUM(CASE WHEN Durum = 'Beklemede' THEN 1 ELSE 0 END) AS Beklemede,
               SUM(CASE WHEN Durum = 'Tamamlandı' THEN 1 ELSE 0 END) AS Tamamlandı,
               SUM(CASE
               WHEN Durum = 'Gecikti' AND
                 GETDATE() > DATEADD(
                     HOUR,
                     TRY_CAST(LEFT(HedefSure, PATINDEX('%[^0-9]%', HedefSure + ' ') - 1) AS int),
                     OlusturmaTarihi
                 )
                 THEN 1 ELSE 0
                 END) AS Gecikti,
                 SUM(CASE WHEN Durum = 'İptal Edildi' THEN 1 ELSE 0 END) AS IptalEdildi,  --Düzenlendi
                 SUM(CASE WHEN Durum = 'Atandı' THEN 1 ELSE 0 END) AS Atandi,  --Düzenlendi
                 COUNT(*) AS Toplam
                FROM Cagri";


                SqlCommand cmd = new SqlCommand(query, baglanti.conn);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    beklemede = reader["Beklemede"] != DBNull.Value ? Convert.ToInt32(reader["Beklemede"]) : 0;
                    tamamlandı = reader["Tamamlandı"] != DBNull.Value ? Convert.ToInt32(reader["Tamamlandı"]) : 0;
                    gecikti = reader["Gecikti"] != DBNull.Value ? Convert.ToInt32(reader["Gecikti"]) : 0;
                    iptalEdildi = reader["IptalEdildi"] != DBNull.Value ? Convert.ToInt32(reader["IptalEdildi"]) : 0; // Değiştirildi
                    atandi = reader["Atandi"] != DBNull.Value ? Convert.ToInt32(reader["Atandi"]) : 0; // Değiştirildi
                    toplam = reader["Toplam"] != DBNull.Value ? Convert.ToInt32(reader["Toplam"]) : 0;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikleri çekerken hata oluştu: " + ex.Message);
                Clipboard.SetText(ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }

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
                    Size = new Size(panel.Width, 30),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };

                Label lblMetin = new Label
                {
                    Text = metin,
                    Font = new Font("Arial", 10),
                    AutoSize = false,
                    Size = new Size(panel.Width, 30),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };

                lblSayi.Location = new Point(0, 20);
                lblMetin.Location = new Point(0, 60);

                panel.Controls.Add(lblSayi);
                panel.Controls.Add(lblMetin);
                flowLayoutPanel1.Controls.Add(panel);
            }


            PanelEkle(Color.FromArgb(187, 222, 251), beklemede.ToString(), "Beklemede"); // Açık Mavi
            PanelEkle(Color.FromArgb(200, 230, 201), tamamlandı.ToString(), "Tamamlandı"); // Açık Yeşil
            PanelEkle(Color.FromArgb(255, 204, 188), gecikti.ToString(), "Gecikti"); // Açık Turuncu
            PanelEkle(Color.FromArgb(144, 164, 174), iptalEdildi.ToString(), "İptal Edildi"); // Açık Gri
            PanelEkle(Color.FromArgb(255, 249, 196), atandi.ToString(), "Atandı"); // Açık Sarı
            PanelEkle(Color.FromArgb(209, 196, 233), toplam.ToString(), "Toplam"); // Lavanta
        }

        private void YeniVeDevamEdenGorevSayilariniGoster()
        {
            Baglanti baglanti = new Baglanti();
            int yeniGorevSayisi = 0;
            int devamEdenGorevSayisi = 0;

            try
            {
                baglanti.BaglantiAc();

                string yeniGorevQuery = @"SELECT COUNT(*) FROM Cagri 
                                 WHERE Durum IN ('Atandı')";

                SqlCommand yeniCmd = new SqlCommand(yeniGorevQuery, baglanti.conn);
                yeniGorevSayisi = (int)yeniCmd.ExecuteScalar();

                // Devam eden görevleri say (Beklemede ve Atandı durumundakiler)
                string devamEdenQuery = @"SELECT COUNT(*) FROM Cagri 
                                 WHERE Durum IN ('Beklemede')";

                SqlCommand devamEdenCmd = new SqlCommand(devamEdenQuery, baglanti.conn);
                devamEdenGorevSayisi = (int)devamEdenCmd.ExecuteScalar();

                // Label'ları güncelle
                lblYeniGorevSayisi.Text = yeniGorevSayisi.ToString();
                lblDevamEdenGorevSayisi.Text = devamEdenGorevSayisi.ToString();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Görev sayılarını çekerken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void ZamanliGuncelleme()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 60000; // 1 dakika
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TarihVeSaatGoster();
            IstatislikleriGoster();
            YeniVeDevamEdenGorevSayilariniGoster();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            OfficerProfile officerProfile = new OfficerProfile();
            officerProfile.Show();
            this.Close();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
        }

        private void CagrilariGoster(List<Cagri> CagriListesi)
        {
            PnlGorevler.Controls.Clear();
            PnlGorevler.FlowDirection = FlowDirection.TopDown;
            PnlGorevler.WrapContents = false;
            PnlGorevler.AutoScroll = true;

            var sonUcGorev = CagriListesi
        .OrderByDescending(g => g.OlusturmaTarihi) 
        .Take(3)
        .ToList();

            foreach (var gorev in sonUcGorev)
            {
                Panel cagriPaneli = new Panel();
                int panelYuksekligi = 120;
                int panelGenisligi = PnlGorevler.Width - 20;
                cagriPaneli.Size = new Size(panelGenisligi, panelYuksekligi);
                cagriPaneli.BorderStyle = BorderStyle.FixedSingle;
                cagriPaneli.BackColor = Color.White;

                cagriPaneli.Cursor = Cursors.Hand;
                cagriPaneli.Click += (s, e) =>
                {
                    OfficerTaskspage officerTaskspage = new OfficerTaskspage();
                    officerTaskspage.Show();
                    this.Hide();
                };

                Label lblGorevAdi = new Label();
                lblGorevAdi.Text = gorev.Baslik;
                lblGorevAdi.Location = new Point(10, 10);
                lblGorevAdi.Font = new Font("Century Gothic", 12, FontStyle.Bold);
                lblGorevAdi.AutoSize = true;

                Label lblTalepEden = new Label();
                lblTalepEden.Text = "Talep Eden: " + gorev.TalepEden;
                lblTalepEden.Location = new Point(10, 30);
                lblTalepEden.Font = new Font("Century Gothic", 10);
                lblTalepEden.AutoSize = true;

                Label lblOlusturmaTarihi = new Label();
                lblOlusturmaTarihi.Text = "Oluşturma Tarihi: " + gorev.OlusturmaTarihi.ToString();
                lblOlusturmaTarihi.Location = new Point(10, 50);
                lblOlusturmaTarihi.Font = new Font("Century Gothic", 9);
                lblOlusturmaTarihi.AutoSize = true;

                Label lblOncelik = new Label();
                lblOncelik.Text = gorev.Oncelik;
                lblOncelik.Location = new Point(cagriPaneli.Width - 80, 10);
                lblOncelik.Font = new Font("Century Gothic", 10, FontStyle.Bold);
                lblOncelik.AutoSize = true;

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

                foreach (var ctrl in new Control[] { lblGorevAdi, lblTalepEden, lblOlusturmaTarihi, lblOncelik })
                {
                    ctrl.Click += (s, e) =>
                    {
                        OfficerTaskspage officerTaskspage = new OfficerTaskspage();
                        officerTaskspage.Show();
                        this.Hide();
                    };
                }

                cagriPaneli.Controls.Add(lblGorevAdi);
                cagriPaneli.Controls.Add(lblTalepEden);
                cagriPaneli.Controls.Add(lblOlusturmaTarihi);
                cagriPaneli.Controls.Add(lblOncelik);

                PnlGorevler.Controls.Add(cagriPaneli);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}