using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net;

namespace TaskFlow360
{
    public partial class ManagerReportsPage : Form
    {
        Baglanti baglanti = new Baglanti();

        public ManagerReportsPage()
        {
            InitializeComponent();
            LogEkle("ManagerReportsPage formu başlatıldı", "Form", "ManagerReportsPage");
        }

        private void LogEkle(string islemDetaylari, string islemTipi, string tabloAdi)
        {
            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();
                string sorgu = @"INSERT INTO Log (IslemTarihi, KullaniciID, IslemTipi, TabloAdi, IslemDetaylari, IPAdresi) 
                                VALUES (@IslemTarihi, @KullaniciID, @IslemTipi, @TabloAdi, @IslemDetaylari, @IPAdresi)";

                using (SqlCommand cmd = new SqlCommand(sorgu, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@IslemTarihi", DateTime.Now);
                    cmd.Parameters.AddWithValue("@KullaniciID", KullaniciBilgi.KullaniciID);
                    cmd.Parameters.AddWithValue("@IslemTipi", islemTipi);
                    cmd.Parameters.AddWithValue("@TabloAdi", tabloAdi);
                    cmd.Parameters.AddWithValue("@IslemDetaylari", islemDetaylari);
                    cmd.Parameters.AddWithValue("@IPAdresi", GetLocalIPAddress());
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Log kayıt hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.conn.State == ConnectionState.Open)
                    baglanti.conn.Close();
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
            area.AxisX.LabelStyle.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            area.AxisY.LabelStyle.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            chart.ChartAreas.Add(area);

            chart.Legends.Clear();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            LogEkle("Kapat butonuna tıklandı", "Buton", "ManagerReportsPage");
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            LogEkle("Küçült butonuna tıklandı", "Buton", "ManagerReportsPage");
            WindowState = FormWindowState.Minimized;
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            LogEkle("Çıkış butonuna tıklandı", "Buton", "ManagerReportsPage");
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            LogEkle("Anasayfa butonuna tıklandı", "Buton", "ManagerReportsPage");
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            LogEkle("Profil butonuna tıklandı", "Buton", "ManagerReportsPage");
            ManagerProfile managerProfile = new ManagerProfile();
            managerProfile.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            LogEkle("Görevler butonuna tıklandı", "Buton", "ManagerReportsPage");
            ManagerTasks managerTasks = new ManagerTasks();
            managerTasks.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            LogEkle("Raporlar butonuna tıklandı", "Buton", "ManagerReportsPage");
            ManagerReportsPage managerReportsPage = new ManagerReportsPage();
            managerReportsPage.Show();
            this.Close();
        }

        private void btnEkipYonetimi_Click(object sender, EventArgs e)
        {
            LogEkle("Ekip Yönetimi butonuna tıklandı", "Buton", "ManagerReportsPage");
            ManagerDashboard managerDashboard = new ManagerDashboard();
            managerDashboard.Show();
            this.Close();
        }

        private void ManagerReportsPage_Load(object sender, EventArgs e)
        {
            LogEkle("ManagerReportsPage yüklenmeye başlandı", "Form", "ManagerReportsPage");
            DepartmanPerformans();    
            LogEkle("Departman performansı yüklendi", "Okuma", "ManagerReportsPage");
            TakimPerformans();
            LogEkle("Ekip performansı yüklendi", "Okuma", "ManagerReportsPage");
            CagriDurum();
            LogEkle("Çağrı durum dağılımı yüklendi", "Okuma", "ManagerReportsPage");
            AylikPrimDagilimi();
            LogEkle("Aylık prim dağılımı yüklendi", "Okuma", "ManagerReportsPage");
        }

        private void DepartmanPerformans()
        {
            BeautifyChart(chartDepartmentPerformance);
            chartDepartmentPerformance.Series.Clear();
            chartDepartmentPerformance.Titles.Clear();
            chartDepartmentPerformance.Titles.Add("Departman Performansı");
            chartDepartmentPerformance.Titles[0].Font = new Font("Century Gothic", 12, FontStyle.Bold);
            chartDepartmentPerformance.Titles[0].ForeColor = Color.FromArgb(126, 87, 194);

            Series cagriSeries = new Series("Çağrı Sayısı")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(126, 87, 194),
                IsValueShownAsLabel = true,
                Font = new Font("Century Gothic", 8, FontStyle.Bold)
            };

            Series cozumSeries = new Series("Ort. Çözüm Süresi (dk)")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.FromArgb(103, 58, 183),
                BorderWidth = 3,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 8,
                MarkerColor = Color.FromArgb(103, 58, 183),
                Font = new Font("Century Gothic", 8, FontStyle.Bold)
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


        private void TakimPerformans()
        {
            BeautifyChart(chartTeamPerformance);
            chartTeamPerformance.Series.Clear();
            chartTeamPerformance.Titles.Clear();
            chartTeamPerformance.Titles.Add("Ekip Performansı (Son 30 Gün)");
            chartTeamPerformance.Titles[0].Font = new Font("Century Gothic", 12, FontStyle.Bold);
            chartTeamPerformance.Titles[0].ForeColor = Color.FromArgb(126, 87, 194);

            Series series = new Series("Çağrı Sayısı")
            {
                ChartType = SeriesChartType.Bar,
                Color = Color.FromArgb(179, 136, 255),
                IsValueShownAsLabel = true,
                Font = new Font("Century Gothic", 8, FontStyle.Bold)
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


        private void CagriDurum()
        {
            BeautifyChart(chartCallStatus);
            chartCallStatus.Series.Clear();
            chartCallStatus.Titles.Clear();
            chartCallStatus.Titles.Add("Çağrı Durum Dağılımı");
            chartCallStatus.Titles[0].Font = new Font("Century Gothic", 12, FontStyle.Bold);
            chartCallStatus.Titles[0].ForeColor = Color.FromArgb(126, 87, 194);

            Series series = new Series
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                Font = new Font("Century Gothic", 9, FontStyle.Regular),
                LabelForeColor = Color.Black
            };

            string[] pieColors = {
                "#7E57C2", // Mor
                "#FFD54F", // Sarı
                "#B39DDB", // Açık mor
                "#81D4FA", // Açık mavi
                "#AED581"  // Yeşilimsi
            };

            chartCallStatus.Legends.Clear();
            Legend legend = new Legend("Durumlar");
            legend.Docking = Docking.Right;
            legend.Font = new Font("Century Gothic", 9, FontStyle.Regular);
            legend.LegendStyle = LegendStyle.Table;
            chartCallStatus.Legends.Add(legend);

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
                    dp.LegendText = dr["Durum"].ToString();
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
            chartMonthlyPrim.Titles[0].Font = new Font("Century Gothic", 12, FontStyle.Bold);
            chartMonthlyPrim.Titles[0].ForeColor = Color.FromArgb(126, 87, 194);

            Series primSeries = new Series("Toplam Prim")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(255, 193, 7),
                IsValueShownAsLabel = true,
                Font = new Font("Century Gothic", 8, FontStyle.Bold)
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