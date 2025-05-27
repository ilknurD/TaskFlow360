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
using System.Windows.Forms.DataVisualization.Charting;

namespace TaskFlow360
{
    public partial class BossHomepage : Form
    {
        private string yoneticiId;
        public BossHomepage()
        {
            InitializeComponent();
            yoneticiId = KullaniciBilgi.KullaniciID;
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
            BossHomepage bossHomepage = new BossHomepage();
            bossHomepage.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            BossProfile bossProfile = new BossProfile();
            bossProfile.Show();
            this.Close();
        }

        private void btnKullaniciIslem_Click(object sender, EventArgs e)
        {

        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {

        }

        private void BossHomepage_Load(object sender, EventArgs e)
        {
            // Eğer yoneticiId boşsa, KullaniciBilgi'den tekrar almayı deneyelim
            if (string.IsNullOrEmpty(yoneticiId))
            {
                yoneticiId = KullaniciBilgi.KullaniciID;

                // Hala boşsa, kullanıcıyı login formuna yönlendirelim
                if (string.IsNullOrEmpty(yoneticiId))
                {
                    MessageBox.Show("Oturum bilgileriniz geçersiz. Lütfen tekrar giriş yapın.", "Oturum Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                    return;
                }
            }

            lblAdSoyad.Text = KullaniciBilgi.TamAd();
            lblHosgeldiniz.Text = $"Hoş Geldiniz, {KullaniciBilgi.TamAd()}";
            CalisanIstatistikleriniGoster();
            DepartmanCagriSayisiGoster(chartDepartman);
            SonEklenenKullanicilariYukle();
            IstatislikleriGoster();
        }
        private void CalisanIstatistikleriniGoster()
        {
            try
            {
                using (SqlConnection connection = Baglanti.BaglantiGetir())
                {
                    string sorgu = @"SELECT 
                            SUM(CASE WHEN Rol = 'Ekip Yöneticisi' AND KullaniciID <> @MudurID THEN 1 ELSE 0 END) AS EkipSayisi,
                            COUNT(*) AS ToplamCalisan
                            FROM Kullanici
                            WHERE KullaniciID <> @MudurID AND Rol <> 'Müdür'";

                    using (SqlCommand komut = new SqlCommand(sorgu, connection))
                    {
                        // Müdür'ün ID'sini parametre olarak ekleyin
                        komut.Parameters.AddWithValue("@MudurID", yoneticiId);

                        using (SqlDataReader reader = komut.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblCalisanBilgisi.Text =
                                    $"{reader.GetInt32(0)} Ekip ve toplamda {reader.GetInt32(1)} çalışanınız bulunmaktadır.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bilgiler alınırken hata oluştu: " + ex.Message);
            }
        }
        private void DepartmanCagriSayisiGoster(Chart chartControl)
        {
            try
            {
                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    string sorgu = @"SELECT d.DepartmanAdi, COUNT(*) AS CagriSayisi
                             FROM Cagri c
                             JOIN Kullanici k ON c.AtananKullaniciID = k.KullaniciID
                             JOIN Departman d ON k.DepartmanID = d.DepartmanID
                             GROUP BY d.DepartmanAdi";

                    using (SqlCommand komut = new SqlCommand(sorgu, conn))
                    using (SqlDataReader dr = komut.ExecuteReader())
                    {
                        // Grafik ayarları
                        chartControl.Series["Series1"].Points.Clear();
                        chartControl.Series["Series1"].ChartType = SeriesChartType.Doughnut;
                        chartControl.Titles.Clear();
                        chartControl.Titles.Add("Departmanlara Göre Çağrı Dağılımı");

                        chartControl.Series["Series1"].Label = "#PERCENT{P1}";
                        chartControl.Series["Series1"].IsValueShownAsLabel = true;

                        // Verileri grafik üzerine ekle
                        while (dr.Read())
                        {
                            string departmanAdi = dr["DepartmanAdi"].ToString();
                            int cagriSayisi = Convert.ToInt32(dr["CagriSayisi"]);

                            int pointIndex = chartControl.Series["Series1"].Points.AddXY(departmanAdi, cagriSayisi);
                            chartControl.Series["Series1"].Points[pointIndex].LegendText = $"{departmanAdi} ({cagriSayisi})";
                        }

                        // Legend (açıklama kutusu) açık
                        chartControl.Legends[0].Enabled = true;
                        chartControl.Legends[0].Docking = Docking.Bottom;
                        chartControl.Legends[0].Alignment = StringAlignment.Center;
                        chartControl.Legends[0].LegendStyle = LegendStyle.Row;
                        chartControl.Legends[0].Font = new Font("Century Gothic", 9, FontStyle.Bold);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Departman çağrı grafiği oluşturulurken hata oluştu: {ex.Message}",
                                "Hata",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
        private void SonEklenenKullanicilariYukle()
        {
            try
            {
                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    string sorgu = @"SELECT 
                    k.KullaniciID,
                    k.Ad + ' ' + k.Soyad AS AdSoyad,
                    k.Rol,
                    d.DepartmanAdi AS Departman,
                    k.Email,
                    k.Telefon,
                    k.IseBaslamaTar
                    FROM Kullanici k
                    JOIN Departman d ON k.DepartmanID = d.DepartmanID
                    WHERE k.Rol <> 'Müdür'
                    ORDER BY k.IseBaslamaTar DESC";

                    SqlDataAdapter da = new SqlDataAdapter(sorgu, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataGridViewColumn column in dataGridViewKullanicilar.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }

                    // DataGridView'i temizle ve hazırla
                    dataGridViewKullanicilar.DataSource = null;
                    dataGridViewKullanicilar.Columns.Clear();

                    // AutoGenerateColumns'u true yap (manuel sütun eklemeye gerek yok)
                    dataGridViewKullanicilar.AutoGenerateColumns = true;

                    // DataTable'ı DataGridView'e bağla
                    dataGridViewKullanicilar.DataSource = dt;

                    // Sütun başlıklarını düzenle (DataSource'dan sonra)
                    if (dataGridViewKullanicilar.Columns.Contains("KullaniciID"))
                        dataGridViewKullanicilar.Columns["KullaniciID"].HeaderText = "ID";

                    if (dataGridViewKullanicilar.Columns.Contains("AdSoyad"))
                        dataGridViewKullanicilar.Columns["AdSoyad"].HeaderText = "Ad Soyad";

                    if (dataGridViewKullanicilar.Columns.Contains("Rol"))
                        dataGridViewKullanicilar.Columns["Rol"].HeaderText = "Rol";

                    if (dataGridViewKullanicilar.Columns.Contains("Departman"))
                        dataGridViewKullanicilar.Columns["Departman"].HeaderText = "Departman";

                    if (dataGridViewKullanicilar.Columns.Contains("Email"))
                        dataGridViewKullanicilar.Columns["Email"].HeaderText = "E-posta";

                    if (dataGridViewKullanicilar.Columns.Contains("Telefon"))
                        dataGridViewKullanicilar.Columns["Telefon"].HeaderText = "Telefon";

                    if (dataGridViewKullanicilar.Columns.Contains("IseBaslamaTar"))
                    {
                        dataGridViewKullanicilar.Columns["IseBaslamaTar"].HeaderText = "İşe Başlama Tarihi";
                        dataGridViewKullanicilar.Columns["IseBaslamaTar"].DefaultCellStyle.Format = "dd.MM.yyyy";
                    }

                    // Sütun genişliklerini ayarla
                    dataGridViewKullanicilar.Columns["KullaniciID"].Width = 60;
                    dataGridViewKullanicilar.Columns["AdSoyad"].Width = 150;
                    dataGridViewKullanicilar.Columns["Rol"].Width = 100;
                    dataGridViewKullanicilar.Columns["Departman"].Width = 120;
                    dataGridViewKullanicilar.Columns["Email"].Width = 180;
                    dataGridViewKullanicilar.Columns["Telefon"].Width = 120;
                    dataGridViewKullanicilar.Columns["IseBaslamaTar"].Width = 120;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı listesi yüklenirken hata oluştu: " + ex.Message,
                                "Hata",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
        private void IstatislikleriGoster()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.AutoScroll = true;

            Size panelBoyut = new Size(280, 150);
            Padding panelMargin = new Padding(0, 0, 10, 0);

            int ekipYoneticisiSayisi = 0;
            int toplamCalisanSayisi = 0;
            int buAyAcilanCagriSayisi = 0;
            int cagriMerkeziSayisi = 0;

            Baglanti baglanti = new Baglanti();

            try
            {
                baglanti.BaglantiAc();

                string query = @"
            SELECT
                (SELECT COUNT(*) FROM Kullanici WHERE Rol = 'Ekip Yöneticisi') AS EkipYoneticisiSayisi,
                (SELECT COUNT(*) FROM Kullanici WHERE Rol <> 'Müdür') AS ToplamCalisanSayisi,
                (SELECT COUNT(*) FROM Cagri WHERE MONTH(OlusturmaTarihi) = MONTH(GETDATE()) AND YEAR(OlusturmaTarihi) = YEAR(GETDATE())) AS BuAyAcilanCagriSayisi,
                (SELECT COUNT(*) FROM Kullanici WHERE Rol = 'Çağrı Merkezi') AS CagriMerkeziSayisi
        ";

                SqlCommand cmd = new SqlCommand(query, baglanti.conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    ekipYoneticisiSayisi = reader["EkipYoneticisiSayisi"] != DBNull.Value ? Convert.ToInt32(reader["EkipYoneticisiSayisi"]) : 0;
                    toplamCalisanSayisi = reader["ToplamCalisanSayisi"] != DBNull.Value ? Convert.ToInt32(reader["ToplamCalisanSayisi"]) : 0;
                    buAyAcilanCagriSayisi = reader["BuAyAcilanCagriSayisi"] != DBNull.Value ? Convert.ToInt32(reader["BuAyAcilanCagriSayisi"]) : 0;
                    cagriMerkeziSayisi = reader["CagriMerkeziSayisi"] != DBNull.Value ? Convert.ToInt32(reader["CagriMerkeziSayisi"]) : 0;
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
                    Font = new Font("Century Gothic", 21, FontStyle.Bold),
                    AutoSize = false,
                    Size = new Size(panel.Width, 40),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Label lblMetin = new Label
                {
                    Text = metin,
                    Font = new Font("Century Gothic", 12),
                    AutoSize = false,
                    Size = new Size(panel.Width, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                lblSayi.Location = new Point(0, 40);
                lblMetin.Location = new Point(0, 80);

                panel.Controls.Add(lblSayi);
                panel.Controls.Add(lblMetin);
                flowLayoutPanel1.Controls.Add(panel);
            }

            // Panelleri oluştur
            PanelEkle(Color.FromArgb(129, 212, 250), ekipYoneticisiSayisi.ToString(), "Ekip Yöneticisi");
            PanelEkle(Color.FromArgb(206, 147, 216), cagriMerkeziSayisi.ToString(), "Çağrı Merkezi Çalışanı");
            PanelEkle(Color.FromArgb(174, 213, 129), toplamCalisanSayisi.ToString(), "Toplam Çalışan");
            PanelEkle(Color.FromArgb(255, 224, 178), buAyAcilanCagriSayisi.ToString(), "Bu Ay Açılan Çağrı");
        }

    }
}
