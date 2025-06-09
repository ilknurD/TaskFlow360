using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net;

namespace TaskFlow360
{
    public partial class CallerReports : Form
    {
        public CallerReports()
        {
            InitializeComponent();
            LogEkle("CallerReports formu başlatıldı", "Form", "CallerReports");
            GrafikYukle();
        }

        private void LogEkle(string islemDetaylari, string islemTipi, string tabloAdi)
        {
            try
            {
                using (SqlConnection conn = Connection.BaglantiGetir())
                {
                    string sorgu = @"INSERT INTO Log (IslemTarihi, KullaniciID, IslemTipi, TabloAdi, IslemDetaylari, IPAdresi) 
                                    VALUES (@IslemTarihi, @KullaniciID, @IslemTipi, @TabloAdi, @IslemDetaylari, @IPAdresi)";

                    using (SqlCommand cmd = new SqlCommand(sorgu, conn))
                    {
                        cmd.Parameters.AddWithValue("@IslemTarihi", DateTime.Now);
                        cmd.Parameters.AddWithValue("@KullaniciID", UserInformation.KullaniciID);
                        cmd.Parameters.AddWithValue("@IslemTipi", islemTipi);
                        cmd.Parameters.AddWithValue("@TabloAdi", tabloAdi);
                        cmd.Parameters.AddWithValue("@IslemDetaylari", islemDetaylari);
                        cmd.Parameters.AddWithValue("@IPAdresi", GetLocalIPAddress());
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Log kayıt hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "IP Adresi Bulunamadı";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            LogEkle("Kapat butonuna tıklandı", "Buton", "CallerReports");
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            LogEkle("Küçült butonuna tıklandı", "Buton", "CallerReports");
            WindowState = FormWindowState.Minimized;
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            LogEkle("Anasayfa butonuna tıklandı", "Buton", "CallerReports");
            CallerHomepage anasayfa = new CallerHomepage();
            anasayfa.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            LogEkle("Profil butonuna tıklandı", "Buton", "CallerReports");
            CallerProfile asistantProfile = new CallerProfile();
            asistantProfile.Show();
            this.Close();
        }

        private void btnCagriOlustur_Click(object sender, EventArgs e)
        {
            LogEkle("Çağrı Oluştur butonuna tıklandı", "Buton", "CallerReports");
            CallerTaskCreationPage assistantTaskCreationPage = new CallerTaskCreationPage();
            assistantTaskCreationPage.Show();
            this.Close();
        }

        private void btnCagriTakip_Click(object sender, EventArgs e)
        {
            LogEkle("Çağrı Takip butonuna tıklandı", "Buton", "CallerReports");
            CallerTasks assistantTasks = new CallerTasks();
            assistantTasks.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            LogEkle("Raporlar butonuna tıklandı", "Buton", "CallerReports");
            CallerReports assistantReports = new CallerReports();
            assistantReports.Show();
            this.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            LogEkle("Çıkış butonuna tıklandı", "Buton", "CallerReports");
            UserInformation.BilgileriTemizle();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void GrafikYukle()
        {
            LogEkle("Grafik yükleme işlemi başladı", "Okuma", "CallerReports");
            // Grafik serilerini kontrol et ve yoksa oluştur
            if (chartDepartman.Series.IndexOf("Series1") == -1)
            {
                chartDepartman.Series.Add("Series1");
            }
            if (chartDurum.Series.IndexOf("Series1") == -1)
            {
                chartDurum.Series.Add("Series1");
            }
            if (chartAylik.Series.IndexOf("Series1") == -1)
            {
                chartAylik.Series.Add("Series1");
            }
            if (chartBolum.Series.IndexOf("Series1") == -1)
            {
                chartBolum.Series.Add("Series1");
            }
            string kullaniciID = UserInformation.KullaniciID;

            try
            {
                using (SqlConnection conn = Connection.BaglantiGetir())
                {
                    // 1. Departmanlara Göre Çağrı Sayısı
                    using (SqlCommand komut1 = new SqlCommand(
                        @"SELECT d.DepartmanAdi, COUNT(*) AS CagriSayisi
                  FROM Cagri c
                  JOIN Kullanici k ON c.AtananKullaniciID = k.KullaniciID
                  JOIN Departman d ON k.DepartmanID = d.DepartmanID
                  GROUP BY d.DepartmanAdi", conn))
                    using (SqlDataReader dr1 = komut1.ExecuteReader())
                    {
                        chartDepartman.Series["Series1"].Points.Clear();
                        chartDepartman.Series["Series1"].ChartType = SeriesChartType.Pie;
                        chartDepartman.Titles.Add("Departmanlara Göre Çağrı Sayısı");
                        while (dr1.Read())
                        {
                            chartDepartman.Series["Series1"].Points.AddXY(
                                dr1["DepartmanAdi"].ToString(),
                                Convert.ToInt32(dr1["CagriSayisi"]));
                        }
                    }

                    // 2. Kullanıcının Çağrı Durum Dağılımı
                    using (SqlCommand komut2 = new SqlCommand(
                        @"SELECT Durum, COUNT(*) AS Sayisi 
                  FROM Cagri 
                  WHERE OlusturanKullaniciID = @kullaniciID 
                  GROUP BY Durum", conn))
                    {
                        komut2.Parameters.AddWithValue("@kullaniciID", kullaniciID);
                        using (SqlDataReader dr2 = komut2.ExecuteReader())
                        {
                            chartDurum.Series["Series1"].Points.Clear();
                            chartDurum.Series["Series1"].ChartType = SeriesChartType.Doughnut;
                            chartDurum.Titles.Add("Kişisel Çağrı Durum Dağılımı");
                            while (dr2.Read())
                            {
                                chartDurum.Series["Series1"].Points.AddXY(
                                    dr2["Durum"].ToString(),
                                    Convert.ToInt32(dr2["Sayisi"]));
                            }
                        }
                    }

                    // 3. Aylara Göre Çağrı Sayısı
                    using (SqlCommand komut3 = new SqlCommand(
                        @"SELECT FORMAT(OlusturmaTarihi, 'yyyy-MM') AS Ay, COUNT(*) AS Sayisi 
                  FROM Cagri 
                  WHERE OlusturmaTarihi >= DATEADD(MONTH, -6, GETDATE()) 
                  GROUP BY FORMAT(OlusturmaTarihi, 'yyyy-MM') 
                  ORDER BY Ay", conn))
                    using (SqlDataReader dr3 = komut3.ExecuteReader())
                    {
                        // Grafik serisini kontrol et ve yoksa oluştur
                        if (chartAylik.Series.IndexOf("Series1") == -1)
                        {
                            chartAylik.Series.Add("Series1");
                        }

                        chartAylik.Series["Series1"].Points.Clear();
                        chartAylik.Series["Series1"].ChartType = SeriesChartType.Column;
                        chartAylik.Titles.Clear(); // Önceki başlıkları temizle
                        chartAylik.Titles.Add("Son 6 Aylık Çağrı Dağılımı");

                        while (dr3.Read())
                        {
                            chartAylik.Series["Series1"].Points.AddXY(
                                dr3["Ay"].ToString(),
                                Convert.ToInt32(dr3["Sayisi"]));
                        }
                    }
                    // 4. Bölümlere Göre Açılan Çağrı Sayısı
                    using (SqlCommand komut4 = new SqlCommand(
                        @"SELECT B.BolumAdi, COUNT(C.CagriID) AS CagriSayisi 
                  FROM Cagri C
                  JOIN Kullanici K ON C.AtananKullaniciID = K.KullaniciID
                  JOIN Bolum B ON K.BolumID = B.BolumID
                  GROUP BY B.BolumAdi", conn))
                    using (SqlDataReader dr4 = komut4.ExecuteReader())
                    {
                        // Grafik serisini kontrol et ve yoksa oluştur
                        if (chartBolum.Series.IndexOf("Series1") == -1)
                        {
                            chartBolum.Series.Add("Series1");
                        }

                        chartBolum.Series["Series1"].Points.Clear();
                        chartBolum.Series["Series1"].ChartType = SeriesChartType.Bar;
                        chartBolum.Titles.Clear(); // Önceki başlıkları temizle
                        chartBolum.Titles.Add("Bölümlere Göre Çağrı Sayısı");

                        while (dr4.Read())
                        {
                            chartBolum.Series["Series1"].Points.AddXY(
                                dr4["BolumAdi"].ToString(),
                                Convert.ToInt32(dr4["CagriSayisi"]));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Genel hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LogEkle("Grafik yükleme işlemi tamamlandı", "Okuma", "CallerReports");
        }
    }
}