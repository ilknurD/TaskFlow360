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
    public partial class UsersSalary : Form
    {
        private string GirisYapanMudurID;
        Connection baglanti = new Connection();
        private readonly Logger _logger;

        public UsersSalary()
        {
            InitializeComponent();
            _logger = new Logger();
            GirisYapanMudurID = UserInformation.KullaniciID;
        }

        private void UsersSalary_Load(object sender, EventArgs e)
        {
            AyComboBoxDoldur();
            DataGridViewKolonlariAyarla();
            PrimAyarlariYukle();
            stil();
            KullaniciPrimleriniYukle();
            PrimveMaasListele();
        }

        private void DataGridViewKolonlariAyarla()
        {
            dataGridViewKullanicilar.Columns.Clear();
            dataGridViewKullanicilar.AutoGenerateColumns = false;

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AdSoyad",
                HeaderText = "Ad Soyad",
                DataPropertyName = "AdSoyad",
                Width = 150
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ToplamCagri",
                HeaderText = "Toplam Çağrı",
                DataPropertyName = "ToplamCagri",
                Width = 100
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "YuksekOncelik",
                HeaderText = "Yüksek Öncelik",
                DataPropertyName = "YuksekOncelik",
                Width = 120
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OrtaOncelik",
                HeaderText = "Orta Öncelik",
                DataPropertyName = "OrtaOncelik",
                Width = 120
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DusukOncelik",
                HeaderText = "Düşük Öncelik",
                DataPropertyName = "DusukOncelik",
                Width = 120
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TemelMaas",
                HeaderText = "Temel Maaş",
                DataPropertyName = "TemelMaas",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ToplamPrim",
                HeaderText = "Toplam Prim",
                DataPropertyName = "ToplamPrim",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ToplamOdeme",
                HeaderText = "Toplam Ödeme",
                DataPropertyName = "ToplamOdeme",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });
        }

        private void AyComboBoxDoldur()
        {
            cmbAyFiltre.Items.Clear();

            string[] aylar = { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                              "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

            for (int i = 0; i < aylar.Length; i++)
            {
                cmbAyFiltre.Items.Add($"{i + 1:00} - {aylar[i]}");
            }

            int suankiAy = DateTime.Now.Month;
            cmbAyFiltre.SelectedIndex = suankiAy - 1;
        }

        private void stil()
        {
            dataGridViewKullanicilar.AutoGenerateColumns = false;
            dataGridViewKullanicilar.AllowUserToAddRows = false;
            dataGridViewKullanicilar.AllowUserToDeleteRows = false;
            dataGridViewKullanicilar.ReadOnly = true;
            dataGridViewKullanicilar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewKullanicilar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewKullanicilar.RowHeadersVisible = false;
            dataGridViewKullanicilar.BorderStyle = BorderStyle.None;

            foreach (DataGridViewColumn column in dataGridViewKullanicilar.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dataGridViewKullanicilar.Font = new Font("Century Gothic", 10);
            dataGridViewKullanicilar.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 11, FontStyle.Bold);
            dataGridViewKullanicilar.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(126, 87, 194);
            dataGridViewKullanicilar.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewKullanicilar.EnableHeadersVisualStyles = false;

            dataGridViewKullanicilar.DefaultCellStyle.BackColor = Color.White;
            dataGridViewKullanicilar.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewKullanicilar.DefaultCellStyle.SelectionBackColor = Color.FromArgb(179, 157, 219);
            dataGridViewKullanicilar.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridViewKullanicilar.GridColor = Color.FromArgb(230, 230, 250);
            dataGridViewKullanicilar.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 240, 255);
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
        private void PrimAyarlariYukle()
        {
            try
            {
                baglanti.BaglantiAc();

                string query = "SELECT TOP 1 * FROM PrimAyar ORDER BY EklemeTarihi DESC";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Null kontrolü ile değerleri al
                        txtMinGorevSayisi.Text = (reader["MinGorevSayisi"] ?? 0).ToString();
                        txtYuksek.Text = (reader["Yüksek"] ?? 0).ToString();
                        txtOrta.Text = (reader["Orta"] ?? 0).ToString();
                        txtDusuk.Text = (reader["Düşük"] ?? 0).ToString();
                    }
                    else
                    {
                        // Varsayılan değerler
                        txtMinGorevSayisi.Text = "0";
                        txtYuksek.Text = "0";
                        txtOrta.Text = "0";
                        txtDusuk.Text = "0";
                        MessageBox.Show("Prim ayarı bulunamadı. Varsayılan değerler yüklendi.", "Bilgi");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Prim ayarları yüklenirken hata: " + ex.Message);

                // Hata durumunda varsayılan değerler
                txtMinGorevSayisi.Text = "0";
                txtYuksek.Text = "0";
                txtOrta.Text = "0";
                txtDusuk.Text = "0";
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void KullaniciPrimleriniYukle()
        {
            try
            {
                int secilenAy = cmbAyFiltre.SelectedIndex + 1;
                int guncelYil = DateTime.Now.Year;

                baglanti.BaglantiAc();

                // Prim ayarlarını önceden al
                decimal yuksekPrim = 0, ortaPrim = 0, dusukPrim = 0;
                int minGorevSayisi = 0;

                string primAyarQuery = "SELECT TOP 1 MinGorevSayisi, Yüksek, Orta, Düşük FROM PrimAyar ORDER BY EklemeTarihi DESC";
                using (SqlCommand primCmd = new SqlCommand(primAyarQuery, baglanti.conn))
                using (SqlDataReader primReader = primCmd.ExecuteReader())
                {
                    if (primReader.Read())
                    {
                        minGorevSayisi = Convert.ToInt32(primReader["MinGorevSayisi"] ?? 0);
                        yuksekPrim = Convert.ToDecimal(primReader["Yüksek"] ?? 0);
                        ortaPrim = Convert.ToDecimal(primReader["Orta"] ?? 0);
                        dusukPrim = Convert.ToDecimal(primReader["Düşük"] ?? 0);
                    }
                }

                string query = @"
                SELECT 
                    k.KullaniciID,
                    ISNULL(k.Ad, '') + ' ' + ISNULL(k.Soyad, '') AS AdSoyad,
                    ISNULL(k.Maas, 0) AS TemelMaas,
                    ISNULL(COUNT(c.CagriID), 0) AS ToplamCagri,
                    ISNULL(SUM(CASE WHEN c.Oncelik = 'Yüksek' THEN 1 ELSE 0 END), 0) AS YuksekOncelik,
                    ISNULL(SUM(CASE WHEN c.Oncelik = 'Orta' THEN 1 ELSE 0 END), 0) AS OrtaOncelik,
                    ISNULL(SUM(CASE WHEN c.Oncelik = 'Düşük' THEN 1 ELSE 0 END), 0) AS DusukOncelik
                FROM Kullanici k
                LEFT JOIN Cagri c ON k.KullaniciID = c.AtananKullaniciID 
                    AND MONTH(c.TeslimTarihi) = @Ay 
                    AND YEAR(c.TeslimTarihi) = @Yil
                    AND c.Durum = 'Tamamlandı'
                WHERE k.KullaniciID IS NOT NULL
                GROUP BY k.KullaniciID, k.Ad, k.Soyad, k.Maas
                ORDER BY k.Ad, k.Soyad";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@Yil", guncelYil);
                    cmd.Parameters.AddWithValue("@Ay", secilenAy);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dt.Columns.Add("ToplamPrim", typeof(decimal));
                    dt.Columns.Add("ToplamOdeme", typeof(decimal));

                    foreach (DataRow row in dt.Rows)
                    {
                        int toplamCagri = Convert.ToInt32(row["ToplamCagri"] ?? 0);
                        int yuksekOncelik = Convert.ToInt32(row["YuksekOncelik"] ?? 0);
                        int ortaOncelik = Convert.ToInt32(row["OrtaOncelik"] ?? 0);
                        int dusukOncelik = Convert.ToInt32(row["DusukOncelik"] ?? 0);
                        decimal temelMaas = Convert.ToDecimal(row["TemelMaas"] ?? 0);

                        decimal toplamPrim = 0;
                        if (toplamCagri >= minGorevSayisi)
                        {
                            toplamPrim = (yuksekOncelik * yuksekPrim) +
                                        (ortaOncelik * ortaPrim) +
                                        (dusukOncelik * dusukPrim);
                        }

                        row["ToplamPrim"] = toplamPrim;
                        row["ToplamOdeme"] = temelMaas + toplamPrim;
                    }

                    Console.WriteLine($"Sorgu sonucu: {dt.Rows.Count} kullanıcı bulundu");

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show($"Hiç kullanıcı bulunamadı!\nSeçilen: {secilenAy}. ay, {guncelYil} yılı", "Uyarı");
                    }

                    dataGridViewKullanicilar.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Kritik Hata");
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                _logger.LogEkle("Prim Ayarı", "PrimAyar", "Prim ayarları güncellendi");
                PrimAyarlariKaydet();
                PrimleriHesapla();
                KullaniciPrimleriniYukle();
                MessageBox.Show("Prim ayarları kaydedildi ve primler hesaplandı.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem sırasında hata: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(txtMinGorevSayisi.Text) ||
                string.IsNullOrEmpty(txtYuksek.Text) ||
                string.IsNullOrEmpty(txtOrta.Text) ||
                string.IsNullOrEmpty(txtDusuk.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı");
                return false;
            }

            if (!int.TryParse(txtMinGorevSayisi.Text, out int minGorev) || minGorev < 0)
            {
                MessageBox.Show("Minimum görev sayısı geçerli bir sayı olmalıdır.", "Uyarı");
                return false;
            }

            if (!decimal.TryParse(txtYuksek.Text, out decimal yuksek) || yuksek < 0 ||
                !decimal.TryParse(txtOrta.Text, out decimal orta) || orta < 0 ||
                !decimal.TryParse(txtDusuk.Text, out decimal dusuk) || dusuk < 0)
            {
                MessageBox.Show("Prim miktarları geçerli sayılar olmalıdır.", "Uyarı");
                return false;
            }

            return true;
        }

        private void PrimAyarlariKaydet()
        {
            try
            {
                baglanti.BaglantiAc();

                string query = @"
                INSERT INTO PrimAyar (MinGorevSayisi, Yüksek, Orta, Düşük, EklemeTarihi)
                VALUES (@MinGorev, @Yuksek, @Orta, @Dusuk, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@MinGorev", Convert.ToInt32(txtMinGorevSayisi.Text));
                    cmd.Parameters.AddWithValue("@Yuksek", Convert.ToDecimal(txtYuksek.Text));
                    cmd.Parameters.AddWithValue("@Orta", Convert.ToDecimal(txtOrta.Text));
                    cmd.Parameters.AddWithValue("@Dusuk", Convert.ToDecimal(txtDusuk.Text));
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private object GetSafeValue(SqlDataReader reader, string columnName, object defaultValue)
        {
            return reader[columnName] == DBNull.Value ? defaultValue : reader[columnName];
        }

        private void PrimleriHesapla()
        {
            try
            {
                _logger.LogEkle("Prim Hesaplama", "PrimKayit", "Kullanıcı primleri hesaplanmaya başlandı");
                baglanti.BaglantiAc();

                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;

                string primAyarQuery = "SELECT TOP 1 AyarID, MinGorevSayisi, Yüksek, Orta, Düşük FROM PrimAyar ORDER BY EklemeTarihi DESC";
                int minGorevSayisi = 0;
                int primAyarID = 0; 
                decimal yuksekPrim = 0, ortaPrim = 0, dusukPrim = 0;

                using (SqlCommand cmd = new SqlCommand(primAyarQuery, baglanti.conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        primAyarID = Convert.ToInt32(GetSafeValue(reader, "AyarID", 0));
                        minGorevSayisi = Convert.ToInt32(GetSafeValue(reader, "MinGorevSayisi", 0));
                        yuksekPrim = Convert.ToDecimal(GetSafeValue(reader, "Yüksek", 0));
                        ortaPrim = Convert.ToDecimal(GetSafeValue(reader, "Orta", 0));
                        dusukPrim = Convert.ToDecimal(GetSafeValue(reader, "Düşük", 0));
                    }
                    else
                    {
                        MessageBox.Show("Prim ayarı bulunamadı. Lütfen önce bir prim ayarı girin.");
                        return;
                    }
                }

                if (primAyarID == 0)
                {
                    MessageBox.Show("Geçerli bir prim ayarı bulunamadı.");
                    return;
                }

                string kullaniciQuery = "SELECT KullaniciID, Maas FROM Kullanici";
                using (SqlCommand kullaniciCmd = new SqlCommand(kullaniciQuery, baglanti.conn))
                using (SqlDataReader reader = kullaniciCmd.ExecuteReader())
                {
                    List<dynamic> kullanicilar = new List<dynamic>();

                    while (reader.Read())
                    {
                        kullanicilar.Add(new
                        {
                            KullaniciID = Convert.ToInt32(GetSafeValue(reader, "KullaniciID", 0)),
                            Maas = Convert.ToDecimal(GetSafeValue(reader, "Maas", 0))
                        });
                    }

                    reader.Close();

                    foreach (var kullanici in kullanicilar)
                    {
                        string cagriQuery = @"
                        SELECT 
                            ISNULL(COUNT(*), 0) AS ToplamCagri,
                            ISNULL(SUM(CASE WHEN Oncelik = 'Yüksek' THEN 1 ELSE 0 END), 0) AS YuksekOncelik,
                            ISNULL(SUM(CASE WHEN Oncelik = 'Orta' THEN 1 ELSE 0 END), 0) AS OrtaOncelik,
                            ISNULL(SUM(CASE WHEN Oncelik = 'Düşük' THEN 1 ELSE 0 END), 0) AS DusukOncelik
                        FROM Cagri 
                        WHERE AtananKullaniciID = @KullaniciID 
                        AND Durum = 'Tamamlandı'
                        AND TeslimTarihi IS NOT NULL
                        AND YEAR(TeslimTarihi) = @Yil 
                        AND MONTH(TeslimTarihi) = @Ay";

                        using (SqlCommand cagriCmd = new SqlCommand(cagriQuery, baglanti.conn))
                        {
                            cagriCmd.Parameters.AddWithValue("@KullaniciID", kullanici.KullaniciID);
                            cagriCmd.Parameters.AddWithValue("@Yil", currentYear);
                            cagriCmd.Parameters.AddWithValue("@Ay", currentMonth);

                            using (SqlDataReader cagriReader = cagriCmd.ExecuteReader())
                            {
                                if (cagriReader.Read())
                                {
                                    int toplamCagri = Convert.ToInt32(GetSafeValue(cagriReader, "ToplamCagri", 0));
                                    int yuksekOncelik = Convert.ToInt32(GetSafeValue(cagriReader, "YuksekOncelik", 0));
                                    int ortaOncelik = Convert.ToInt32(GetSafeValue(cagriReader, "OrtaOncelik", 0));
                                    int dusukOncelik = Convert.ToInt32(GetSafeValue(cagriReader, "DusukOncelik", 0));

                                    decimal toplamPrim = 0;
                                    if (toplamCagri >= minGorevSayisi)
                                    {
                                        toplamPrim = (yuksekOncelik * yuksekPrim) +
                                                     (ortaOncelik * ortaPrim) +
                                                     (dusukOncelik * dusukPrim);
                                    }

                                    cagriReader.Close();

                                    string kontrolQuery = @"
                                    SELECT ISNULL(COUNT(*), 0) FROM PrimKayit 
                                    WHERE KullaniciID = @KullaniciID AND Yil = @Yil AND Ay = @Ay";

                                    using (SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, baglanti.conn))
                                    {
                                        kontrolCmd.Parameters.AddWithValue("@KullaniciID", kullanici.KullaniciID);
                                        kontrolCmd.Parameters.AddWithValue("@Yil", currentYear);
                                        kontrolCmd.Parameters.AddWithValue("@Ay", currentMonth);

                                        object result = kontrolCmd.ExecuteScalar();
                                        int kayitSayisi = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);

                                        if (kayitSayisi == 0)
                                        {
                                            string insertQuery = @"
                                            INSERT INTO PrimKayit 
                                            (PrimAyarID, KullaniciID, Yil, Ay, ToplamCagriAdedi, YuksekOncelikCagriAdedi, OrtaOncelikCagriAdedi, 
                                             DusukOncelikCagriAdedi, PrimToplam, Maas, HesaplamaTarihi)
                                            VALUES 
                                            (@PrimAyarID, @KullaniciID, @Yil, @Ay, @ToplamCagriAdedi, @YuksekOncelikCagriAdedi, @OrtaOncelikCagriAdedi, 
                                             @DusukOncelikCagriAdedi, @PrimToplam, @Maas, GETDATE())";

                                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, baglanti.conn))
                                            {
                                                insertCmd.Parameters.AddWithValue("@PrimAyarID", primAyarID); 
                                                insertCmd.Parameters.AddWithValue("@KullaniciID", kullanici.KullaniciID);
                                                insertCmd.Parameters.AddWithValue("@Yil", currentYear);
                                                insertCmd.Parameters.AddWithValue("@Ay", currentMonth);
                                                insertCmd.Parameters.AddWithValue("@ToplamCagriAdedi", toplamCagri);
                                                insertCmd.Parameters.AddWithValue("@YuksekOncelikCagriAdedi", yuksekOncelik);
                                                insertCmd.Parameters.AddWithValue("@OrtaOncelikCagriAdedi", ortaOncelik);
                                                insertCmd.Parameters.AddWithValue("@DusukOncelikCagriAdedi", dusukOncelik);
                                                insertCmd.Parameters.AddWithValue("@PrimToplam", toplamPrim);
                                                insertCmd.Parameters.AddWithValue("@Maas", kullanici.Maas);
                                                insertCmd.ExecuteNonQuery();
                                            }
                                        }
                                        else
                                        {
                                            string updateQuery = @"
                                            UPDATE PrimKayit SET 
                                                PrimAyarID = @PrimAyarID,
                                                ToplamCagriAdedi = @ToplamCagriAdedi,
                                                YuksekOncelikCagriAdedi = @YuksekOncelikCagriAdedi,
                                                OrtaOncelikCagriAdedi = @OrtaOncelikCagriAdedi,
                                                DusukOncelikCagriAdedi = @DusukOncelikCagriAdedi,
                                                PrimToplam = @PrimToplam,
                                                Maas = @Maas,
                                                HesaplamaTarihi = GETDATE()
                                            WHERE KullaniciID = @KullaniciID AND Yil = @Yil AND Ay = @Ay";

                                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, baglanti.conn))
                                            {
                                                updateCmd.Parameters.AddWithValue("@PrimAyarID", primAyarID); 
                                                updateCmd.Parameters.AddWithValue("@KullaniciID", kullanici.KullaniciID);
                                                updateCmd.Parameters.AddWithValue("@Yil", currentYear);
                                                updateCmd.Parameters.AddWithValue("@Ay", currentMonth);
                                                updateCmd.Parameters.AddWithValue("@ToplamCagriAdedi", toplamCagri);
                                                updateCmd.Parameters.AddWithValue("@YuksekOncelikCagriAdedi", yuksekOncelik);
                                                updateCmd.Parameters.AddWithValue("@OrtaOncelikCagriAdedi", ortaOncelik);
                                                updateCmd.Parameters.AddWithValue("@DusukOncelikCagriAdedi", dusukOncelik);
                                                updateCmd.Parameters.AddWithValue("@PrimToplam", toplamPrim);
                                                updateCmd.Parameters.AddWithValue("@Maas", kullanici.Maas);
                                                updateCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Prim hesaplama hatası: {ex.Message}", "Hata");
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }
        private void cmbAyFiltre_SelectedIndexChanged(object sender, EventArgs e)
        {
            int secilenAy = cmbAyFiltre.SelectedIndex + 1;
            _logger.LogEkle("Filtreleme", "PrimKayit", $"Prim listesi {secilenAy}. ay için filtrelendi");
            KullaniciPrimleriniYukle();
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
            BossHomepage anasayfa = new BossHomepage();
            anasayfa.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            BossProfile profil = new BossProfile();
            profil.Show();
            this.Close();
        }

        private void btnKullaniciIslem_Click(object sender, EventArgs e)
        {
            BossUsersControl kullaniciIslem = new BossUsersControl();
            kullaniciIslem.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UsersSalary usersSalary = new UsersSalary();
            usersSalary.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            BossReports raporlar = new BossReports();
            raporlar.Show();
            this.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            UserInformation.BilgileriTemizle();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}