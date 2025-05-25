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
        private string yoneticiId;

        public ManagerHomepage()
        {
            InitializeComponent();
            SetupDataGridViewColumns();

            yoneticiId = KullaniciBilgi.KullaniciID;
        }
        private void ManagerHomepage_Load(object sender, EventArgs e)
        {
            bekleyenCagrilarDGV.DefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Regular);
            bekleyenCagrilarDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);

            // Eğer yoneticiId boşsa, KullaniciBilgi'den tekrar almayı deneyelim
            if (string.IsNullOrEmpty(yoneticiId))
            {
                yoneticiId = KullaniciBilgi.KullaniciID;

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

            lblAdSoyad.Text = KullaniciBilgi.TamAd();
            lblHosgeldiniz.Text = $"Hoş Geldiniz, {KullaniciBilgi.TamAd()}";

            ConfigureBekleyenCagrilarDGV();
            ConfigureEkipUyeleriDGV();

            bekleyenCagrilarDGV.Visible = true;
            ekipUyeleriDGV.Visible = true;

            try
            {
                BekleyenCagrilariYukle();
                EkipUyeleriniYukle();
                IstatistikleriGoster();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridViewColumns()
        {
            if (bekleyenCagrilarDGV.Columns.Count == 0)
            {
                bekleyenCagrilarDGV.Columns.Add("cagriId", "ID");
                bekleyenCagrilarDGV.Columns.Add("baslik", "Başlık");
                bekleyenCagrilarDGV.Columns.Add("kategori", "Kategori");
                bekleyenCagrilarDGV.Columns.Add("oncelik", "Öncelik");
                bekleyenCagrilarDGV.Columns.Add("durum", "Durum");
                bekleyenCagrilarDGV.Columns.Add("ataButon", "İşlem");
                bekleyenCagrilarDGV.Columns["cagriId"].Visible = false;
            }

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

        private void ConfigureBekleyenCagrilarDGV()
        {

            bekleyenCagrilarDGV.DefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Regular);

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

            // Buton sütunu için düzenleme
            bekleyenCagrilarDGV.Columns["ataButon"].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(126, 87, 194);
            bekleyenCagrilarDGV.Columns["ataButon"].DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            bekleyenCagrilarDGV.Columns["ataButon"].DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(126, 87, 194);
            bekleyenCagrilarDGV.Columns["ataButon"].DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            bekleyenCagrilarDGV.Columns["ataButon"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Event bağlantıları
            bekleyenCagrilarDGV.CellClick += bekleyenCagrilarDGV_CellClick;
            bekleyenCagrilarDGV.CellFormatting += BekleyenCagrilarDGV_CellFormatting;
            bekleyenCagrilarDGV.DataBindingComplete += (s, e) => bekleyenCagrilarDGV.ClearSelection();
        }

        private void ConfigureEkipUyeleriDGV()
        {
            // Özel renk şeması
            ekipUyeleriDGV.DefaultCellStyle.BackColor = Color.White;
            ekipUyeleriDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            // Seçim efekti
            ekipUyeleriDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ekipUyeleriDGV.DefaultCellStyle.SelectionBackColor = ekipUyeleriDGV.DefaultCellStyle.BackColor;
            ekipUyeleriDGV.DefaultCellStyle.SelectionForeColor = ekipUyeleriDGV.DefaultCellStyle.ForeColor;

            // Başlık ayarları
            ekipUyeleriDGV.EnableHeadersVisualStyles = false;
            ekipUyeleriDGV.ColumnHeadersDefaultCellStyle.SelectionBackColor = ekipUyeleriDGV.ColumnHeadersDefaultCellStyle.BackColor;
            ekipUyeleriDGV.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Transparent;

            // Görsel iyileştirmeler
            ekipUyeleriDGV.BorderStyle = BorderStyle.None;
            ekipUyeleriDGV.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            ekipUyeleriDGV.GridColor = Color.FromArgb(240, 240, 240);
            

            // Event bağlantıları
            ekipUyeleriDGV.CellFormatting += EkipUyeleriDGV_CellFormatting;
            ekipUyeleriDGV.DataBindingComplete += (s, e) => ekipUyeleriDGV.ClearSelection();
        }
        private void BekleyenCagrilarDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Regular);
            if (e.ColumnIndex == bekleyenCagrilarDGV.Columns["ataButon"].Index && e.RowIndex >= 0)
            {
                e.CellStyle.BackColor = System.Drawing.Color.FromArgb(126, 87, 194);
                e.CellStyle.ForeColor = System.Drawing.Color.White;
            }

            if (e.ColumnIndex == bekleyenCagrilarDGV.Columns["oncelik"].Index && e.RowIndex >= 0 && e.Value != null)
            {
                string oncelik = e.Value.ToString();
                if (oncelik == "Yüksek")
                    e.CellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#f85c5c");
                else if (oncelik == "Orta")
                    e.CellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#f0ad4e");
                else
                    e.CellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#63c966");
            }

            if (bekleyenCagrilarDGV.Rows[e.RowIndex].Selected)
            {
                e.CellStyle.Font = new Font(bekleyenCagrilarDGV.Font, FontStyle.Bold);
            }
            else
            {
                e.CellStyle.Font = new Font(bekleyenCagrilarDGV.Font, FontStyle.Regular);
            }

            // Seçili hücre için renklendirme
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = bekleyenCagrilarDGV.Rows[e.RowIndex].Cells[e.ColumnIndex];

                // Önceden atanmış renklendirme varsa, seçili olunca da koru
                if (cell.OwningColumn.Name == "oncelik")
                {
                    string oncelik = cell.Value?.ToString();
                    if (oncelik == "Yüksek")
                    {
                        e.CellStyle.BackColor = ColorTranslator.FromHtml("#f85c5c");
                        e.CellStyle.SelectionBackColor = ColorTranslator.FromHtml("#f85c5c");
                    }
                    else if (oncelik == "Orta")
                    {
                        e.CellStyle.BackColor = ColorTranslator.FromHtml("#f0ad4e");
                        e.CellStyle.SelectionBackColor = ColorTranslator.FromHtml("#f0ad4e");
                    }
                    else
                    {
                        e.CellStyle.BackColor = ColorTranslator.FromHtml("#63c966");
                        e.CellStyle.SelectionBackColor = ColorTranslator.FromHtml("#63c966");
                    }
                }

                if (cell.OwningColumn.Name == "ataButon")
                {
                    e.CellStyle.BackColor = Color.FromArgb(126, 87, 194);
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.SelectionBackColor = Color.FromArgb(126, 87, 194);
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }

        }
        private void EkipUyeleriDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Regular);

            // Performans yüzdesini renklendirme
            if (e.ColumnIndex == ekipUyeleriDGV.Columns["aylikPerformans"].Index && e.Value != null && e.RowIndex >= 0)
            {
                string performansStr = e.Value.ToString();
                if (!string.IsNullOrEmpty(performansStr))
                {
                    int performans = int.Parse(performansStr.Replace("%", ""));

                    if (performans >= 80)
                        e.CellStyle.ForeColor = System.Drawing.Color.Green;
                    else if (performans >= 50)
                        e.CellStyle.ForeColor = System.Drawing.Color.Orange;
                    else
                        e.CellStyle.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
        private void BekleyenCagrilariYukle()
        {
            try
            {
                baglanti.BaglantiAc();

                string kontrolSorgusu = @"
            SELECT COUNT(*) FROM Cagri 
            WHERE Durum = 'Beklemede' 
            AND AtananKullaniciID = @managerID";  

                SqlCommand kontrolKomut = new SqlCommand(kontrolSorgusu, baglanti.conn);
                kontrolKomut.Parameters.AddWithValue("@managerID", yoneticiId);

                int cagriSayisi = Convert.ToInt32(kontrolKomut.ExecuteScalar());

                if (cagriSayisi == 0)
                {
                    // Veri olmadığında gösterilecek boş durum görünümü
                    bekleyenCagrilarDGV.Rows.Clear();

                    if (!PnlGorevler.Controls.ContainsKey("lblBosGorev"))
                    {
                        Label lblBosGorev = new Label();
                        lblBosGorev.Name = "lblBosGorev";
                        lblBosGorev.Text = "Bekleyen çağrı bulunmuyor.\nYeni çağrı eklemek için Çağrı Merkezi ile iletişime geçebilirsiniz.";
                        lblBosGorev.AutoSize = false;
                        lblBosGorev.Size = new Size(PnlGorevler.Width - 20, 60);
                        lblBosGorev.TextAlign = ContentAlignment.MiddleCenter;
                        lblBosGorev.Font = new Font(lblBosGorev.Font.FontFamily, 10, FontStyle.Regular);
                        lblBosGorev.ForeColor = Color.Gray;
                        lblBosGorev.Location = new Point(10, PnlGorevler.Height / 2 - 30);
                        PnlGorevler.Controls.Add(lblBosGorev);
                    }
                    bekleyenCagrilarDGV.Visible = false;
                    return;
                }
                else
                {
                    if (PnlGorevler.Controls.ContainsKey("lblBosGorev"))
                    {
                        PnlGorevler.Controls.RemoveByKey("lblBosGorev");
                    }
                    bekleyenCagrilarDGV.Visible = true;
                }
                string sorgu = @"
            SELECT CagriID, Baslik, CagriKategori, Oncelik, Durum 
            FROM Cagri 
            WHERE Durum = 'Beklemede'
            AND AtananKullaniciID = @managerID"; 

                SqlCommand komut = new SqlCommand(sorgu, baglanti.conn);
                komut.Parameters.AddWithValue("@managerID", yoneticiId);
                SqlDataReader dr = komut.ExecuteReader();

                bekleyenCagrilarDGV.Rows.Clear();

                while (dr.Read())
                {
                    int rowIndex = bekleyenCagrilarDGV.Rows.Add();
                    DataGridViewRow row = bekleyenCagrilarDGV.Rows[rowIndex];

                    row.Cells["cagriId"].Value = dr["CagriID"].ToString();
                    row.Cells["baslik"].Value = dr["Baslik"].ToString();
                    row.Cells["kategori"].Value = dr["CagriKategori"].ToString();
                    row.Cells["oncelik"].Value = dr["Oncelik"].ToString();
                    row.Cells["durum"].Value = dr["Durum"].ToString();
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
                dr.Close();

                if (bekleyenCagrilarDGV.Rows.Count == 0 && cagriSayisi > 0)
                {
                    MessageBox.Show("Bekleyen çağrılar grid'e yüklenemedi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bekleyen çağrılar yüklenirken hata oluştu: " + ex.Message, "Veri Yükleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                baglanti.BaglantiAc();

                string kontrolSorgusu =
                    @"SELECT COUNT(*) FROM Kullanici 
            WHERE Rol = 'Ekip Üyesi'
            AND (DepartmanID = (SELECT DepartmanID FROM Kullanici WHERE KullaniciID = @yoneticiId)
            OR YoneticiID = @yoneticiId)";

                SqlCommand kontrolKomut = new SqlCommand(kontrolSorgusu, baglanti.conn);
                kontrolKomut.Parameters.AddWithValue("@yoneticiId", yoneticiId);

                int kullaniciSayisi = Convert.ToInt32(kontrolKomut.ExecuteScalar());

                if (kullaniciSayisi == 0)
                {
                    ekipUyeleriDGV.Rows.Clear();

                    if (!icerikPanel.Controls.ContainsKey("lblBosEkip"))
                    {
                        Label lblBosEkip = new Label();
                        lblBosEkip.Name = "lblBosEkip";
                        lblBosEkip.Text = "Departmanınızda ekip üyesi bulunmuyor.";
                        lblBosEkip.AutoSize = false;
                        lblBosEkip.Size = new Size(icerikPanel.Width - 20, 60);
                        lblBosEkip.TextAlign = ContentAlignment.MiddleCenter;
                        lblBosEkip.Font = new Font(lblBosEkip.Font.FontFamily, 10, FontStyle.Regular);
                        lblBosEkip.ForeColor = Color.Gray;
                        lblBosEkip.Location = new Point(10, icerikPanel.Height / 2 - 30);
                        icerikPanel.Controls.Add(lblBosEkip);
                    }

                    ekipUyeleriDGV.Visible = false;
                    return;
                }
                else
                {
                    if (icerikPanel.Controls.ContainsKey("lblBosEkip"))
                    {
                        icerikPanel.Controls.RemoveByKey("lblBosEkip");
                    }

                    ekipUyeleriDGV.Visible = true;
                }

                string anaSorgu =
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
          WHERE k.Rol = 'Ekip Üyesi'
            AND (k.DepartmanID = (SELECT DepartmanID FROM Kullanici WHERE KullaniciID = @yoneticiId)
            OR k.YoneticiID = @yoneticiId)
          GROUP BY k.KullaniciID, k.Ad, k.Soyad";

                SqlCommand komut = new SqlCommand(anaSorgu, baglanti.conn);
                komut.Parameters.AddWithValue("@yoneticiId", yoneticiId);
                SqlDataReader dr = komut.ExecuteReader();

                ekipUyeleriDGV.Rows.Clear();

                while (dr.Read())
                {
                    int rowIndex = ekipUyeleriDGV.Rows.Add();
                    DataGridViewRow row = ekipUyeleriDGV.Rows[rowIndex];

                    int kullaniciID = Convert.ToInt32(dr["KullaniciID"]);
                    row.Cells["calisan"].Value = dr["AdSoyad"].ToString();
                    row.Cells["aktifGorev"].Value = dr["AktifGorevSayisi"].ToString();
                    row.Cells["tamamlananGorev"].Value = dr["TamamlananGorevSayisi"].ToString();

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

                    row.Tag = kullaniciID;
                }
                dr.Close();

                if (ekipUyeleriDGV.Rows.Count == 0 && kullaniciSayisi > 0)
                {
                    MessageBox.Show("Ekip üyeleri grid'e yüklenemedi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekip üyeleri yüklenirken hata oluştu: " + ex.Message, "Veri Yükleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                int yoneticiId = int.Parse(KullaniciBilgi.KullaniciID);

                // Ekibe ait kullanıcıları al
                SqlCommand ekipUyeKomut = new SqlCommand("SELECT KullaniciID FROM Kullanici WHERE YoneticiID = @yoneticiId", baglanti.conn);
                ekipUyeKomut.Parameters.AddWithValue("@yoneticiId", yoneticiId);
                List<int> ekipUyeIDListesi = new List<int>();
                using (SqlDataReader reader = ekipUyeKomut.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ekipUyeIDListesi.Add(reader.GetInt32(0));
                    }
                }
                // Kendini de ekle (kendi adına gelen çağrılar için)
                ekipUyeIDListesi.Add(yoneticiId);

                // Listeyi SQL IN ifadesine dönüştür
                string idListeString = string.Join(",", ekipUyeIDListesi);

                // Bekleyen çağrı
                SqlCommand bekleyenCagri = new SqlCommand(
                    $"SELECT COUNT(*) FROM Cagri WHERE Durum = 'Beklemede' AND AtananKullaniciID IN ({idListeString})", baglanti.conn);
                int bekleyenCagriSayisi = Convert.ToInt32(bekleyenCagri.ExecuteScalar());
                lblBeklemede.Text = bekleyenCagriSayisi.ToString();

                // Atanan çağrı
                SqlCommand atananCagri = new SqlCommand(
                    $"SELECT COUNT(*) FROM Cagri WHERE Durum = 'Atandı' AND AtananKullaniciID IN ({idListeString})", baglanti.conn);
                int atananCagriSayisi = Convert.ToInt32(atananCagri.ExecuteScalar());
                lblAtanan.Text = atananCagriSayisi.ToString();

                // Tamamlanan çağrı (Son 30 gün)
                SqlCommand tamamlananCagri = new SqlCommand(
                    $@"SELECT COUNT(*) FROM Cagri 
               WHERE Durum = 'Tamamlandı' 
               AND TeslimTarihi >= DATEADD(day, -30, GETDATE())
               AND AtananKullaniciID IN ({idListeString})", baglanti.conn);
                int tamamlananCagriSayisi = Convert.ToInt32(tamamlananCagri.ExecuteScalar());
                lblTamamlanan.Text = tamamlananCagriSayisi.ToString();

                // Geciken çağrı
                SqlCommand gecikenCagri = new SqlCommand($@"
            SELECT COUNT(*) FROM Cagri 
            WHERE Durum != 'Tamamlandı' 
            AND AtananKullaniciID IN ({idListeString})
            AND TRY_CONVERT(float, REPLACE(REPLACE(REPLACE(HedefSure, ' Saat', ''), ' saat', ''), ',', '.')) < 
                DATEDIFF(hour, OlusturmaTarihi, GETDATE())", baglanti.conn);
                int gecikenCagriSayisi = Convert.ToInt32(gecikenCagri.ExecuteScalar());
                lblGeciken.Text = gecikenCagriSayisi.ToString();

                // Pano mesajı
                label1.Text = $"Bugün {bekleyenCagriSayisi} bekleyen çağrı ve {atananCagriSayisi} devam eden görev bulunmaktadır.";
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

                var cellValue = bekleyenCagrilarDGV.Rows[e.RowIndex].Cells["baslik"].Value;
                string baslik = cellValue != null ? cellValue.ToString() : string.Empty;

                try
                {
                    // Çağrı atama formunu aç
                    TasksAssignmentForm cagriAtamaForm = new TasksAssignmentForm(cagriId, baslik, yoneticiId);
                    if (cagriAtamaForm.ShowDialog() == DialogResult.OK)
                    {
                        BekleyenCagrilariYukle();
                        EkipUyeleriniYukle();
                        IstatistikleriGoster();
                    }
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Çağrı atama formu açılırken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            ManagerReportsPage managerReports = new ManagerReportsPage();
            managerReports.Show();
            this.Close();
        }

        private void btnEkipYonetimi_Click(object sender, EventArgs e)
        {
            ManagerDashboard managerDashboard = new ManagerDashboard();
            managerDashboard.Show();
            this.Close();
        }
        private void btnCikis_Click(object sender, EventArgs e)
        {
            KullaniciBilgi.BilgileriTemizle();

            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

    }

}