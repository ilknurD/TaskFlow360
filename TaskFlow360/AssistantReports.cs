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

namespace TaskFlow360
{
    public partial class AssistantReports : Form
    {
        public AssistantReports()
        {
            InitializeComponent();
            GrafikYukle();
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
            AssistantHomepage anasayfa = new AssistantHomepage();
            anasayfa.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            AsistantProfile asistantProfile = new AsistantProfile();
            asistantProfile.Show();
            this.Close();
        }

        private void btnCagriOlustur_Click(object sender, EventArgs e)
        {
            AssistantTaskCreationPage assistantTaskCreationPage = new AssistantTaskCreationPage();
            assistantTaskCreationPage.Show();
            this.Close();
        }

        private void btnCagriTakip_Click(object sender, EventArgs e)
        {
            AssistantTasks assistantTasks = new AssistantTasks();
            assistantTasks.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            AssistantReports assistantReports = new AssistantReports();
            assistantReports.Show();
            this.Close();
        }

        private void GrafikYukle()
        {
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
            string kullaniciID = KullaniciBilgi.KullaniciID;

            try
            {
                using (SqlConnection conn = Baglanti.BaglantiGetir()) // Bağlantı zaten açılıyor
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
                            chartDurum.Titles.Add("Çağrı Durum Dağılımı");
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
        }
    }
}