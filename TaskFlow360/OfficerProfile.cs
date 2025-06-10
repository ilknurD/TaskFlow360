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

namespace TaskFlow360
{
    public partial class OfficerProfile : Form
    {
        Connection baglanti = new Connection();
        private readonly Logger _logger;

        public OfficerProfile()
        {
            InitializeComponent();
            _logger = new Logger();
            _logger.LogEkle("Giriş", "OfficerProfile", "Memur profil sayfası açıldı");
            IstatistikleriGoster();
        }

        private void IstatistikleriGoster()
        {
            try
            {
                baglanti.BaglantiAc();
                string kullaniciID = UserInformation.KullaniciID;
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;

                string toplamGorevQuery = @"SELECT COUNT(*) FROM Cagri WHERE AtananKullaniciID = @KullaniciID";
                SqlCommand toplamCmd = new SqlCommand(toplamGorevQuery, baglanti.conn);
                toplamCmd.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                int toplamGorev = (int)toplamCmd.ExecuteScalar();

                string tamamlananQuery = @"SELECT COUNT(*) FROM Cagri 
                                         WHERE AtananKullaniciID = @KullaniciID AND Durum = 'Tamamlandı'";
                SqlCommand tamamlananCmd = new SqlCommand(tamamlananQuery, baglanti.conn);
                tamamlananCmd.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                int tamamlananGorev = (int)tamamlananCmd.ExecuteScalar();

                string primQuery = @"SELECT TOP 1 PrimToplam
                    FROM PrimKayit 
                    WHERE KullaniciID = @KullaniciID 
                    AND Yil = @Yil 
                    AND Ay = @Ay
                    ORDER BY HesaplamaTarihi DESC";

                SqlCommand primCmd = new SqlCommand(primQuery, baglanti.conn);
                primCmd.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                primCmd.Parameters.AddWithValue("@Yil", currentYear);
                primCmd.Parameters.AddWithValue("@Ay", currentMonth);

                object primResult = primCmd.ExecuteScalar();
                decimal primTutari = primResult != null ? Convert.ToDecimal(primResult) : 0;
                lblPrimTutari.Text = primTutari.ToString("N2") + " ₺";

                string ortalamaSureQuery = @"SELECT AVG(DATEDIFF(HOUR, OlusturmaTarihi, 
                    CASE WHEN Durum = 'Tamamlandı' THEN TeslimTarihi ELSE GETDATE() END)) as OrtalamaSure
                    FROM Cagri 
                    WHERE AtananKullaniciID = @KullaniciID 
                    AND Durum IN ('Tamamlandı', 'Beklemede', 'Atandı')";

                SqlCommand ortalamaSureCmd = new SqlCommand(ortalamaSureQuery, baglanti.conn);
                ortalamaSureCmd.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                object ortalamaSureResult = ortalamaSureCmd.ExecuteScalar();
                double ortalamaSure = ortalamaSureResult != DBNull.Value ? Convert.ToDouble(ortalamaSureResult) : 0;
                lblOrtalamaSure.Text = $"{ortalamaSure:F1} saat";

                lblToplamGorev.Text = toplamGorev.ToString();
                lblTamamlananGorev.Text = tamamlananGorev.ToString();

                double performansYuzdesi = toplamGorev > 0 ? (double)tamamlananGorev / toplamGorev * 100 : 0;
                lblPerformans.Text = $"%{performansYuzdesi:F1}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikleri çekerken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Çıkış", "OfficerProfile", "Memur profil sayfasından çıkış yapıldı");
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Yönlendirme", "OfficerProfile", "Ana sayfaya yönlendirildi");
            OfficerHomepage officerHomepage = new OfficerHomepage();
            officerHomepage.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Yönlendirme", "OfficerProfile", "Görevler sayfasına yönlendirildi");
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
            this.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Çıkış", "OfficerProfile", "Sistemden çıkış yapıldı");
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Yönlendirme", "OfficerProfile", "Profil sayfası yenilendi");
            OfficerProfile profile = new OfficerProfile();
            profile.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Yönlendirme", "OfficerProfile", "Raporlar sayfasına yönlendirildi");
            OfficerReportsPage officerReportsPage = new OfficerReportsPage();
            officerReportsPage.Show();
            this.Close();
        }

        private void OfficerProfile_Load(object sender, EventArgs e)
        {
            string kullaniciID = UserInformation.KullaniciID;

            try
            {
                baglanti.BaglantiAc();

                string query = @"SELECT 
                K.Ad, K.Soyad, K.Email, K.Telefon, K.Adres, 
                K.DogumTar, K.IseBaslamaTar, 
                D.DepartmanAdi, B.BolumAdi,
                YK.Ad AS YoneticiAd, YK.Soyad AS YoneticiSoyad,
                K.Cinsiyet
                FROM Kullanici K
                LEFT JOIN Departman D ON K.DepartmanID = D.DepartmanID
                LEFT JOIN Bolum B ON K.BolumID = B.BolumID
                LEFT JOIN Kullanici YK ON K.YoneticiID = YK.KullaniciID
                WHERE K.KullaniciID = @KullaniciID";

                SqlCommand cmd = new SqlCommand(query, baglanti.conn);
                cmd.Parameters.AddWithValue("@KullaniciID", kullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblAdSoyad.Text = $"{dr["Ad"]} {dr["Soyad"]}";
                    lblEmail.Text = dr["Email"].ToString();
                    lblTelefon.Text = dr["Telefon"].ToString();
                    lblAdres.Text = dr["Adres"].ToString();
                    lblDogumTarihi.Text = Convert.ToDateTime(dr["DogumTar"]).ToShortDateString();
                    lblIseBaslama.Text = Convert.ToDateTime(dr["IseBaslamaTar"]).ToShortDateString();
                    lblDepartman.Text = dr["DepartmanAdi"].ToString();
                    lblBolum.Text = dr["BolumAdi"].ToString() + " " + "(Rol)";

                    string yoneticiAdSoyad = dr["YoneticiAd"] != DBNull.Value ? $"{dr["YoneticiAd"]} {dr["YoneticiSoyad"]}" : "Yönetici yok";
                    lblYonetici.Text = yoneticiAdSoyad;

                    string cinsiyet = dr["Cinsiyet"].ToString();

                    if (cinsiyet == "Kadın")
                        pctrProfil.Image = Properties.Resources.kadin;
                    else if (cinsiyet == "Erkek")
                        pctrProfil.Image = Properties.Resources.erkek;

                }

                dr.Close();
                lblKullaniciID.Text = kullaniciID;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı bilgileri çekilirken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }
    }
}
