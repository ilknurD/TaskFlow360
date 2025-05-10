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
            // Implementation for team management would go here
        }

        private void ManagerReportsPage_Load(object sender, EventArgs e)
        {
            LoadDepartmentPerformance();    
            LoadTeamPerformance();
            LoadCallStatusDistribution();
            LoadMonthlyPrimReport();
        }

        private void LoadDepartmentPerformance()
        {
            chartDepartmentPerformance.Series.Clear();
            chartDepartmentPerformance.Titles.Clear();
            chartDepartmentPerformance.Titles.Add("Departman Performansı");

            Series cagriSeries = new Series("Çağrı Sayısı")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.SteelBlue
            };

            Series cozumSeries = new Series("Ort. Çözüm Süresi (dk)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Orange,
                BorderWidth = 3
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
            chartTeamPerformance.Series.Clear();
            chartTeamPerformance.Titles.Clear();
            chartTeamPerformance.Titles.Add("Ekip Performansı (Son 30 Gün)");

            Series series = new Series("Çağrı Sayısı")
            {
                ChartType = SeriesChartType.Bar,
                Color = Color.DodgerBlue,
                IsValueShownAsLabel = true
            };

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                string query = @"
                    SELECT k.Ad + ' ' + k.Soyad AS Personel,
                           COUNT(c.CagriID) AS CagriSayisi,
                           SUM(pr.Prim) AS ToplamPrim
                    FROM Kullanici k
                    LEFT JOIN Cagri c ON k.KullaniciID = c.AtananKullaniciID
                    LEFT JOIN PerformansRaporu pr ON k.KullaniciID = pr.KullaniciID
                    WHERE k.YoneticiID = @YoneticiID AND
                          c.OlusturmaTarihi >= DATEADD(DAY, -30, GETDATE())
                    GROUP BY k.Ad, k.Soyad
                    ORDER BY CagriSayisi DESC";

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
            chartCallStatus.Series.Clear();
            chartCallStatus.Titles.Clear();
            chartCallStatus.Titles.Add("Çağrı Durum Dağılımı");

            Series series = new Series
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true
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
                while (dr.Read())
                {
                    series.Points.AddXY(dr["Durum"].ToString(), dr["Adet"]);
                }
                dr.Close();
            }

            chartCallStatus.Series.Add(series);
        }

        private void LoadMonthlyPrimReport()
        {
            chartMonthlyPrim.Series.Clear();
            chartMonthlyPrim.Titles.Clear();
            chartMonthlyPrim.Titles.Add("Aylık Prim Dağılımı");

            Series primSeries = new Series("Toplam Prim")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Gold,
                IsValueShownAsLabel = true
            };

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                string query = @"
                    SELECT FORMAT(pr.RaporTarihi, 'yyyy-MM') AS Ay,
                           SUM(pr.Prim) AS ToplamPrim
                    FROM PerformansRaporu pr
                    JOIN Kullanici k ON pr.KullaniciID = k.KullaniciID
                    WHERE k.YoneticiID = @YoneticiID AND
                          pr.RaporTarihi >= DATEADD(MONTH, -5, GETDATE())
                    GROUP BY FORMAT(pr.RaporTarihi, 'yyyy-MM')
                    ORDER BY Ay";

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