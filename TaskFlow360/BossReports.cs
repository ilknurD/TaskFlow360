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
        public BossReports()
        {
            InitializeComponent();

        }

        private void BossReports_Load(object sender, EventArgs e)
        {
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

            using (SqlConnection conn = Baglanti.BaglantiGetir())
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

            using (SqlConnection conn = Baglanti.BaglantiGetir())
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
            maasSeries.ChartType = SeriesChartType.Line;

            Series primSeries = new Series("Prim");
            primSeries.ChartType = SeriesChartType.Line;

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT FORMAT(RaporTarihi, 'yyyy-MM') AS Ay, 
                           SUM(Maas) AS ToplamMaas, 
                           SUM(Prim) AS ToplamPrim
                    FROM PerformansRaporu
                    GROUP BY FORMAT(RaporTarihi, 'yyyy-MM')
                    ORDER BY Ay", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string ay = reader["Ay"].ToString();
                    maasSeries.Points.AddXY(ay, Convert.ToDouble(reader["ToplamMaas"]));
                    primSeries.Points.AddXY(ay, Convert.ToDouble(reader["ToplamPrim"]));
                }
                reader.Close();
            }

            chartMaasPrim.Series.Add(maasSeries);
            chartMaasPrim.Series.Add(primSeries);
            maasSeries.BorderWidth = 4;
            primSeries.BorderWidth = 4;
            chartMaasPrim.Titles.Add("Aylık Maaş ve Prim Karşılaştırması");
            if (chartMaasPrim.Titles.Count > 0)
            {
                chartMaasPrim.Titles[0].Font = new Font("Century Gothic", 12, FontStyle.Bold | FontStyle.Italic);
                chartMaasPrim.Titles[0].ForeColor = Color.FromArgb(50, 50, 50);
            }
        }

        private void AylikCagriListele()
        {
            chartCagriTrend.Series.Clear();
            chartCagriTrend.Titles.Clear();
            BeautifyChart(chartCagriTrend);

            Series series = new Series("Çağrı Sayısı");
            series.ChartType = SeriesChartType.Line;

            using (SqlConnection conn = Baglanti.BaglantiGetir())
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
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            new BossHomepage().Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            new BossProfile().Show();
            this.Close();
        }

        private void btnKullaniciIslem_Click(object sender, EventArgs e)
        {
            new BossUsersControl().Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            new BossReports().Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UsersSalary usersSalary = new UsersSalary();
            usersSalary.Show();
            this.Close();
        }
    }
}
