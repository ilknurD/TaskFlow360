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
    public partial class ManagerDashboard : Form
    {
        Baglanti baglanti = new Baglanti();
        public ManagerDashboard()
        {
            InitializeComponent();
        }

        private void ManagerDashboard_Load(object sender, EventArgs e)
        {
            EkibindekiKullanicilariGetir();
            SonCagrilariGetir();
            SonIslemleriGetir();
            AylikCagriDurumlariniCharttaGoster();
            foreach (var dgv in new[] { EkipDGV, SonGorevlerDGV, SonIslemlerDGV })
            {
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                StilUygula(dgv);
            }
        }
        private void StilUygula(DataGridView dgv)
        {
            dgv.Font = new Font("Century Gothic", 10);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(126, 87, 194); // Mor
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(179, 157, 219); // Açık mor
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.RowHeadersVisible = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Color.FromArgb(230, 230, 250); // Morumsu grid çizgisi

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 240, 255); // Hafif mor ton
        }

        private void EkibindekiKullanicilariGetir()
        {
            // DataGrid ayarları - boyutun korunması ve kaydırma çubuklarının eklenmesi  
            EkipDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            EkipDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            EkipDGV.ScrollBars = ScrollBars.Both;

            SqlConnection connection = null;

            try
            {
                connection = Baglanti.BaglantiGetir();
                baglanti.BaglantiAc();

                string query = @"
            SELECT   
                k.KullaniciID,   
                k.Ad,   
                k.Soyad,   
                k.Email,   
                k.Telefon,   
                b.BolumAdi,  
                COUNT(c.CagriID) AS AktifCagriSayisi  
            FROM Kullanici k  
            LEFT JOIN Bolum b ON k.BolumID = b.BolumID  
            LEFT JOIN Cagri c ON k.KullaniciID = c.AtananKullaniciID AND c.Durum = 'Beklemede'  
            WHERE k.YoneticiID = @YoneticiID  
            GROUP BY k.KullaniciID, k.Ad, k.Soyad, k.Email, k.Telefon, b.BolumAdi  
            ORDER BY k.Ad, k.Soyad";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@YoneticiID", KullaniciBilgi.KullaniciID);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    EkipDGV.DataSource = dataTable;

                    if (EkipDGV.Columns.Count > 0)
                    {
                        EkipDGV.Columns["KullaniciID"].HeaderText = "ID";
                        EkipDGV.Columns["Ad"].HeaderText = "Adı";
                        EkipDGV.Columns["Soyad"].HeaderText = "Soyadı";
                        EkipDGV.Columns["Email"].HeaderText = "E-posta";
                        EkipDGV.Columns["Telefon"].HeaderText = "Telefon";
                        EkipDGV.Columns["BolumAdi"].HeaderText = "Bölüm";
                        EkipDGV.Columns["AktifCagriSayisi"].HeaderText = "Aktif Çağrı Sayısı";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekip üyeleri getirilirken bir hata oluştu: " + ex.Message, "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat(); 
            }
        }

        private void SonCagrilariGetir()
        {
            // DataGrid ayarları
            SonGorevlerDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SonGorevlerDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            SonGorevlerDGV.ScrollBars = ScrollBars.Both;

            SqlConnection connection = null;

            try
            {
                connection = Baglanti.BaglantiGetir();
                baglanti.BaglantiAc();

                string query = @"
            SELECT TOP 10
                c.CagriID,
                c.Baslik,
                c.Durum,
                c.OlusturmaTarihi,
                k.Ad + ' ' + k.Soyad AS AtananKullanici,
                b.BolumAdi
            FROM Cagri c
            LEFT JOIN Kullanici k ON c.AtananKullaniciID = k.KullaniciID
            LEFT JOIN Bolum b ON k.BolumID = b.BolumID
            WHERE k.YoneticiID = @YoneticiID
            ORDER BY c.OlusturmaTarihi DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@YoneticiID", KullaniciBilgi.KullaniciID);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    SonGorevlerDGV.DataSource = dataTable;

                    if (SonGorevlerDGV.Columns.Count > 0)
                    {
                        SonGorevlerDGV.Columns["CagriID"].HeaderText = "Çağrı ID";
                        SonGorevlerDGV.Columns["Baslik"].HeaderText = "Başlık";
                        SonGorevlerDGV.Columns["Durum"].HeaderText = "Durum";
                        SonGorevlerDGV.Columns["OlusturmaTarihi"].HeaderText = "Oluşturulma Tarihi";
                        SonGorevlerDGV.Columns["AtananKullanici"].HeaderText = "Atanan Kişi";
                        SonGorevlerDGV.Columns["BolumAdi"].HeaderText = "Bölüm";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Son çağrılar getirilirken bir hata oluştu: " + ex.Message, "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void SonIslemleriGetir()
        {
            SonIslemlerDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SonIslemlerDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            SonIslemlerDGV.ScrollBars = ScrollBars.Both;

            SqlConnection connection = null;

            try
            {
                connection = Baglanti.BaglantiGetir();
                baglanti.BaglantiAc();

                    string query = @"SELECT TOP 10
                k.Ad + ' ' + k.Soyad AS CalisanAdi,
                c.Baslik,
                c.Durum,
                c.CagriAciklama,
                c.OlusturmaTarihi,
                c.TeslimTarihi,
                c.CevapTarihi,
                -- En son değişiklik tarihini hesapla
                CASE 
                    WHEN c.CevapTarihi IS NOT NULL THEN c.CevapTarihi
                    WHEN c.TeslimTarihi IS NOT NULL THEN c.TeslimTarihi
                    ELSE c.OlusturmaTarihi
                END AS SonDegisiklikTarihi
            FROM dbo.Cagri c
            INNER JOIN dbo.Kullanici k ON c.AtananKullaniciID = k.KullaniciID
            WHERE k.YoneticiID = @YoneticiID
            ORDER BY SonDegisiklikTarihi DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@YoneticiID", KullaniciBilgi.KullaniciID);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        SonIslemlerDGV.DataSource = dataTable;

                        if (SonIslemlerDGV.Columns.Count > 0)
                        {
                            SonIslemlerDGV.Columns["CalisanAdi"].HeaderText = "Çalışan";
                            SonIslemlerDGV.Columns["Baslik"].HeaderText = "Çağrı Başlığı";
                            SonIslemlerDGV.Columns["Durum"].HeaderText = "Durum";
                            SonIslemlerDGV.Columns["CagriAciklama"].HeaderText = "Açıklama";
                            SonIslemlerDGV.Columns["OlusturmaTarihi"].HeaderText = "Oluşturma Tarihi";
                            SonIslemlerDGV.Columns["TeslimTarihi"].HeaderText = "Teslim Tarihi";
                            SonIslemlerDGV.Columns["CevapTarihi"].HeaderText = "Cevap Tarihi";
                            SonIslemlerDGV.Columns["SonDegisiklikTarihi"].HeaderText = "Son Değişiklik";

                            // İstenmeyen sütunları gizleyebilirsiniz
                            SonIslemlerDGV.Columns["OlusturmaTarihi"].Visible = false;
                            SonIslemlerDGV.Columns["TeslimTarihi"].Visible = false;
                            SonIslemlerDGV.Columns["CevapTarihi"].Visible = false;
                        }
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Son işlemler alınırken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void AylikCagriDurumlariniCharttaGoster()
        {
            SqlConnection connection = null;
            try
            {
                connection = Baglanti.BaglantiGetir();
                baglanti.BaglantiAc();

                string query = @"
        SELECT 
            Durum,
            COUNT(*) AS CagriSayisi
        FROM dbo.Cagri
        WHERE YEAR(OlusturmaTarihi) = YEAR(GETDATE())
              AND MONTH(OlusturmaTarihi) = MONTH(GETDATE())
        GROUP BY Durum
        ORDER BY CagriSayisi DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Grafik temizleme
                    chartCagriDurum.Series.Clear();
                    chartCagriDurum.Titles.Clear();
                    chartCagriDurum.ChartAreas.Clear();

                    // Chart Area ayarları
                    ChartArea area = new ChartArea();
                    area.BackColor = Color.WhiteSmoke;
                    area.AxisX.Title = "Çağrı Durumu";
                    area.AxisX.TitleFont = new Font("Century Gothic", 10, FontStyle.Bold);
                    area.AxisX.MajorGrid.LineColor = Color.LightGray;
                    area.AxisX.LabelStyle.Angle = -45;
                    area.AxisY.Title = "Çağrı Sayısı";
                    area.AxisY.TitleFont = new Font("Century Gothic", 10, FontStyle.Bold);
                    area.AxisY.MajorGrid.LineColor = Color.LightGray;
                    chartCagriDurum.ChartAreas.Add(area);

                    // Seri oluştur
                    Series series = new Series("Çağrı\nDurumları");
                    series.ChartType = SeriesChartType.Column;
                    series.IsValueShownAsLabel = true;
                    series.LabelForeColor = Color.Black;
                    series.Font = new Font("Century Gothic", 10, FontStyle.Bold);
                    series.BorderColor = Color.Black;
                    series.BorderWidth = 1;

                    // Renk paleti
                    Color[] renkler = new Color[]
                    {
                    Color.FromArgb(126, 87, 194), // Mor
                    Color.FromArgb(255, 138, 101), // Turuncu
                    Color.FromArgb(102, 187, 106), // Yeşil
                    Color.FromArgb(65, 105, 225), // Mavi
                    Color.FromArgb(255, 202, 40)  // Sarı
                    };

                    int renkIndex = 0;

                    // Veriyi seriye ekle
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string durum = row["Durum"].ToString();
                        int sayi = Convert.ToInt32(row["CagriSayisi"]);
                        int pointIndex = series.Points.AddXY(durum, sayi);
                        series.Points[pointIndex].Label = sayi.ToString();
                        series.Points[pointIndex].Color = renkler[renkIndex % renkler.Length];
                        renkIndex++;
                    }

                    // Seriyi ekle
                    chartCagriDurum.Series.Add(series);

                    // Başlık
                    chartCagriDurum.Titles.Add(new Title
                    {
                        Text = $"{DateTime.Now:MMMM} Ayı Çağrı Durumları",
                        Font = new Font("Century Gothic", 12, FontStyle.Bold),
                        ForeColor = Color.FromArgb(94, 53, 177), // Tema rengi
                        Docking = Docking.Top,
                        Alignment = ContentAlignment.MiddleCenter
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Aylık çağrı durumları yüklenirken hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
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
    }
}
