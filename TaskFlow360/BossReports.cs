using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TaskFlow360
{
    public partial class BossReports : Form
    {
        private readonly Logger _logger;

        public BossReports()
        {
            InitializeComponent();
            _logger = new Logger();
        }

        private void BossReports_Load(object sender, EventArgs e)
        {
            _logger.LogEkle("Görüntüleme", "BossReports", "Yönetici raporlar sayfası görüntülendi");
            DepartmanlaraGoreCagriListele();
            RollerKullaniciListele();
            PrimveMaasListele();
            AylikCagriListele();
        }

        private void BeautifyChart(Chart chart)
        {
            chart.ChartAreas[0].BackColor = Color.Transparent;
            chart.BackColor = Color.Transparent;
            chart.Legends[0].BackColor = Color.Transparent;
            chart.Legends[0].ForeColor = Color.Black;
            chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;
        }

        private void DepartmanlaraGoreCagriListele()
        {
            chartCagriDepartman.Series.Clear();
            chartCagriDepartman.Titles.Clear();
            BeautifyChart(chartCagriDepartman);

            Series series = new Series("Çağrı Sayısı");
            series.ChartType = SeriesChartType.Column;

            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                SqlCommand cmd = new SqlCommand(@"SELECT d.DepartmanAdi, COUNT(*) AS CagriSayisi
                                                  FROM Cagri c
                                                  JOIN Kullanici k ON c.AtananKullaniciID = k.KullaniciID
                                                  JOIN Departman d ON k.DepartmanID = d.DepartmanID
                                                  GROUP BY d.DepartmanAdi", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    series.Points.AddXY(reader["DepartmanAdi"].ToString(), Convert.ToInt32(reader["CagriSayisi"]));
                }
                reader.Close();
            }

            chartCagriDepartman.Series.Add(series);
            chartCagriDepartman.Titles.Add("Departman Bazlı Çağrı Sayısı");
            if (chartCagriDepartman.Titles.Count > 0)
            {
                chartCagriDepartman.Titles[0].Font = new Font("Century Gothic", 12, FontStyle.Bold | FontStyle.Italic);
                chartCagriDepartman.Titles[0].ForeColor = Color.FromArgb(50, 50, 50);
            }
        }

        private void RollerKullaniciListele()
        {
            chartKullaniciDagilimi.Series.Clear();
            chartKullaniciDagilimi.Titles.Clear();
            BeautifyChart(chartKullaniciDagilimi);

            Series series = new Series("Kullanıcı Dağılımı");
            series.ChartType = SeriesChartType.Doughnut;

            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                SqlCommand cmd = new SqlCommand("SELECT Rol, COUNT(*) AS Say FROM Kullanici GROUP BY Rol", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    series.Points.AddXY(reader["Rol"].ToString(), Convert.ToInt32(reader["Say"]));
                }
                reader.Close();
            }

            chartKullaniciDagilimi.Series.Add(series);
            chartKullaniciDagilimi.Titles.Add("Kullanıcı Rol Dağılımı");
            if (chartKullaniciDagilimi.Titles.Count > 0)
            {
                chartKullaniciDagilimi.Titles[0].Font = new Font("Century Gothic", 12, FontStyle.Bold | FontStyle.Italic);
                chartKullaniciDagilimi.Titles[0].ForeColor = Color.FromArgb(50, 50, 50);
            }
        }

        private void PrimveMaasListele()
        {
            chartMaasPrim.Series.Clear();
            chartMaasPrim.Titles.Clear();
            BeautifyChart(chartMaasPrim);

            Series maasSeries = new Series("Maaş");
            maasSeries.ChartType = SeriesChartType.Bar;
            Series primSeries = new Series("Prim");
            primSeries.ChartType = SeriesChartType.Bar;
            primSeries.YAxisType = AxisType.Secondary;

            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    CAST(Yil AS VARCHAR(4)) + '-' + 
                    RIGHT('0' + CAST(Ay AS VARCHAR(2)), 2) AS Ay, 
                    ISNULL(SUM(Maas), 0) AS ToplamMaas, 
                    ISNULL(SUM(PrimToplam), 0) AS ToplamPrim
                FROM PrimKayit
                WHERE Yil IS NOT NULL AND Ay IS NOT NULL
                GROUP BY Yil, Ay
                ORDER BY Yil, Ay", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string ay = reader["Ay"].ToString();

                    // Decimal olarak oku, sonra double'a çevir
                    decimal toplamMaas = Convert.ToDecimal(reader["ToplamMaas"]);
                    decimal toplamPrim = Convert.ToDecimal(reader["ToplamPrim"]);

                    maasSeries.Points.AddXY(ay, (double)toplamMaas);
                    primSeries.Points.AddXY(ay, (double)toplamPrim);
                }
                reader.Close();
            }

            chartMaasPrim.Series.Add(maasSeries);
            chartMaasPrim.Series.Add(primSeries);
        }


        private void AylikCagriListele()
        {
            chartCagriTrend.Series.Clear();
            chartCagriTrend.Titles.Clear();
            BeautifyChart(chartCagriTrend);

            Series series = new Series("Çağrı Sayısı");
            series.ChartType = SeriesChartType.Line;

            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                SqlCommand cmd = new SqlCommand(@"
                   SELECT FORMAT(OlusturmaTarihi, 'yyyy-MM') AS Ay, 
                          COUNT(*) AS CagriSayisi
                   FROM Cagri
                   GROUP BY FORMAT(OlusturmaTarihi, 'yyyy-MM')
                   ORDER BY Ay", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    series.Points.AddXY(reader["Ay"].ToString(), Convert.ToInt32(reader["CagriSayisi"]));
                }
                reader.Close();
            }

            chartCagriTrend.Series.Add(series);
            series.BorderWidth = 4; // Çizgi kalınlığı
            chartCagriTrend.Titles.Add("Aylık Çağrı Trend Analizi");
            if (chartCagriTrend.Titles.Count > 0)
            {
                chartCagriTrend.Titles[0].Font = new Font("Century Gothic", 12, FontStyle.Bold | FontStyle.Italic);
                chartCagriTrend.Titles[0].ForeColor = Color.FromArgb(50, 50, 50);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Çıkış", "BossReports", "Yönetici raporlar sayfasından çıkış yapıldı");
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Görüntüleme", "BossHomepage", "Yönetici ana sayfasına geçiş yapıldı");
            new BossHomepage().Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Görüntüleme", "BossProfile", "Profil sayfasına geçiş yapıldı");
            new BossProfile().Show();
            this.Close();
        }

        private void btnKullaniciIslem_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Görüntüleme", "BossUsersControl", "Kullanıcı işlemleri sayfasına geçiş yapıldı");
            new BossUsersControl().Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Görüntüleme", "BossReports", "Raporlar sayfasına geçiş yapıldı");
            new BossReports().Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Görüntüleme", "UsersSalary", "Maaş bilgileri sayfasına geçiş yapıldı");
            UsersSalary usersSalary = new UsersSalary();
            usersSalary.Show();
            this.Close();
        }
    }
}
