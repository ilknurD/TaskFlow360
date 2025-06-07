using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TaskFlow360
{
    public partial class ManagerReportsPage : Form
    {
        Baglanti baglanti = new Baglanti();

        public ManagerReportsPage()
        {
            InitializeComponent();
        }
        private void BeautifyChart(Chart chart)
        {
            chart.BackColor = Color.White;
            chart.ChartAreas.Clear();

            ChartArea area = new ChartArea();
            area.BackColor = Color.White;
            area.AxisX.LabelStyle.ForeColor = Color.DimGray;
            area.AxisY.LabelStyle.ForeColor = Color.DimGray;
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(224, 224, 224);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(224, 224, 224);
            chart.ChartAreas.Add(area);

            chart.Legends.Clear();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            ManagerProfile managerProfile = new ManagerProfile();
            managerProfile.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            ManagerTasks managerTasks = new ManagerTasks();
            managerTasks.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            ManagerReportsPage managerReportsPage = new ManagerReportsPage();
            managerReportsPage.Show();
            this.Close();
        }

        private void btnEkipYonetimi_Click(object sender, EventArgs e)
        {
            ManagerDashboard managerDashboard = new ManagerDashboard();
            managerDashboard.Show();
            this.Close();
        }

        private void ManagerReportsPage_Load(object sender, EventArgs e)
        {
            
            LoadDepartmentPerformance();    
            LoadTeamPerformance();
            LoadCallStatusDistribution();
            AylikPrimDagilimi();
        }

        private void LoadDepartmentPerformance()
        {
            BeautifyChart(chartDepartmentPerformance); // Eklendi

            chartDepartmentPerformance.Series.Clear();
            chartDepartmentPerformance.Titles.Clear();
            chartDepartmentPerformance.Titles.Add("Departman Performansı");
            chartDepartmentPerformance.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
            chartDepartmentPerformance.Titles[0].ForeColor = Color.FromArgb(126, 87, 194);

            Series cagriSeries = new Series("Çağrı Sayısı")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(126, 87, 194),
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };

            Series cozumSeries = new Series("Ort. Çözüm Süresi (dk)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(103, 58, 183),
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 8,
                MarkerColor = Color.FromArgb(103, 58, 183)
            };

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                string query = @"
            SELECT d.DepartmanAdi, 
                   COUNT(c.CagriID) AS CagriSayisi,
                   AVG(DATEDIFF(MINUTE, c.OlusturmaTarihi, c.TeslimTarihi)) AS OrtalamaCozumSuresi
            FROM Departman d
            LEFT JOIN Kullanici k ON d.DepartmanID = k.DepartmanID
            LEFT JOIN Cagri c ON k.KullaniciID = c.AtananKullaniciID
            WHERE c.OlusturmaTarihi >= DATEADD(MONTH, -1, GETDATE())
            GROUP BY d.DepartmanAdi
            ORDER BY CagriSayisi DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string departman = dr["DepartmanAdi"].ToString();
                    cagriSeries.Points.AddXY(departman, dr["CagriSayisi"]);
                    cozumSeries.Points.AddXY(departman, dr["OrtalamaCozumSuresi"]);
                }
                dr.Close();
            }

            chartDepartmentPerformance.Series.Add(cagriSeries);
            chartDepartmentPerformance.Series.Add(cozumSeries);
        }


        private void LoadTeamPerformance()
        {
            BeautifyChart(chartTeamPerformance);

            chartTeamPerformance.Series.Clear();
            chartTeamPerformance.Titles.Clear();
            chartTeamPerformance.Titles.Add("Ekip Performansı (Son 30 Gün)");
            chartTeamPerformance.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
            chartTeamPerformance.Titles[0].ForeColor = Color.FromArgb(126, 87, 194);

            Series series = new Series("Çağrı Sayısı")
            {
                ChartType = SeriesChartType.Bar,
                Color = Color.FromArgb(179, 136, 255), // Açık mor
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                            string query = @"
                        SELECT 
                k.Ad + ' ' + k.Soyad AS Personel,
                COUNT(CASE 
                        WHEN c.OlusturmaTarihi >= DATEADD(DAY, -30, GETDATE()) 
                             THEN c.CagriID 
                        ELSE NULL 
                     END) AS CagriSayisi
            FROM Kullanici k
            LEFT JOIN Cagri c ON k.KullaniciID = c.AtananKullaniciID
            WHERE k.YoneticiID = @YoneticiID
            GROUP BY k.Ad, k.Soyad
            ORDER BY CagriSayisi DESC
";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@YoneticiID", KullaniciBilgi.KullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    series.Points.AddXY(dr["Personel"].ToString(), dr["CagriSayisi"]);
                }
                dr.Close();
            }

            chartTeamPerformance.Series.Add(series);
        }


        private void LoadCallStatusDistribution()
        {
            BeautifyChart(chartCallStatus);

            chartCallStatus.Series.Clear();
            chartCallStatus.Titles.Clear();
            chartCallStatus.Titles.Add("Çağrı Durum Dağılımı");
            chartCallStatus.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
            chartCallStatus.Titles[0].ForeColor = Color.FromArgb(126, 87, 194);

            Series series = new Series
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                LabelForeColor = Color.Black
            };

            string[] pieColors = {
        "#7E57C2", // Mor
        "#FFD54F", // Sarı
        "#B39DDB", // Açık mor
        "#81D4FA", // Açık mavi
        "#AED581"  // Yeşilimsi
    };

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                string query = @"
            SELECT Durum, COUNT(*) AS Adet
            FROM Cagri
            WHERE AtananKullaniciID IN (
                SELECT KullaniciID FROM Kullanici WHERE YoneticiID = @YoneticiID
            )
            GROUP BY Durum";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@YoneticiID", KullaniciBilgi.KullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();
                int colorIndex = 0;

                while (dr.Read())
                {
                    DataPoint dp = new DataPoint();
                    dp.AxisLabel = dr["Durum"].ToString();
                    dp.YValues = new double[] { Convert.ToDouble(dr["Adet"]) };
                    dp.Color = ColorTranslator.FromHtml(pieColors[colorIndex % pieColors.Length]);
                    series.Points.Add(dp);
                    colorIndex++;
                }

                dr.Close();
            }

            chartCallStatus.Series.Add(series);
        }

        private void AylikPrimDagilimi()
        {
            BeautifyChart(chartMonthlyPrim);
            chartMonthlyPrim.Series.Clear();
            chartMonthlyPrim.Titles.Clear();
            chartMonthlyPrim.Titles.Add("Aylık Prim Dağılımı");
            chartMonthlyPrim.Titles[0].Font = new Font("Segoe UI", 12, FontStyle.Bold);
            chartMonthlyPrim.Titles[0].ForeColor = Color.FromArgb(126, 87, 194);

            Series primSeries = new Series("Toplam Prim")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(255, 193, 7), // Hardal sarısı
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                string query = @"
        SELECT CONCAT(pk.Yil, '-', FORMAT(pk.Ay, '00')) AS Ay,
               SUM(pk.PrimToplam) AS ToplamPrim
        FROM PrimKayit pk
        JOIN Kullanici k ON pk.KullaniciID = k.KullaniciID
        WHERE k.YoneticiID = @YoneticiID AND
              pk.HesaplamaTarihi >= DATEADD(MONTH, -5, GETDATE())
        GROUP BY pk.Yil, pk.Ay
        ORDER BY pk.Yil, pk.Ay";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@YoneticiID", KullaniciBilgi.KullaniciID);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    primSeries.Points.AddXY(dr["Ay"].ToString(), dr["ToplamPrim"]);
                }
                dr.Close();
            }
            chartMonthlyPrim.Series.Add(primSeries);
        }

    }
}