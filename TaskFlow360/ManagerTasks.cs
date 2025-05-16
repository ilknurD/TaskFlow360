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
    public partial class ManagerTasks : Form
    {
        private Baglanti baglanti = new Baglanti();

        public ManagerTasks()
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

        private void gorevlerTab_Click(object sender, EventArgs e)
        {

        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerTasks managerTasks = new ManagerTasks();
            managerTasks.Show();
        }

        private void ManagerTasks_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDataFromDatabase();
                LoadTeamMembers();
                ConfigureBekleyenCagrilarDGV();
                ConfigureEkipUyeleriDGV();
                FormatDataGridViews();

                CagrilarDGV.CellContentClick += CagrilarDGV_CellContentClick;
                CagrilarDGV.DataBindingComplete += CagrilarDGV_DataBindingComplete;
                CagrilarDGV.CellFormatting += CagrilarDGV_CellFormatting;
                ekipUyeleriDGV.CellContentClick += ekipUyeleriDGV_CellContentClick;
                ekipUyeleriDGV.DataBindingComplete += ekipUyeleriDGV_DataBindingComplete;
                ekipUyeleriDGV.CellFormatting += ekipUyeleriDGV_CellFormatting;

                CagrilarDGV.RowTemplate.Height = 40;
                ekipUyeleriDGV.RowTemplate.Height = 40;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();

                string managerID = KullaniciBilgi.KullaniciID;

                string queryForSelf = @"SELECT 
    '#' + CAST(C.CagriID AS NVARCHAR(10)) AS CagriNumarasi,
    C.Baslik AS Baslik,
    C.CagriKategori AS CagriKategori,
    C.Oncelik AS Oncelik,
    C.Durum AS Durum
FROM 
    [dbo].[Cagri] C
WHERE 
    C.AtananKullaniciID = @ManagerID
ORDER BY 
    CASE C.Oncelik
        WHEN 'Yüksek' THEN 1
        WHEN 'Orta' THEN 2
        WHEN 'Normal' THEN 3
        ELSE 4
    END";

                // Sütunlar temizlenip yeniden ekleniyor
                CagrilarDGV.Columns.Clear();
                CagrilarDGV.Columns.Add("CagriNumarasi", "Çağrı No");
                CagrilarDGV.Columns.Add("Baslik", "Başlık");
                CagrilarDGV.Columns.Add("CagriKategori", "Kategori");
                CagrilarDGV.Columns.Add("Oncelik", "Öncelik");
                CagrilarDGV.Columns.Add("Durum", "Durum");

                DataGridViewButtonColumn islemButon = new DataGridViewButtonColumn();
                islemButon.Name = "islemButon";
                islemButon.HeaderText = "İşlem";
                islemButon.Text = "Düzenle";
                islemButon.UseColumnTextForButtonValue = true;
                CagrilarDGV.Columns.Add(islemButon);

                CagrilarDGV.Rows.Clear();

                using (SqlCommand cmd = new SqlCommand(queryForSelf, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@ManagerID", managerID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CagrilarDGV.Rows.Add(
                                reader["CagriNumarasi"].ToString(),
                                reader["Baslik"].ToString(),
                                reader["CagriKategori"].ToString(),
                                reader["Oncelik"].ToString(),
                                reader["Durum"].ToString()
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SQL Hatası: {ex.ToString()}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.conn.State == ConnectionState.Open)
                    baglanti.conn.Close();
            }
        }

        private void FormatDataGridViews()
        {
            // CagrilarDGV formatlama
            foreach (DataGridViewRow row in CagrilarDGV.Rows)
            {
                if (row.Cells["Oncelik"].Value != null)
                {
                    string oncelik = row.Cells["Oncelik"].Value.ToString();
                    if (oncelik == "Yüksek")
                        row.Cells["Oncelik"].Style.BackColor = ColorTranslator.FromHtml("#f85c5c");
                    else if (oncelik == "Orta")
                        row.Cells["Oncelik"].Style.BackColor = ColorTranslator.FromHtml("#f0ad4e");
                    else
                        row.Cells["Oncelik"].Style.BackColor = ColorTranslator.FromHtml("#63c966");

                    row.Cells["Oncelik"].Style.ForeColor = Color.White;
                    row.Cells["Oncelik"].Style.Font = new Font("Century Gothic", 12, FontStyle.Bold);
                }
            }
            
            // EkipUyeleriDGV formatlama
            foreach (DataGridViewRow row in ekipUyeleriDGV.Rows)
            {
                // Örnek: Aktif görev sayısı 5'ten fazlaysa özel renk
                if (row.Cells["AktifGorevler"].Value != null &&
                    int.TryParse(row.Cells["AktifGorevler"].Value.ToString(), out int gorevSayisi) &&
                    gorevSayisi > 5)
                {
                    row.Cells["AktifGorevler"].Style.BackColor = Color.LightSalmon;
                }
            }
        }
        private void CagriAtama(string memberID, string taskId)
        {
            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();

                string query = @"UPDATE dbo.Cagri
                        SET AtananKullaniciID = @MemberID
                        WHERE CagriID = @TaskID";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@MemberID", memberID);
                    cmd.Parameters.AddWithValue("@TaskID", taskId);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Çağrı başarıyla üyeye atandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (baglanti.conn.State == ConnectionState.Open) //yenileme
                            baglanti.conn.Close();

                        LoadDataFromDatabase(); 
                    }
                    else
                    {
                        MessageBox.Show("Çağrı atama sırasında bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Çağrı atama sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.conn.State == ConnectionState.Open)
                    baglanti.conn.Close();
            }
        }
        private void ekipUyeleriDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= ekipUyeleriDGV.Rows.Count)
                    return;

                if (ekipUyeleriDGV.Columns[e.ColumnIndex].Name != "gorevAtaButon")
                    return;

                // Kullanıcı ID'sini al Tag'den
                DataGridViewRow selectedRow = ekipUyeleriDGV.Rows[e.RowIndex];
                string memberID = selectedRow.Tag?.ToString();

                if (string.IsNullOrEmpty(memberID))
                {
                    MessageBox.Show("Üye seçimi geçersiz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Yeni formu aç ve memberID'yi gönder
                //TaskAssignmentForm assignmentForm = new TaskAssignmentForm(memberID);
                //assignmentForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureBekleyenCagrilarDGV()
        {
            // Temel seçim ayarları
            CagrilarDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            CagrilarDGV.DefaultCellStyle.SelectionBackColor = CagrilarDGV.DefaultCellStyle.BackColor;
            CagrilarDGV.DefaultCellStyle.SelectionForeColor = CagrilarDGV.DefaultCellStyle.ForeColor;

            // Başlık ayarları
            CagrilarDGV.EnableHeadersVisualStyles = false;
            CagrilarDGV.ColumnHeadersDefaultCellStyle.SelectionBackColor = CagrilarDGV.ColumnHeadersDefaultCellStyle.BackColor;
            CagrilarDGV.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Transparent;

            // Görsel iyileştirmeler
            CagrilarDGV.BorderStyle = BorderStyle.None;
            CagrilarDGV.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            CagrilarDGV.GridColor = Color.FromArgb(240, 240, 240);

            // Buton sütunu stil ayarı
            if (CagrilarDGV.Columns.Contains("islemButon"))
            {
                CagrilarDGV.Columns["islemButon"].DefaultCellStyle.BackColor = Color.FromArgb(126, 87, 194);
                CagrilarDGV.Columns["islemButon"].DefaultCellStyle.ForeColor = Color.White;
                CagrilarDGV.Columns["islemButon"].DefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }
        }

        private void ConfigureEkipUyeleriDGV()
        {
            // Temel seçim ayarları (CagrilarDGV ile aynı)
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

            if (ekipUyeleriDGV.Columns.Contains("gorevAtaButon"))
            {
                ekipUyeleriDGV.Columns["gorevAtaButon"].DefaultCellStyle.BackColor = Color.FromArgb(126, 87, 194);
                ekipUyeleriDGV.Columns["gorevAtaButon"].DefaultCellStyle.ForeColor = Color.White;
                ekipUyeleriDGV.Columns["gorevAtaButon"].DefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }
        }

        private void LoadTeamMembers()
        {
            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();

                string managerID = KullaniciBilgi.KullaniciID;

                string query = @"SELECT 
    K.KullaniciID,
    K.Ad + ' ' + K.Soyad AS AdSoyad,
    -- Aktif görev sayısı
    (SELECT COUNT(*) FROM Cagri C WHERE C.AtananKullaniciID = K.KullaniciID AND C.Durum IN ('Atandı', 'Beklemede')) AS AktifGorevler,
    
    -- Bugün tamamlanan görevler
    (SELECT COUNT(*) FROM Cagri C WHERE C.AtananKullaniciID = K.KullaniciID AND C.Durum = 'Tamamlandı' AND CAST(C.TeslimTarihi AS DATE) = CAST(GETDATE() AS DATE)) AS BugunTamamlanan,

    -- Son 30 gün içindeki performans
    (SELECT COUNT(*) FROM Cagri C WHERE C.AtananKullaniciID = K.KullaniciID AND C.Durum = 'Tamamlandı' AND C.TeslimTarihi >= DATEADD(DAY, -30, GETDATE())) AS AylikPerformans,

    -- Ortalama çözüm süresi (saat cinsinden)
    (SELECT 
        AVG(DATEDIFF(MINUTE, C.OlusturmaTarihi, C.TeslimTarihi)) / 60.0
     FROM 
        Cagri C 
     WHERE 
        C.AtananKullaniciID = K.KullaniciID 
        AND C.Durum = 'Tamamlandı'
        AND C.TeslimTarihi IS NOT NULL
    ) AS OrtalamaSureSaat

FROM 
    Kullanici K
WHERE 
    K.YoneticiID = @ManagerID
";
                ekipUyeleriDGV.Columns.Clear();
                ekipUyeleriDGV.Columns.Add("AdSoyad", "Ad Soyad");
                ekipUyeleriDGV.Columns.Add("AktifGorevler", "Aktif Görevler");
                ekipUyeleriDGV.Columns.Add("BugunTamamlanan", "Bugün Tamamlanan");
                ekipUyeleriDGV.Columns.Add("AylikPerformans", "Aylık Performans");
                ekipUyeleriDGV.Columns.Add("OrtalamaSureSaat", "Ortalama Süre (saat)");

                DataGridViewButtonColumn gorevAtaButon = new DataGridViewButtonColumn();
                gorevAtaButon.Name = "gorevAtaButon";
                gorevAtaButon.HeaderText = "Görev Ata";
                gorevAtaButon.Text = "Görev Ata";
                gorevAtaButon.UseColumnTextForButtonValue = true;
                ekipUyeleriDGV.Columns.Add(gorevAtaButon);
                ekipUyeleriDGV.Rows.Clear();

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@ManagerID", managerID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rowIndex = ekipUyeleriDGV.Rows.Add(
                                reader["AdSoyad"].ToString(),
                                reader["AktifGorevler"].ToString(),
                                reader["BugunTamamlanan"].ToString(),
                                reader["AylikPerformans"].ToString(),
                                string.Format("{0:0.00} saat", reader["OrtalamaSureSaat"] is DBNull ? 0 : reader["OrtalamaSureSaat"])
                            );
                            ekipUyeleriDGV.Rows[rowIndex].Tag = reader["KullaniciID"].ToString();
                        }
                    }
                }
                ConfigureEkipUyeleriDGV();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekip bilgileri yüklenirken hata oluştu: " + ex.Message);
            }
            finally
            {
                if (baglanti.conn.State == ConnectionState.Open)
                    baglanti.conn.Close();
            }
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerProfile manageProfile = new ManagerProfile();
            manageProfile.Show();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {

        }

        private void txtAraCagri_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtAraCagri.Text.Trim().ToLower();
            CagrilarDGV.ClearSelection();

            foreach (DataGridViewRow row in CagrilarDGV.Rows)
            {
                if (row.IsNewRow) continue;

                bool match = false;

                if (row.Cells["CagriNumarasi"].Value != null && row.Cells["CagriNumarasi"].Value.ToString().ToLower().Contains(searchText) ||
                    row.Cells["Baslik"].Value != null && row.Cells["Baslik"].Value.ToString().ToLower().Contains(searchText) ||
                    row.Cells["CagriKategori"].Value != null && row.Cells["CagriKategori"].Value.ToString().ToLower().Contains(searchText) ||
                    row.Cells["Oncelik"].Value != null && row.Cells["Oncelik"].Value.ToString().ToLower().Contains(searchText) ||
                    row.Cells["Durum"].Value != null && row.Cells["Durum"].Value.ToString().ToLower().Contains(searchText))
                {
                    match = true;
                }

                row.Visible = match;
            }
        }


        private void CagrilarDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Tüm hücreler için temel font ayarı
            e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Regular);

            // İşlem butonu için özel stil
            if (CagrilarDGV.Columns[e.ColumnIndex].Name == "islemButon")
            {
                e.CellStyle.BackColor = Color.FromArgb(126, 87, 194);
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Regular);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                e.CellStyle.SelectionBackColor = Color.FromArgb(126, 87, 194); // Seçili durumda da rengi koru
                return;
            }

            // Öncelik sütunu için renklendirme
            if (CagrilarDGV.Columns[e.ColumnIndex].Name == "Oncelik" && e.Value != null)
            {
                string oncelik = e.Value.ToString();
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
                e.CellStyle.ForeColor = Color.White;
                return;
            }

            // Seçili satır için genel ayarlar
            if (CagrilarDGV.Rows[e.RowIndex].Selected)
            {
                // Özel renklendirme yapılmamış sütunlar için varsayılan seçim rengi
                if (CagrilarDGV.Columns[e.ColumnIndex].Name != "Oncelik" &&
                    CagrilarDGV.Columns[e.ColumnIndex].Name != "islemButon")
                {
                    e.CellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
                    e.CellStyle.SelectionForeColor = Color.Black;
                }
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }
        }

        private void CagrilarDGV_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            CagrilarDGV.ClearSelection();
        }

        private void txtAraEkip_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtAraEkip.Text.Trim().ToLower();
            ekipUyeleriDGV.ClearSelection();

            foreach (DataGridViewRow row in ekipUyeleriDGV.Rows)
            {
                if (row.IsNewRow) continue;

                bool match = false;

                for (int i = 0; i < ekipUyeleriDGV.Columns.Count - 1; i++) //son sütun buton, onu komtrol etme
                {
                    if (row.Cells[i].Value != null && row.Cells[i].Value.ToString().ToLower().Contains(searchText))
                    {
                        match = true;
                        break;
                    }
                }

                row.Visible = match;
            }
        }

        private void ekipUyeleriDGV_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ekipUyeleriDGV.ClearSelection();
        }

        private void CagrilarDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        //    try
        //    {
        //        if (e.RowIndex >= 0 && e.ColumnIndex == CagrilarDGV.Columns["islemButon"].Index)
        //        {
        //            // Satırı seç
        //            CagrilarDGV.Rows[e.RowIndex].Selected = true;

        //            string cagriID = CagrilarDGV.Rows[e.RowIndex].Cells["CagriNumarasi"].Value.ToString().Replace("#", "");

        //            // İşlemlerinizi burada yapın
        //            //EditCallForm editForm = new EditCallForm(cagriID);
        //            //editForm.ShowDialog();

        //            LoadDataFromDatabase();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"İşlem sırasında bir hata oluştu: {ex.Message}", "Hata",
        //                      MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //
        }

        private void ekipUyeleriDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // Buton sütunu hariç diğer sütunlar için formatlama
            if (ekipUyeleriDGV.Columns[e.ColumnIndex].Name != "gorevAtaButon")
            {
                // Satır seçiliyse arka plan rengi
                if (ekipUyeleriDGV.Rows[e.RowIndex].Selected)
                {
                    e.CellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
                    e.CellStyle.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                // Görev Ata butonu için özel stil
                e.CellStyle.BackColor = Color.FromArgb(126, 87, 194);
                e.CellStyle.ForeColor = Color.Black;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
    }
}