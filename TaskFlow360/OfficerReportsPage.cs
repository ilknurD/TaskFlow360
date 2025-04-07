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
    public partial class OfficerReportsPage : Form
    {
        Baglanti baglanti = new Baglanti();
        public OfficerReportsPage()
        {
            InitializeComponent();
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

        private void OfficerReportsPage_Load(object sender, EventArgs e)
        {
            PieChartGunlukDurum();
            ColumnChartHaftalikPerformans();
            AylikPerformansGrafik();
        }

        private void PieChartGunlukDurum()
        {
            chartGunlukDurum.Series.Clear();
            chartGunlukDurum.Titles.Clear();
            chartGunlukDurum.Titles.Add("Bugünkü Çağrı Dağılımı");

            Series durumSeries = new Series
            {
                ChartType = SeriesChartType.Pie
            };

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                string query = @"
                SELECT Durum, COUNT(*) AS Adet
                FROM Cagri
                WHERE AtananKullaniciID = @KullaniciID AND 
                CAST(OlusturmaTarihi AS DATE) = CAST(GETDATE() AS DATE)
                GROUP BY Durum";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@KullaniciID", KullaniciBilgi.KullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    durumSeries.Points.AddXY(dr["Durum"].ToString(), dr["Adet"]);
                }
                dr.Close();
            }

            chartGunlukDurum.Series.Add(durumSeries);
        }
        private void ColumnChartHaftalikPerformans()
        {
            chartHaftalik.Series.Clear();
            chartHaftalik.Titles.Clear();
            chartHaftalik.Titles.Add("Son 7 Günlük Performans");

            Series cagriSeries = new Series("Toplam Çağrı")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.CornflowerBlue
            };

            Series sureSeries = new Series("Ortalama Süre (dk)")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                Color = Color.OrangeRed
            };

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                string query = @"
            SELECT FORMAT(RaporTarihi, 'dd.MM') AS Tarih,
                   SUM(ToplamCagriSayisi) AS CagriSayisi,
                   AVG(OrtalamaCozumSuresi) AS OrtalamaSure
            FROM PerformansRaporu
            WHERE KullaniciID = @KullaniciID AND 
                  RaporTarihi >= DATEADD(DAY, -6, GETDATE())
            GROUP BY FORMAT(RaporTarihi, 'dd.MM')
            ORDER BY Tarih";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@KullaniciID", KullaniciBilgi.KullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string tarih = dr["Tarih"].ToString();
                    cagriSeries.Points.AddXY(tarih, dr["CagriSayisi"]);
                    sureSeries.Points.AddXY(tarih, dr["OrtalamaSure"]);
                }
                dr.Close();
            }

            chartHaftalik.Series.Add(cagriSeries);
            chartHaftalik.Series.Add(sureSeries);
        }

        private void AylikPerformansGrafik()
        {
            chartAylik.Series.Clear();
            chartAylik.Titles.Clear();
            chartAylik.Titles.Add("Son 3 Ay Performansı");

            // 1. Sütun Grafik: Çağrı Sayısı
            Series cagriSeries = new Series("Toplam Çağrı")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.MediumSeaGreen,
                YAxisType = AxisType.Primary
            };

            // 2. Çizgi Grafik: Prim
            Series primSeries = new Series("Toplam Prim")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                Color = Color.OrangeRed,
                YAxisType = AxisType.Secondary
            };

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                string query = @"
                   SELECT FORMAT(RaporTarihi, 'yyyy-MM') AS Ay,
                   SUM(ToplamCagriSayisi) AS ToplamCagri,
                   SUM(Prim) AS ToplamPrim
                   FROM PerformansRaporu
                   WHERE KullaniciID = @KullaniciID AND
                   RaporTarihi >= DATEADD(MONTH, -2, GETDATE())
                   GROUP BY FORMAT(RaporTarihi, 'yyyy-MM')
                   ORDER BY Ay";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@KullaniciID", KullaniciBilgi.KullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string ay = dr["Ay"].ToString();
                    cagriSeries.Points.AddXY(ay, dr["ToplamCagri"]);
                    primSeries.Points.AddXY(ay, dr["ToplamPrim"]);
                }
                dr.Close();
            }

            // Serileri ekle
            chartAylik.Series.Add(cagriSeries);
            chartAylik.Series.Add(primSeries);

            // Eksen başlıkları
            chartAylik.ChartAreas[0].AxisX.Title = "Ay";
            chartAylik.ChartAreas[0].AxisY.Title = "Çağrı Sayısı";
            chartAylik.ChartAreas[0].AxisY2.Title = "Prim (₺)";
            chartAylik.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            cagriSeries.IsValueShownAsLabel = true;
            primSeries.IsValueShownAsLabel = true;
        }



    }
}
