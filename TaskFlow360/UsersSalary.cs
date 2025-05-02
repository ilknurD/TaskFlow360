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
        Baglanti baglanti = new Baglanti();
        Kullanici kullanici = new Kullanici();

        public UsersSalary()
        {
            InitializeComponent();
            GirisYapanMudurID = KullaniciBilgi.KullaniciID;
        }

        private void UsersSalary_Load(object sender, EventArgs e)
        {
            AyComboBoxDoldur();
            DataGridViewKolonlariAyarla();
            KullaniciPrimleriniYukle();
            PrimAyarlariniYukle();
            stil();
        }

        private void DataGridViewKolonlariAyarla()
        {
            dataGridViewKullanicilar.Columns.Clear();
            dataGridViewKullanicilar.AutoGenerateColumns = false;

            // Kolonları manuel olarak ekle
            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AdSoyad",
                HeaderText = "Ad Soyad",
                DataPropertyName = "AdSoyad",
                Width = 150
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonemBilgisi",
                HeaderText = "Dönem",
                DataPropertyName = "DonemBilgisi",
                Visible = false, // Dönem bilgisi başlangıçta görünmesin
                Width = 100
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ToplamGorev",
                HeaderText = "Toplam Çağrı",
                DataPropertyName = "ToplamGorev",
                Width = 100
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "YuksekOncelikGorev",
                HeaderText = "Yüksek Öncelik",
                DataPropertyName = "YuksekOncelikGorev",
                Width = 120
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OrtaOncelikGorev",
                HeaderText = "Orta Öncelik",
                DataPropertyName = "OrtaOncelikGorev",
                Width = 120
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DusukOncelikGorev",
                HeaderText = "Düşük Öncelik",
                DataPropertyName = "DusukOncelikGorev",
                Width = 120
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
                Name = "TemelMaas",
                HeaderText = "Temel Maaş",
                DataPropertyName = "TemelMaas",
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
            cmbAyFiltre.Items.Add("Tüm Aylar");

            string[] aylar = { "Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran",
                              "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık" };

            for (int i = 0; i < aylar.Length; i++)
            {
                cmbAyFiltre.Items.Add($"{i + 1:00} - {aylar[i]}");
            }

            int suankiAy = DateTime.Now.Month;
            cmbAyFiltre.SelectedIndex = suankiAy;
        }

        private void stil()
        {
            dataGridViewKullanicilar.AllowUserToAddRows = false;
            dataGridViewKullanicilar.AllowUserToDeleteRows = false;
            dataGridViewKullanicilar.ReadOnly = true;
            dataGridViewKullanicilar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewKullanicilar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewKullanicilar.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridViewKullanicilar.ScrollBars = ScrollBars.Both;

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

            dataGridViewKullanicilar.RowHeadersVisible = false;
            dataGridViewKullanicilar.BorderStyle = BorderStyle.None;
            dataGridViewKullanicilar.GridColor = Color.FromArgb(230, 230, 250);
            dataGridViewKullanicilar.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 240, 255);
        }

        private void PrimAyarlariniYukle()
        {
            try
            {
                baglanti.BaglantiAc();

                // Mevcut ay için prim ayarı var mı kontrol et
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;

                string query = @"
                    SELECT pa.* FROM PrimAyar pa
                    WHERE YEAR(pa.EklemeTarihi) = @CurrentYear 
                      AND MONTH(pa.EklemeTarihi) = @CurrentMonth
                    ORDER BY pa.EklemeTarihi DESC";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@CurrentYear", currentYear);
                    cmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Mevcut ay için ayarlar var
                            txtMinGorevSayisi.Text = reader["MinGorevSayisi"].ToString();
                            txtYuksek.Text = reader["Yüksek"].ToString();
                            txtOrta.Text = reader["Orta"].ToString();
                            txtDusuk.Text = reader["Düşük"].ToString();
                        }
                        else
                        {
                            // Mevcut ay için ayar yok, en son ayarları getir
                            reader.Close();

                            string lastQuery = "SELECT TOP 1 * FROM PrimAyar ORDER BY EklemeTarihi DESC";
                            using (SqlCommand lastCmd = new SqlCommand(lastQuery, baglanti.conn))
                            using (SqlDataReader lastReader = lastCmd.ExecuteReader())
                            {
                                if (lastReader.Read())
                                {
                                    txtMinGorevSayisi.Text = lastReader["MinGorevSayisi"].ToString();
                                    txtYuksek.Text = lastReader["Yüksek"].ToString();
                                    txtOrta.Text = lastReader["Orta"].ToString();
                                    txtDusuk.Text = lastReader["Düşük"].ToString();
                                }
                                else
                                {
                                    // Hiç ayar yoksa varsayılan değerler
                                    txtMinGorevSayisi.Text = "5";
                                    txtYuksek.Text = "100";
                                    txtOrta.Text = "75";
                                    txtDusuk.Text = "50";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Prim ayarları yüklenirken hata: " + ex.Message);
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
                int secilenAy = cmbAyFiltre.SelectedIndex > 0 ? cmbAyFiltre.SelectedIndex : 0;
                int guncelYil = DateTime.Now.Year;

                string query = @"
                WITH CagriOzet AS (
                    SELECT 
                        AtananKullaniciID,
                        COUNT(*) AS ToplamCagriAdedi,
                        SUM(CASE WHEN Oncelik = 'Yüksek' THEN 1 ELSE 0 END) AS YuksekOncelikCagriAdedi,
                        SUM(CASE WHEN Oncelik = 'Orta' THEN 1 ELSE 0 END) AS OrtaOncelikCagriAdedi,
                        SUM(CASE WHEN Oncelik = 'Düşük' THEN 1 ELSE 0 END) AS DusukOncelikCagriAdedi
                    FROM [TaskFlow360].[dbo].[Cagri]
                    WHERE Durum = 'Tamamlandı' 
                      AND YEAR(TeslimTarihi) = @Yil
                      AND (@SecilenAy = 0 OR MONTH(TeslimTarihi) = @SecilenAy)
                      AND TeslimTarihi IS NOT NULL
                    GROUP BY AtananKullaniciID
                )
                SELECT 
                    K.KullaniciID,
                    K.Ad + ' ' + K.Soyad AS AdSoyad,
                    K.Maas AS TemelMaas,
                    COALESCE(co.ToplamCagriAdedi, 0) AS ToplamGorev,
                    COALESCE(co.YuksekOncelikCagriAdedi, 0) AS YuksekOncelikGorev,
                    COALESCE(co.OrtaOncelikCagriAdedi, 0) AS OrtaOncelikGorev,
                    COALESCE(co.DusukOncelikCagriAdedi, 0) AS DusukOncelikGorev,
                    COALESCE(pk.PrimToplam, 0) AS ToplamPrim,
                    COALESCE(K.Maas + pk.PrimToplam, K.Maas) AS ToplamOdeme,
                    CASE 
                        WHEN pk.Ay IS NOT NULL AND pk.Yil IS NOT NULL 
                        THEN CAST(pk.Yil AS VARCHAR(4)) + '/' + FORMAT(pk.Ay, '00')
                        ELSE 'Hesaplanmamış'
                    END AS DonemBilgisi
                FROM [TaskFlow360].[dbo].[Kullanici] K
                LEFT JOIN [TaskFlow360].[dbo].[PrimKayit] pk ON K.KullaniciID = pk.KullaniciID 
                    AND pk.Yil = @Yil
                    AND (@SecilenAy = 0 OR pk.Ay = @SecilenAy)
                LEFT JOIN CagriOzet co ON K.KullaniciID = co.AtananKullaniciID
                ORDER BY K.Ad, K.Soyad";

                baglanti.BaglantiAc();
                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@Yil", guncelYil);
                    cmd.Parameters.AddWithValue("@SecilenAy", secilenAy);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewKullanicilar.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı verileri yüklenirken hata: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMinGorevSayisi.Text) ||
                string.IsNullOrEmpty(txtYuksek.Text) ||
                string.IsNullOrEmpty(txtOrta.Text) ||
                string.IsNullOrEmpty(txtDusuk.Text))
            {
                MessageBox.Show("Lütfen tüm prim ayarlarını doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Sayısal değer kontrolleri
            if (!int.TryParse(txtMinGorevSayisi.Text, out int minGorev) || minGorev < 0)
            {
                MessageBox.Show("Minimum görev sayısı geçerli bir pozitif sayı olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtYuksek.Text, out decimal yuksekPrim) || yuksekPrim < 0)
            {
                MessageBox.Show("Yüksek öncelik prim miktarı geçerli bir pozitif sayı olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtOrta.Text, out decimal ortaPrim) || ortaPrim < 0)
            {
                MessageBox.Show("Orta öncelik prim miktarı geçerli bir pozitif sayı olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtDusuk.Text, out decimal dusukPrim) || dusukPrim < 0)
            {
                MessageBox.Show("Düşük öncelik prim miktarı geçerli bir pozitif sayı olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                AylikPrimleriHesapla();
                KullaniciPrimleriniYukle();
                MessageBox.Show("Prim ayarları başarıyla kaydedildi ve primler hesaplandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrimAyarlariKaydet()
        {
            try
            {
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;

                baglanti.BaglantiAc();

                // Mevcut ay için prim ayarı var mı kontrol et (sadece yıl-ay bazında)
                string checkQuery = @"
            SELECT AyarID FROM PrimAyar 
            WHERE YEAR(EklemeTarihi) = @CurrentYear 
              AND MONTH(EklemeTarihi) = @CurrentMonth";

                int existingAyarID = 0;
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, baglanti.conn))
                {
                    checkCmd.Parameters.AddWithValue("@CurrentYear", currentYear);
                    checkCmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);

                    object result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        existingAyarID = Convert.ToInt32(result);
                    }
                }

                if (existingAyarID > 0)
                {
                    // Mevcut ay için güncelleme yap
                    string updateQuery = @"
                UPDATE PrimAyar 
                SET 
                    MinGorevSayisi = @MinGorev, 
                    [Yüksek] = @Yuksek, 
                    Orta = @Orta, 
                    [Düşük] = @Dusuk,
                    EklemeTarihi = GETDATE()
                WHERE AyarID = @AyarID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, baglanti.conn))
                    {
                        cmd.Parameters.AddWithValue("@MinGorev", Convert.ToInt32(txtMinGorevSayisi.Text));
                        cmd.Parameters.AddWithValue("@Yuksek", Convert.ToDecimal(txtYuksek.Text));
                        cmd.Parameters.AddWithValue("@Orta", Convert.ToDecimal(txtOrta.Text));
                        cmd.Parameters.AddWithValue("@Dusuk", Convert.ToDecimal(txtDusuk.Text));
                        cmd.Parameters.AddWithValue("@AyarID", existingAyarID);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Prim ayarları başarıyla güncellendi.", "Bilgi",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Yeni kayıt ekle
                    string insertQuery = @"
                INSERT INTO PrimAyar 
                    (MinGorevSayisi, [Yüksek], Orta, [Düşük], EklemeTarihi)
                VALUES 
                    (@MinGorev, @Yuksek, @Orta, @Dusuk, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, baglanti.conn))
                    {
                        cmd.Parameters.AddWithValue("@MinGorev", Convert.ToInt32(txtMinGorevSayisi.Text));
                        cmd.Parameters.AddWithValue("@Yuksek", Convert.ToDecimal(txtYuksek.Text));
                        cmd.Parameters.AddWithValue("@Orta", Convert.ToDecimal(txtOrta.Text));
                        cmd.Parameters.AddWithValue("@Dusuk", Convert.ToDecimal(txtDusuk.Text));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Prim ayarları başarıyla kaydedildi.", "Bilgi",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Prim ayarları kaydedilirken hata: {ex.Message}", "Hata",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void AylikPrimleriHesapla()
        {
            try
            {
                baglanti.BaglantiAc();
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;

                // Mevcut ay için prim ayarlarını al
                string primAyarQuery = @"
            SELECT TOP 1 * FROM PrimAyar 
            WHERE YEAR(EklemeTarihi) = @CurrentYear 
              AND MONTH(EklemeTarihi) = @CurrentMonth
            ORDER BY EklemeTarihi DESC";

                int minGorevSayisi = 0;
                decimal yuksek = 0, orta = 0, dusuk = 0;
                int primAyarID = 0;

                using (SqlCommand cmd = new SqlCommand(primAyarQuery, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@CurrentYear", currentYear);
                    cmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            primAyarID = Convert.ToInt32(reader["AyarID"]);
                            minGorevSayisi = Convert.ToInt32(reader["MinGorevSayisi"]);
                            yuksek = Convert.ToDecimal(reader["Yüksek"]);
                            orta = Convert.ToDecimal(reader["Orta"]);
                            dusuk = Convert.ToDecimal(reader["Düşük"]);
                        }
                        else
                        {
                            throw new Exception("Prim ayarları bulunamadı. Lütfen önce prim ayarlarını kaydedin.");
                        }
                    }
                }

                // TÜM AKTİF KULLANICILARI AL (Adminler hariç)
                string tumKullanicilarQuery = @"
            SELECT KullaniciID, Maas 
            FROM Kullanici 
            WHERE Rol != 'Admin' AND Aktif = 1";

                List<Kullanici> tumKullanicilar = new List<Kullanici>();

                using (SqlCommand cmd = new SqlCommand(tumKullanicilarQuery, baglanti.conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tumKullanicilar.Add(new Kullanici
                            {
                                KullaniciID = Convert.ToInt32(reader["KullaniciID"]),
                                Maas = reader["Maas"].ToString() // Maas string olarak tanımlandığı için
                            });
                        }
                    }
                }

                // Her kullanıcı için prim hesaplama
                int islemYapilanKullanici = 0;

                foreach (var kullanici in tumKullanicilar)
                {
                    decimal maasDecimal = decimal.Parse(kullanici.Maas); // String maas'ı decimal'e çevir

                    // Kullanıcının tamamladığı çağrıları al
                    string cagriQuery = @"
                SELECT 
                    SUM(CASE WHEN c.Oncelik = 'Yüksek' THEN 1 ELSE 0 END) AS YuksekAdet,
                    SUM(CASE WHEN c.Oncelik = 'Orta' THEN 1 ELSE 0 END) AS OrtaAdet,
                    SUM(CASE WHEN c.Oncelik = 'Düşük' THEN 1 ELSE 0 END) AS DusukAdet,
                    COUNT(*) AS ToplamAdet
                FROM Cagri c
                WHERE c.AtananKullaniciID = @KullaniciID
                  AND c.Durum = 'Tamamlandı' 
                  AND c.TeslimTarihi IS NOT NULL
                  AND YEAR(c.TeslimTarihi) = @CurrentYear
                  AND MONTH(c.TeslimTarihi) = @CurrentMonth";

                    int yuksekAdet = 0, ortaAdet = 0, dusukAdet = 0, toplamAdet = 0;

                    using (SqlCommand cmd = new SqlCommand(cagriQuery, baglanti.conn))
                    {
                        cmd.Parameters.AddWithValue("@KullaniciID", kullanici.KullaniciID);
                        cmd.Parameters.AddWithValue("@CurrentYear", currentYear);
                        cmd.Parameters.AddWithValue("@CurrentMonth", currentMonth);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                yuksekAdet = Convert.ToInt32(reader["YuksekAdet"]);
                                ortaAdet = Convert.ToInt32(reader["OrtaAdet"]);
                                dusukAdet = Convert.ToInt32(reader["DusukAdet"]);
                                toplamAdet = Convert.ToInt32(reader["ToplamAdet"]);
                            }
                        }
                    }

                    // Prim hesaplama (minimum görev sayısını karşılayanlar için)
                    decimal primToplam = 0;
                    if (toplamAdet >= minGorevSayisi)
                    {
                        primToplam = (yuksekAdet * yuksek) + (ortaAdet * orta) + (dusukAdet * dusuk);
                    }

                    decimal toplamOdeme = maasDecimal + primToplam;

                    // Mevcut kayıt var mı kontrol et
                    string kontrolQuery = @"
                SELECT COUNT(*) FROM PrimKayit 
                WHERE KullaniciID = @KullaniciID AND Ay = @Ay AND Yil = @Yil";

                    using (SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, baglanti.conn))
                    {
                        kontrolCmd.Parameters.AddWithValue("@KullaniciID", kullanici.KullaniciID);
                        kontrolCmd.Parameters.AddWithValue("@Ay", currentMonth);
                        kontrolCmd.Parameters.AddWithValue("@Yil", currentYear);

                        int kayitSayisi = (int)kontrolCmd.ExecuteScalar();

                        if (kayitSayisi == 0)
                        {
                            // Yeni kayıt ekle (tüm kullanıcılar için, prim 0 olsa bile)
                            string insertQuery = @"
                        INSERT INTO PrimKayit 
                            (KullaniciID, Yil, Ay, YuksekOncelikCagriAdedi, OrtaOncelikCagriAdedi, 
                             DusukOncelikCagriAdedi, PrimAyarID, ToplamCagriAdedi, PrimToplam, Maas, ToplamOdeme)
                        VALUES 
                            (@KullaniciID, @Yil, @Ay, @Yuksek, @Orta, @Dusuk, @PrimAyarID, @Toplam, @PrimToplam, @Maas, @ToplamOdeme)";

                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, baglanti.conn))
                            {
                                insertCmd.Parameters.AddWithValue("@KullaniciID", kullanici.KullaniciID);
                                insertCmd.Parameters.AddWithValue("@Yil", currentYear);
                                insertCmd.Parameters.AddWithValue("@Ay", currentMonth);
                                insertCmd.Parameters.AddWithValue("@Yuksek", yuksekAdet);
                                insertCmd.Parameters.AddWithValue("@Orta", ortaAdet);
                                insertCmd.Parameters.AddWithValue("@Dusuk", dusukAdet);
                                insertCmd.Parameters.AddWithValue("@PrimAyarID", primAyarID);
                                insertCmd.Parameters.AddWithValue("@Toplam", toplamAdet);
                                insertCmd.Parameters.AddWithValue("@PrimToplam", primToplam);
                                insertCmd.Parameters.AddWithValue("@Maas", maasDecimal);
                                insertCmd.Parameters.AddWithValue("@ToplamOdeme", toplamOdeme);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Mevcut kaydı güncelle (sadece güncel ay için)
                            string updateQuery = @"
                        UPDATE PrimKayit SET 
                            YuksekOncelikCagriAdedi = @Yuksek,
                            OrtaOncelikCagriAdedi = @Orta,
                            DusukOncelikCagriAdedi = @Dusuk,
                            ToplamCagriAdedi = @Toplam,
                            PrimToplam = @PrimToplam,
                            Maas = @Maas,
                            ToplamOdeme = @ToplamOdeme,
                            PrimAyarID = @PrimAyarID
                        WHERE KullaniciID = @KullaniciID AND Yil = @Yil AND Ay = @Ay";

                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, baglanti.conn))
                            {
                                updateCmd.Parameters.AddWithValue("@KullaniciID", kullanici.KullaniciID);
                                updateCmd.Parameters.AddWithValue("@Yil", currentYear);
                                updateCmd.Parameters.AddWithValue("@Ay", currentMonth);
                                updateCmd.Parameters.AddWithValue("@Yuksek", yuksekAdet);
                                updateCmd.Parameters.AddWithValue("@Orta", ortaAdet);
                                updateCmd.Parameters.AddWithValue("@Dusuk", dusukAdet);
                                updateCmd.Parameters.AddWithValue("@Toplam", toplamAdet);
                                updateCmd.Parameters.AddWithValue("@PrimToplam", primToplam);
                                updateCmd.Parameters.AddWithValue("@Maas", maasDecimal);
                                updateCmd.Parameters.AddWithValue("@ToplamOdeme", toplamOdeme);
                                updateCmd.Parameters.AddWithValue("@PrimAyarID", primAyarID);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        islemYapilanKullanici++;
                    }
                }

                MessageBox.Show($"Prim hesaplama işlemi tamamlandı.\n" +
                               $"İşlem yapılan kullanıcı sayısı: {islemYapilanKullanici}\n" +
                               $"Hesaplama dönemi: {currentMonth:00}/{currentYear}",
                               "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Prim hesaplama sırasında hata oluştu: {ex.Message}",
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.conn.State == ConnectionState.Open)
                {
                    baglanti.BaglantiKapat();
                }
            }
        }      

        private void cmbAyFiltre_SelectedIndexChanged(object sender, EventArgs e)
        {
            KullaniciPrimleriniYukle();
        }

        // Form navigasyon metodları
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
    }

}