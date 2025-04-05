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
            List<Cagri> cagriListesi = CagrilariVeritabanindanGetir();
            CagrileriGoster(cagriListesi);
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

                string query = @"SELECT c.CagriID, c.Baslik, c.CagriAciklama, c.CagriKategori, c.Oncelik, 
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

                reader.Close(); // 🔄 Reader kapatılıyor
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

            Size panelBoyut = new Size(200, 100);
            Padding panelMargin = new Padding(15, 0, 15, 0);

            int acik = 0;
            int geciken = 0;
            int cozuldu = 0;
            int toplam = 0;

            Baglanti baglanti = new Baglanti();

            try
            {
                baglanti.BaglantiAc();

                string query = @"SELECT 
                 SUM(CASE WHEN Durum = 'Açık' THEN 1 ELSE 0 END) AS Açık,
                 SUM(CASE WHEN Durum = 'Çözüldü' THEN 1 ELSE 0 END) AS Çözüldü,
                 SUM(CASE 
                     WHEN Durum = 'Geciken' AND 
                          GETDATE() > DATEADD(
                              HOUR, 
                              TRY_CAST(LEFT(HedefSure, PATINDEX('%[^0-9]%', HedefSure + ' ') - 1) AS int), 
                              OlusturmaTarihi
                          ) 
                     THEN 1 ELSE 0 
                     END) AS Geciken,
                 COUNT(*) AS Toplam
                 FROM Cagri";

                SqlCommand cmd = new SqlCommand(query, baglanti.conn);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    acik = reader["Açık"] != DBNull.Value ? Convert.ToInt32(reader["Açık"]) : 0;
                    geciken = reader["Geciken"] != DBNull.Value ? Convert.ToInt32(reader["Geciken"]) : 0;
                    cozuldu = reader["Çözüldü"] != DBNull.Value ? Convert.ToInt32(reader["Çözüldü"]) : 0;
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

            // Panel oluşturma işlemi
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

            // Dinamik verilerle panelleri ekle
            PanelEkle(Color.LightBlue, acik.ToString(), "Açık");
            PanelEkle(Color.LightCoral, geciken.ToString(), "Geciken");
            PanelEkle(Color.MediumAquamarine, cozuldu.ToString(), "Çözüldü");
            PanelEkle(Color.Khaki, toplam.ToString(), "Toplam");
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

        private void CagrileriGoster(List<Cagri> CagriListesi)
        {
            PnlGorevler.Controls.Clear();
            PnlGorevler.FlowDirection = FlowDirection.TopDown; // Dikey
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

                // Tüm label'lara da tıklama olayı ekle (Panel dışı tıklamalarda da çalışması için)
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

    }
}