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

namespace TaskFlow360
{
    public partial class UsersSalary : Form
    {
        // Müdür ID'sini formda tanımlamalısın
        private string GirisYapanMudurID;
        Baglanti baglanti = new Baglanti();
        public UsersSalary()
        {
            InitializeComponent();
            GirisYapanMudurID = KullaniciBilgi.KullaniciID;
        }
        private void UsersSalary_Load(object sender, EventArgs e)
        {
            KullaniciVerileriniYukle();
            PrimAyarGetir();
        }
        private void KullaniciVerileriniYukle()
        {
            Baglanti baglanti = new Baglanti();
            DataTable dt = new DataTable();
            string query = @"
    WITH CagriVeri AS (
        SELECT 
            K.KullaniciID,
            K.Ad + ' ' + K.Soyad AS AdSoyad,
            K.Maas,
            COUNT(C.CagriID) AS ToplamCagri,
            SUM(
                CASE 
                    WHEN C.Oncelik = 'Yüksek' THEN PA.Yüksek
                    WHEN C.Oncelik = 'Orta' THEN PA.Orta
                    WHEN C.Oncelik = 'Düşük' THEN PA.Düşük
                    ELSE 0
                END
            ) AS HesaplananPrim,
            PA.MinGorevSayisi
        FROM Kullanici K
        LEFT JOIN Cagri C ON K.KullaniciID = C.AtananKullaniciID AND C.Durum = 'Tamamlandı'
        CROSS APPLY (
            SELECT TOP 1 * FROM PrimAyar ORDER BY EklemeTarihi DESC
        ) AS PA
        GROUP BY K.KullaniciID, K.Ad, K.Soyad, K.Maas, PA.MinGorevSayisi
    )
    SELECT 
        KullaniciID,
        AdSoyad,
        Maas,
        ToplamCagri,
        CASE 
            WHEN ToplamCagri >= MinGorevSayisi THEN ISNULL(HesaplananPrim, 0)
            ELSE 0
        END AS Prim,
        Maas + 
        CASE 
            WHEN ToplamCagri >= MinGorevSayisi THEN ISNULL(HesaplananPrim, 0)
            ELSE 0
        END AS ToplamMaas
    FROM CagriVeri
    ";

            try
            {
                baglanti.BaglantiAc();

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@MudurID", GirisYapanMudurID); // Müdürün ID'sini sen formda ayarlarsın

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    dataGridViewKullanicilar.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }


        private (int minGorev, decimal yuksek, decimal orta, decimal dusuk) PrimAyarGetir()
        {
            Baglanti baglanti = new Baglanti();

            string query = "SELECT TOP 1 * FROM PrimAyar ORDER BY EklemeTarihi DESC";

            try
            {
                baglanti.BaglantiAc();
                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtDusuk.Text = reader["Düşük"].ToString();
                        txtOrta.Text = reader["Orta"].ToString();
                        txtYuksek.Text = reader["Yüksek"].ToString();
                        txtMinGorevSayisi.Text = reader["MinGorevSayisi"].ToString();
                     
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Prim ayarları alınamadı: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }

            return (0, 0, 0, 0);
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

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            // Doğrulama
            if (!int.TryParse(txtMinGorevSayisi.Text, out int minGorevSayisi))
            {
                MessageBox.Show("Minimum görev sayısı geçerli bir sayı olmalı.");
                return;
            }

            if (!decimal.TryParse(txtYuksek.Text, out decimal yuksekPrim) ||
                !decimal.TryParse(txtOrta.Text, out decimal ortaPrim) ||
                !decimal.TryParse(txtDusuk.Text, out decimal dusukPrim))
            {
                MessageBox.Show("Prim miktarları geçerli sayılar olmalı.");
                return;
            }

            Baglanti baglanti = new Baglanti();

            string query = @"
                INSERT INTO PrimAyar (MinGorevSayisi, Yüksek, Orta, Düşük)
                VALUES (@MinGorevSayisi, @Yuksek, @Orta, @Dusuk);
            ";

            try
            {
                baglanti.BaglantiAc();

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@MinGorevSayisi", minGorevSayisi);
                    cmd.Parameters.AddWithValue("@Yuksek", yuksekPrim);
                    cmd.Parameters.AddWithValue("@Orta", ortaPrim);
                    cmd.Parameters.AddWithValue("@Dusuk", dusukPrim);

                    int sonuc = cmd.ExecuteNonQuery();

                    if (sonuc > 0)
                    {
                        MessageBox.Show("Kayıt başarıyla eklendi.");
                        KullaniciVerileriniYukle();
                    }

                    else
                        MessageBox.Show("Kayıt eklenemedi.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }

           
        }

        private void PrimleriHesaplaVeKaydet()
        {
            try
            {
                baglanti.BaglantiAc();

                // 1. Prim ayarlarını al
                decimal yuksekPrim = 0, ortaPrim = 0, dusukPrim = 0;
                int minGorevSayisi = 0;

                SqlCommand primCmd = new SqlCommand("SELECT TOP 1 * FROM PrimAyar ORDER BY AyarID DESC", baglanti.conn);
                SqlDataReader primReader = primCmd.ExecuteReader();
                if (primReader.Read())
                {
                    yuksekPrim = Convert.ToDecimal(primReader["Yüksek"]);
                    ortaPrim = Convert.ToDecimal(primReader["Orta"]);
                    dusukPrim = Convert.ToDecimal(primReader["Düşük"]);
                    minGorevSayisi = Convert.ToInt32(primReader["MinGorevSayisi"]);
                }
                primReader.Close();

                // 2. Tarih aralığını belirle (aylık prim için)
                DateTime baslangic = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime bitis = baslangic.AddMonths(1).AddDays(-1);

                // 3. Personel görev ve çözüm süresi bilgilerini al
                string query = @"
            WITH GorevBilgileri AS (
                SELECT
                    C.CagriID,
                    C.AtananKullaniciID,
                    C.CozulmeSuresi
                FROM Cagri C
                WHERE C.CagriDurum = 'Tamamlandı' AND C.CozulmeTarihi BETWEEN @Baslangic AND @Bitis
            )
            SELECT
                K.KullaniciID,
                K.AdSoyad,
                COUNT(G.CagriID) AS ToplamGorev,
                AVG(CAST(G.CozulmeSuresi AS FLOAT)) AS OrtalamaCozumSuresi
            FROM Kullanici K
            LEFT JOIN GorevBilgileri G ON K.KullaniciID = G.AtananKullaniciID
            WHERE K.Rol = 'Personel'
            GROUP BY K.KullaniciID, K.AdSoyad";

                SqlCommand komut = new SqlCommand(query, baglanti.conn);
                komut.Parameters.AddWithValue("@Baslangic", baslangic);
                komut.Parameters.AddWithValue("@Bitis", bitis);

                SqlDataReader reader = komut.ExecuteReader();

                while (reader.Read())
                {
                    int kullaniciID = Convert.ToInt32(reader["KullaniciID"]);
                    int toplamGorev = reader["ToplamGorev"] != DBNull.Value ? Convert.ToInt32(reader["ToplamGorev"]) : 0;
                    double ortalamaCozum = reader["OrtalamaCozumSuresi"] != DBNull.Value ? Convert.ToDouble(reader["OrtalamaCozumSuresi"]) : 0;

                    decimal primMiktari = 0;

                    if (toplamGorev >= minGorevSayisi)
                    {
                        if (ortalamaCozum < 2)
                            primMiktari = yuksekPrim;
                        else if (ortalamaCozum >= 2 && ortalamaCozum <= 5)
                            primMiktari = ortaPrim;
                        else
                            primMiktari = dusukPrim;
                    }

                    // 4. Prim kaydını veritabanına ekle
                    SqlCommand ekleKomut = new SqlCommand(@"
                INSERT INTO Primler (KullaniciID, ToplamGorev, OrtalamaCozumSuresi, PrimMiktari, Donem)
                VALUES (@KullaniciID, @ToplamGorev, @OrtalamaCozumSuresi, @PrimMiktari, @Donem)", baglanti.conn);

                    ekleKomut.Parameters.AddWithValue("@KullaniciID", kullaniciID);
                    ekleKomut.Parameters.AddWithValue("@ToplamGorev", toplamGorev);
                    ekleKomut.Parameters.AddWithValue("@OrtalamaCozumSuresi", ortalamaCozum);
                    ekleKomut.Parameters.AddWithValue("@PrimMiktari", primMiktari);
                    ekleKomut.Parameters.AddWithValue("@Donem", baslangic.ToString("yyyy-MM"));

                    ekleKomut.ExecuteNonQuery();
                }

                reader.Close();
                baglanti.BaglantiKapat();

                MessageBox.Show("Primler başarıyla hesaplandı ve kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                baglanti.BaglantiKapat();
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                PrimleriHesaplaVeKaydet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Prim hesaplama hatası: " + ex.Message);
            }
        }
    }
}
