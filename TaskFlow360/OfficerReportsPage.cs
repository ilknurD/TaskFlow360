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
using System.Net;

namespace TaskFlow360
{
    public partial class OfficerReportsPage : Form
    {
        Connection baglanti = new Connection();
        private readonly Logger _logger = new Logger();

        // Ana tema renkleri
        private readonly Color PrimaryColor = Color.FromArgb(126, 87, 194);
        private readonly Color SecondaryColor = Color.FromArgb(158, 129, 214);
        private readonly Color AccentColor = Color.FromArgb(94, 65, 162);
        private readonly Color LightAccent = Color.FromArgb(190, 174, 230);
        private readonly Color TextColor = Color.FromArgb(51, 51, 51);

        public OfficerReportsPage()
        {
            InitializeComponent();
            _logger.LogEkle("OfficerReportsPage formu başlatıldı", "Form", "OfficerReportsPage");
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Kapat butonuna tıklandı", "Buton", "OfficerReportsPage");
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Küçült butonuna tıklandı", "Buton", "OfficerReportsPage");
            WindowState = FormWindowState.Minimized;
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Anasayfa butonuna tıklandı", "Buton", "OfficerReportsPage");
            OfficerHomepage officerHomepage = new OfficerHomepage();
            officerHomepage.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Profil butonuna tıklandı", "Buton", "OfficerReportsPage");
            OfficerProfile officerProfile = new OfficerProfile();
            officerProfile.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Görevler butonuna tıklandı", "Buton", "OfficerReportsPage");
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Raporlar butonuna tıklandı", "Buton", "OfficerReportsPage");
            OfficerReportsPage officerReportsPage = new OfficerReportsPage();
            officerReportsPage.Show();
            this.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Çıkış butonuna tıklandı", "Buton", "OfficerReportsPage");
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void OfficerReportsPage_Load(object sender, EventArgs e)
        {
            _logger.LogEkle("Raporlar yüklenmeye başlandı", "Okuma", "OfficerReportsPage");
            PieChartGunlukDurum();
            ColumnChartHaftalikPerformans();
            AylikPerformansGrafik();
            OncelikDagilimiGrafik();
            _logger.LogEkle("Raporlar başarıyla yüklendi", "Okuma", "OfficerReportsPage");
        }

        private void BeautifyChart(Chart chart)
        {
            chart.ChartAreas[0].BackColor = Color.White;
            chart.BackColor = Color.White;
            chart.BorderlineColor = Color.FromArgb(230, 230, 230);
            chart.BorderlineWidth = 1;
            chart.BorderlineDashStyle = ChartDashStyle.Solid;

            if (chart.Legends.Count > 0)
            {
                chart.Legends[0].BackColor = Color.Transparent;
                chart.Legends[0].ForeColor = TextColor;
                chart.Legends[0].Font = new Font("Century Gothic", 9F);
                chart.Legends[0].BorderColor = Color.Transparent;
            }

            chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = TextColor;
            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = TextColor;
            chart.ChartAreas[0].AxisX.LineColor = Color.FromArgb(200, 200, 200);
            chart.ChartAreas[0].AxisY.LineColor = Color.FromArgb(200, 200, 200);
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Century Gothic", 8F);
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Century Gothic", 8F);

            foreach (Title title in chart.Titles)
            {
                title.ForeColor = TextColor;
                title.Font = new Font("Century Gothic", 11F, FontStyle.Bold);
            }
        }

        private void PieChartGunlukDurum()
        {
            chartGunlukDurum.Series.Clear();
            chartGunlukDurum.Titles.Clear();
            BeautifyChart(chartGunlukDurum);
            chartGunlukDurum.Titles.Add("Bugünkü Çağrı Dağılımı");

            Series durumSeries = new Series
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                LabelForeColor = Color.White,
                Font = new Font("Century Gothic", 9F, FontStyle.Bold)
            };

            Color[] pieColors = {
                PrimaryColor,           // Ana mor
                SecondaryColor,         // Açık mor
                AccentColor,            // Koyu mor
                LightAccent,            // Çok açık mor
                Color.FromArgb(106, 67, 174),  // Orta mor
                Color.FromArgb(146, 107, 204)  // Yumuşak mor
            };

            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                string query = @"
            SELECT Durum, COUNT(*) AS Adet
            FROM Cagri
            WHERE AtananKullaniciID = @KullaniciID AND 
                  CAST(OlusturmaTarihi AS DATE) = CAST(GETDATE() AS DATE)
            GROUP BY Durum";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@KullaniciID", UserInformation.KullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();
                int colorIndex = 0;
                while (dr.Read())
                {
                    var point = durumSeries.Points.AddXY(dr["Durum"].ToString(), dr["Adet"]);
                    durumSeries.Points[point].Color = pieColors[colorIndex % pieColors.Length];
                    colorIndex++;
                }
                dr.Close();
            }

            chartGunlukDurum.Series.Add(durumSeries);
        }

        private void ColumnChartHaftalikPerformans()
        {
            chartHaftalik.Series.Clear();
            chartHaftalik.Titles.Clear();
            BeautifyChart(chartHaftalik);
            chartHaftalik.Titles.Add("Son 7 Günlük Performans");

            Series cagriSeries = new Series("Toplam Çağrı")
            {
                ChartType = SeriesChartType.Column,
                Color = PrimaryColor,
                BorderWidth = 0,
                IsValueShownAsLabel = true,
                LabelForeColor = TextColor,
                Font = new Font("Century Gothic", 8F)
            };

            Series sureSeries = new Series("Ortalama Süre (dk)")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                Color = AccentColor,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6,
                MarkerColor = AccentColor,
                IsValueShownAsLabel = true,
                LabelForeColor = TextColor,
                Font = new Font("Century Gothic", 8F)
            };

            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                string query = @"
                SELECT FORMAT(OlusturmaTarihi, 'dd.MM') AS Tarih,
                       COUNT(*) AS CagriSayisi,
                       AVG(DATEDIFF(MINUTE, OlusturmaTarihi, TeslimTarihi)) AS OrtalamaSure
                FROM Cagri
                WHERE AtananKullaniciID = @KullaniciID AND
                      OlusturmaTarihi >= DATEADD(DAY, -6, GETDATE()) AND
                      TeslimTarihi IS NOT NULL
                GROUP BY FORMAT(OlusturmaTarihi, 'dd.MM')
                ORDER BY Tarih";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@KullaniciID", UserInformation.KullaniciID);

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
            BeautifyChart(chartAylik);
            chartAylik.Titles.Add("Son 3 Ay Performansı");

            Series cagriSeries = new Series("Toplam Çağrı")
            {
                ChartType = SeriesChartType.Column,
                Color = SecondaryColor,
                YAxisType = AxisType.Primary,
                BorderWidth = 0,
                IsValueShownAsLabel = true,
                LabelForeColor = TextColor,
                Font = new Font("Century Gothic", 8F)
            };

            Series primSeries = new Series("Toplam Prim")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                Color = Color.FromArgb(220, 53, 69), // Kırmızı tonu prim için
                YAxisType = AxisType.Secondary,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 6,
                MarkerColor = Color.FromArgb(220, 53, 69),
                IsValueShownAsLabel = true,
                LabelForeColor = TextColor,
                Font = new Font("Century Gothic", 8F)
            };

            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                string query = @"
               SELECT CONCAT(pk.Yil, '-', FORMAT(pk.Ay, '00')) AS Ay,
               COUNT(c.CagriID) AS ToplamCagri,
               SUM(pk.PrimToplam) AS ToplamPrim
               FROM PrimKayit pk
               LEFT JOIN Cagri c ON c.AtananKullaniciID = pk.KullaniciID 
                                  AND YEAR(c.TeslimTarihi) = pk.Yil 
                                  AND MONTH(c.TeslimTarihi) = pk.Ay
                                  AND c.Durum = 'Tamamlandı'
               WHERE pk.KullaniciID = @KullaniciID AND
               pk.HesaplamaTarihi >= DATEADD(MONTH, -2, GETDATE())
               GROUP BY pk.Yil, pk.Ay
               ORDER BY pk.Yil, pk.Ay";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@KullaniciID", UserInformation.KullaniciID);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string ay = dr["Ay"].ToString();
                    cagriSeries.Points.AddXY(ay, dr["ToplamCagri"]);
                    primSeries.Points.AddXY(ay, dr["ToplamPrim"]);
                }
                dr.Close();
            }

            chartAylik.Series.Add(cagriSeries);
            chartAylik.Series.Add(primSeries);

            chartAylik.ChartAreas[0].AxisX.Title = "Ay";
            chartAylik.ChartAreas[0].AxisY.Title = "Çağrı Sayısı";
            chartAylik.ChartAreas[0].AxisY2.Title = "Prim (₺)";
            chartAylik.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;

            chartAylik.ChartAreas[0].AxisX.TitleForeColor = TextColor;
            chartAylik.ChartAreas[0].AxisY.TitleForeColor = TextColor;
            chartAylik.ChartAreas[0].AxisY2.TitleForeColor = TextColor;
            chartAylik.ChartAreas[0].AxisX.TitleFont = new Font("Century Gothic", 9F);
            chartAylik.ChartAreas[0].AxisY.TitleFont = new Font("Century Gothic", 9F);
            chartAylik.ChartAreas[0].AxisY2.TitleFont = new Font("Century Gothic", 9F);
        }

        private void OncelikDagilimiGrafik()
        {
            chartOncelik.Series.Clear();
            chartOncelik.Titles.Clear();
            BeautifyChart(chartOncelik);
            chartOncelik.Titles.Add("Çağrı Öncelik Dağılımı");

            Series oncelikSeries = new Series("Öncelik Dağılımı")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                LabelForeColor = Color.White,
                Font = new Font("Century Gothic", 9F, FontStyle.Bold)
            };

            Color[] priorityColors = {
                Color.FromArgb(220, 53, 69),   // Yüksek öncelik - Kırmızı
                Color.FromArgb(255, 193, 7),   // Orta öncelik - Sarı  
                AccentColor,                   // Normal - Ana tema mor
                Color.FromArgb(40, 167, 69),   // Düşük öncelik - Yeşil
                LightAccent,                   // Çok düşük - Açık mor
                SecondaryColor                 // Diğer - Orta mor
            };

            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                string query = @"
                SELECT c.Oncelik, COUNT(*) AS Adet
                FROM Cagri c
                WHERE c.AtananKullaniciID = @KullaniciID 
                AND c.Durum = 'Tamamlandı'
                AND c.CevapTarihi >= DATEADD(MONTH, -3, GETDATE())
                GROUP BY c.Oncelik
                ORDER BY Adet DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@KullaniciID", UserInformation.KullaniciID);
                SqlDataReader dr = cmd.ExecuteReader();

                int colorIndex = 0;
                while (dr.Read())
                {
                    var point = oncelikSeries.Points.AddXY(dr["Oncelik"].ToString(), dr["Adet"]);
                    oncelikSeries.Points[point].Color = priorityColors[colorIndex % priorityColors.Length];
                    colorIndex++;
                }
                dr.Close();
            }

            chartOncelik.Series.Add(oncelikSeries);
        }
    }
}