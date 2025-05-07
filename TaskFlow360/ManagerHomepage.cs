using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class ManagerHomepage : Form
    {
        Baglanti baglanti = new Baglanti();
        // Yönetici ID'sini KullaniciBilgi sınıfından alacağız
        private string yoneticiId;

        public ManagerHomepage()
        {
            InitializeComponent();
            SetupDataGridViewColumns();

            // Yönetici ID'sini KullaniciBilgi sınıfından alalım
            yoneticiId = KullaniciBilgi.KullaniciID;
            Debug.WriteLine("Yönetici ID konstruktörde set edildi: " + yoneticiId);
        }

        private void SetupDataGridViewColumns()
        {
            // Bekleyen Çağrılar DataGridView sütunları
            if (bekleyenCagrilarDGV.Columns.Count == 0)
            {
                bekleyenCagrilarDGV.Columns.Add("cagriId", "ID");
                bekleyenCagrilarDGV.Columns.Add("baslik", "Başlık");
                bekleyenCagrilarDGV.Columns.Add("kategori", "Kategori");
                bekleyenCagrilarDGV.Columns.Add("oncelik", "Öncelik");
                bekleyenCagrilarDGV.Columns.Add("durum", "Durum");
                bekleyenCagrilarDGV.Columns.Add("ataButon", "İşlem");

                // ID sütununu gizle
                bekleyenCagrilarDGV.Columns["cagriId"].Visible = false;
            }

            // Ekip Üyeleri DataGridView sütunları
            if (ekipUyeleriDGV.Columns.Count == 0)
            {
                ekipUyeleriDGV.Columns.Add("calisan", "Çalışan");
                ekipUyeleriDGV.Columns.Add("aktifGorev", "Aktif Görevler");
                ekipUyeleriDGV.Columns.Add("tamamlananGorev", "Tamamlanan Görevler");
                ekipUyeleriDGV.Columns.Add("aylikPerformans", "Aylık Performans");
                ekipUyeleriDGV.Columns.Add("ortalamaSure", "Ortalama Çözüm Süresi");
                ekipUyeleriDGV.Columns.Add("gorevAtaButon", "İşlem");
            }
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void ConfigureBekleyenCagrilarDGV()
        {
            // Temel seçim ayarları
            bekleyenCagrilarDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            bekleyenCagrilarDGV.DefaultCellStyle.SelectionBackColor = bekleyenCagrilarDGV.DefaultCellStyle.BackColor;
            bekleyenCagrilarDGV.DefaultCellStyle.SelectionForeColor = bekleyenCagrilarDGV.DefaultCellStyle.ForeColor;

            // Başlık ayarları
            bekleyenCagrilarDGV.EnableHeadersVisualStyles = false;
            bekleyenCagrilarDGV.ColumnHeadersDefaultCellStyle.SelectionBackColor = bekleyenCagrilarDGV.ColumnHeadersDefaultCellStyle.BackColor;
            bekleyenCagrilarDGV.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Transparent;

            // Görsel iyileştirmeler
            bekleyenCagrilarDGV.BorderStyle = BorderStyle.None;
            bekleyenCagrilarDGV.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            bekleyenCagrilarDGV.GridColor = Color.FromArgb(240, 240, 240);

            // Event bağlantıları
            bekleyenCagrilarDGV.CellClick += (s, e) => bekleyenCagrilarDGV.ClearSelection();
            bekleyenCagrilarDGV.DataBindingComplete += (s, e) => bekleyenCagrilarDGV.ClearSelection();
        }

        private void ConfigureEkipUyeleriDGV()
        {
            // Özel renk şeması
            ekipUyeleriDGV.DefaultCellStyle.BackColor = Color.White;
            ekipUyeleriDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            // Seçim efekti kaldırma
            ekipUyeleriDGV.SelectionMode = DataGridViewSelectionMode.CellSelect;
            ekipUyeleriDGV.DefaultCellStyle.SelectionBackColor = ekipUyeleriDGV.DefaultCellStyle.BackColor;
            ekipUyeleriDGV.DefaultCellStyle.SelectionForeColor = ekipUyeleriDGV.DefaultCellStyle.ForeColor;
        }

        private void ManagerHomepage_Load(object sender, EventArgs e)
        {
           
            // Debug mesajları ekleyelim
            Debug.WriteLine("Manager Homepage yükleniyor. Yönetici ID: " + yoneticiId);

            // Eğer yoneticiId boşsa, KullaniciBilgi'den tekrar almayı deneyelim
            if (string.IsNullOrEmpty(yoneticiId))
            {
                yoneticiId = KullaniciBilgi.KullaniciID;
                Debug.WriteLine("Yönetici ID Load event'te tekrar alındı: " + yoneticiId);

                // Hala boşsa, kullanıcıyı login formuna yönlendirelim
                if (string.IsNullOrEmpty(yoneticiId))
                {
                    MessageBox.Show("Oturum bilgileriniz geçersiz. Lütfen tekrar giriş yapın.", "Oturum Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                    return;
                }

            }

            ConfigureBekleyenCagrilarDGV();
            ConfigureEkipUyeleriDGV();

            // Veritabanı işlemlerini deneyelim
            try
            {
                BekleyenCagrilariYukle();
                EkipUyeleriniYukle();
                IstatistikleriGoster();
                Debug.WriteLine("Tüm veriler başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("Veri yükleme hatası: " + ex.ToString());
            }
        }

        private void BekleyenCagrilariYukle()
        {
            try
            {
                Debug.WriteLine("Bekleyen çağrılar yükleniyor...");
                baglanti.BaglantiAc();

                // Sorgunuzun doğruluğunu kontrol etmek için önce test edelim
                SqlCommand testKomut = new SqlCommand("SELECT COUNT(*) FROM Cagri WHERE Durum = 'Beklemede'", baglanti.conn);
                int sayac = Convert.ToInt32(testKomut.ExecuteScalar());
                Debug.WriteLine($"Bekleyen çağrı sayısı: {sayac}");

                // Asıl sorguyu çalıştıralım
                SqlCommand komut = new SqlCommand(
                    @"SELECT CagriID, Baslik, CagriKategori, CagriAciklama, Oncelik, Durum 
                    FROM Cagri 
                    WHERE Durum = 'Beklemede' AND AtananKullaniciID IS NULL", baglanti.conn);

                SqlDataReader dr = komut.ExecuteReader();
                bekleyenCagrilarDGV.Rows.Clear();

                int satirSayisi = 0;
                while (dr.Read())
                {
                    satirSayisi++;
                    int rowIndex = bekleyenCagrilarDGV.Rows.Add();
                    DataGridViewRow row = bekleyenCagrilarDGV.Rows[rowIndex];

                    row.Cells["cagriId"].Value = dr["CagriID"];
                    row.Cells["baslik"].Value = dr["Baslik"];
                    row.Cells["kategori"].Value = dr["CagriKategori"];
                    row.Cells["oncelik"].Value = dr["Oncelik"];
                    row.Cells["durum"].Value = dr["Durum"];
                    row.Cells["ataButon"].Value = "Ata";

                    // Renk ayarları
                    string oncelik = dr["Oncelik"].ToString();
                    if (oncelik == "Yüksek")
                        row.Cells["oncelik"].Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#f85c5c");
                    else if (oncelik == "Orta")
                        row.Cells["oncelik"].Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#f0ad4e");
                    else
                        row.Cells["oncelik"].Style.BackColor = System.Drawing.ColorTranslator.FromHtml("#63c966");

                    row.Cells["oncelik"].Style.ForeColor = System.Drawing.Color.White;
                }

                Debug.WriteLine($"Toplam {satirSayisi} adet bekleyen çağrı yüklendi.");
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bekleyen çağrılar yüklenirken hata oluştu: " + ex.Message, "Veri Yükleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("BekleyenCagrilariYukle hata detayı: " + ex.ToString());
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void EkipUyeleriniYukle()
        {
            try
            {
                Debug.WriteLine("Ekip üyeleri yükleniyor... Yönetici ID: " + yoneticiId);
                baglanti.BaglantiAc();

                // Test sorgusu
                SqlCommand testKomut = new SqlCommand(
                    "SELECT COUNT(*) FROM Kullanici WHERE Rol = '1' AND YoneticiID = @yoneticiId",
                    baglanti.conn);
                testKomut.Parameters.AddWithValue("@yoneticiId", yoneticiId);
                int uyeSayisi = Convert.ToInt32(testKomut.ExecuteScalar());
                Debug.WriteLine($"Yöneticiye bağlı ekip üyesi sayısı: {uyeSayisi}");

                // Ana sorgu - Rol değerini '1' (Ekip Üyesi) olarak güncelledim
                SqlCommand komut = new SqlCommand(
                    @"SELECT 
                    k.KullaniciID, 
                    k.Ad + ' ' + k.Soyad AS AdSoyad, 
                    COUNT(CASE WHEN c.Durum = 'Beklemede' OR c.Durum = 'Atandı' THEN 1 END) AS AktifGorevSayisi, 
                    COUNT(CASE WHEN c.Durum = 'Tamamlandı' THEN 1 END) AS TamamlananGorevSayisi,
                    AVG(CASE 
                        WHEN c.HedefSure IS NOT NULL THEN
                            TRY_CONVERT(float, REPLACE(REPLACE(REPLACE(c.HedefSure, ' Saat', ''), ' saat', ''), ',', '.'))
                        ELSE NULL
                    END) AS OrtalamaSure
                  FROM Kullanici k 
                  LEFT JOIN Cagri c ON k.KullaniciID = c.AtananKullaniciID 
                  WHERE k.Rol = '1' AND k.YoneticiID = @yoneticiId 
                  GROUP BY k.KullaniciID, k.Ad, k.Soyad",
                    baglanti.conn);

                komut.Parameters.AddWithValue("@yoneticiId", yoneticiId);
                SqlDataReader dr = komut.ExecuteReader();

                ekipUyeleriDGV.Rows.Clear();
                int satirSayisi = 0;

                while (dr.Read())
                {
                    satirSayisi++;
                    int rowIndex = ekipUyeleriDGV.Rows.Add();
                    DataGridViewRow row = ekipUyeleriDGV.Rows[rowIndex];

                    int kullaniciID = Convert.ToInt32(dr["KullaniciID"]);
                    row.Cells["calisan"].Value = dr["AdSoyad"];
                    row.Cells["aktifGorev"].Value = dr["AktifGorevSayisi"];
                    row.Cells["tamamlananGorev"].Value = dr["TamamlananGorevSayisi"];

                    // Hesaplanan performans yüzdesi
                    int tamamlananGorevSayisi = Convert.ToInt32(dr["TamamlananGorevSayisi"]);
                    int aktifGorevSayisi = Convert.ToInt32(dr["AktifGorevSayisi"]);
                    int toplamGorevSayisi = tamamlananGorevSayisi + aktifGorevSayisi;
                    int performansYuzdesi = toplamGorevSayisi > 0 ? (tamamlananGorevSayisi * 100) / toplamGorevSayisi : 0;

                    row.Cells["aylikPerformans"].Value = performansYuzdesi.ToString() + "%";

                    if (!dr.IsDBNull(dr.GetOrdinal("OrtalamaSure")))
                    {
                        double sure = Convert.ToDouble(dr["OrtalamaSure"]);
                        row.Cells["ortalamaSure"].Value = $"{Math.Round(sure, 1)} saat";
                    }
                    else
                    {
                        row.Cells["ortalamaSure"].Value = "Veri yok";
                    }

                    row.Cells["gorevAtaButon"].Value = "Görev Ata";
                    row.Tag = kullaniciID;
                }

                Debug.WriteLine($"Toplam {satirSayisi} adet ekip üyesi yüklendi.");
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekip üyeleri yüklenirken hata oluştu: " + ex.Message, "Veri Yükleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("EkipUyeleriniYukle hata detayı: " + ex.ToString());
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void IstatistikleriGoster()
        {
            try
            {
                baglanti.BaglantiAc();

                // Toplam bekleyen çağrı sayısı
                SqlCommand bekleyenCagri = new SqlCommand(
                    "SELECT COUNT(*) FROM Cagri WHERE Durum = 'Beklemede'", baglanti.conn);
                int bekleyenCagriSayisi = Convert.ToInt32(bekleyenCagri.ExecuteScalar());
                lblBeklemede.Text = bekleyenCagriSayisi.ToString();

                SqlCommand atananCagri = new SqlCommand(
                    "SELECT COUNT(*) FROM Cagri WHERE Durum = 'Atandı'", baglanti.conn);
                int atananCagriSayisi = Convert.ToInt32(atananCagri.ExecuteScalar());
                lblAtanan.Text = atananCagriSayisi.ToString();

                // Toplam tamamlanan çağrı sayısı (Son 30 gün)
                SqlCommand tamamlananCagri = new SqlCommand(
                    "SELECT COUNT(*) FROM Cagri WHERE Durum = 'Tamamlandı' AND TeslimTarihi >= DATEADD(day, -30, GETDATE())", baglanti.conn);
                int tamamlananCagriSayisi = Convert.ToInt32(tamamlananCagri.ExecuteScalar());
                lblTamamlanan.Text = tamamlananCagriSayisi.ToString();

                // Geciken çağrı sayısı - HedefSure için düzeltilmiş sorgu
                SqlCommand gecikenCagri = new SqlCommand(@"
                    SELECT COUNT(*) FROM Cagri 
                    WHERE Durum != 'Tamamlandı' 
                    AND TRY_CONVERT(float, REPLACE(REPLACE(REPLACE(HedefSure, ' Saat', ''), ' saat', ''), ',', '.')) < 
                        DATEDIFF(hour, OlusturmaTarihi, GETDATE())", baglanti.conn);
                int gecikenCagriSayisi = Convert.ToInt32(gecikenCagri.ExecuteScalar());
                lblGeciken.Text = gecikenCagriSayisi.ToString();

                // Ekip üyesi sayısı - Rol değerini '1' (Ekip Üyesi) olarak güncelledim
                SqlCommand ekipUyesiSayisi = new SqlCommand(
                    "SELECT COUNT(*) FROM Kullanici WHERE Rol = '1' AND YoneticiID = @yoneticiId", baglanti.conn);
                ekipUyesiSayisi.Parameters.AddWithValue("@yoneticiId", yoneticiId);
                int toplamEkipUyesiSayisi = Convert.ToInt32(ekipUyesiSayisi.ExecuteScalar());
                // Eğer lblEkipUyesi kontrolü bulunuyorsa
                // lblEkipUyesi.Text = toplamEkipUyesiSayisi.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler yüklenirken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void bekleyenCagrilarDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // "Ata" butonuna tıklandığında görev atama işlemi
            if (e.ColumnIndex == bekleyenCagrilarDGV.Columns["ataButon"].Index && e.RowIndex >= 0)
            {
                int cagriId = Convert.ToInt32(bekleyenCagrilarDGV.Rows[e.RowIndex].Cells["cagriId"].Value);
                string baslik = bekleyenCagrilarDGV.Rows[e.RowIndex].Cells["baslik"].Value.ToString();

                // Çağrı atama formunu aç
                MessageBox.Show($"Çağrı ID: {cagriId}, Başlık: {baslik} için atama işlemi yapılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Form sınıfı aktif değilse yorum satırını kaldırabilirsiniz
                // CagriAtamaForm cagriAtamaForm = new CagriAtamaForm(cagriId, baslik, yoneticiId);
                // if (cagriAtamaForm.ShowDialog() == DialogResult.OK)
                // {
                //     // Atama başarılı olduğunda tabloları yenile
                //     BekleyenCagrilariYukle();
                //     EkipUyeleriniYukle();
                //     IstatistikleriGoster();
                // }
            }
        }

        private void ekipUyeleriDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // "Görev Ata" butonuna tıklandığında çalışana görev atama işlemi
            if (e.ColumnIndex == ekipUyeleriDGV.Columns["gorevAtaButon"].Index && e.RowIndex >= 0)
            {
                int kullaniciId = (int)ekipUyeleriDGV.Rows[e.RowIndex].Tag;
                string calisan = ekipUyeleriDGV.Rows[e.RowIndex].Cells["calisan"].Value.ToString();

                // Çalışana görev atama formu
                MessageBox.Show($"Kullanıcı ID: {kullaniciId}, Çalışan: {calisan} için görev atama işlemi yapılacak.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Form sınıfı aktif değilse yorum satırını kaldırabilirsiniz
                // CalisanaGorevAtaForm gorevAtaForm = new CalisanaGorevAtaForm(kullaniciId, calisan);
                // if (gorevAtaForm.ShowDialog() == DialogResult.OK)
                // {
                //     // Atama başarılı olduğunda tabloları yenile
                //     BekleyenCagrilariYukle();
                //     EkipUyeleriniYukle();
                //     IstatistikleriGoster();
                // }
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
            // Zaten anasayfadayız, yeniden yükle
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            ManagerTasks managerTasks = new ManagerTasks();
            managerTasks.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            ManagerProfile managerProfile = new ManagerProfile();
            managerProfile.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            // Form sınıfı aktif değilse yorum satırını kaldırabilirsiniz
            // ManagerReports managerReports = new ManagerReports(yoneticiId);
            // managerReports.Show();
            // this.Close();
            MessageBox.Show("Raporlar ekranı henüz aktif değil.");
        }

        private void btnEkipYonetimi_Click(object sender, EventArgs e)
        {
            ManagerAdminPage managerAdminPage = new ManagerAdminPage();
            managerAdminPage.Show();
            this.Close();
        }
    }
}