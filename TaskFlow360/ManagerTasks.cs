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
using System.Net;

namespace TaskFlow360
{
    public partial class ManagerTasks : Form
    {
        private Connection baglanti = new Connection();
        private Logger logger;
        public ManagerTasks()
        {
            InitializeComponent();
            logger = new Logger();
            logger.LogEkle("ManagerTasks formu başlatıldı", "Form", "ManagerTasks");
        }
        private void GuncelDurumLabeliniGuncelle()
        {
            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();

                string managerID = UserInformation.KullaniciID;

                // Bugün açılan bekleyen çağrı sayısı (yönetici ve ekibi için)
                string bekleyenSorgu = @"
                SELECT COUNT(*) FROM Cagri C
                INNER JOIN Kullanici K ON C.AtananKullaniciID = K.KullaniciID
                WHERE (C.AtananKullaniciID = @ManagerID OR K.YoneticiID = @ManagerID)
                AND C.Durum = 'Beklemede'
                AND CAST(C.OlusturmaTarihi AS DATE) = CAST(GETDATE() AS DATE)";

                // Devam eden görevler (Beklemede veya Atandı)
                string devamEdenSorgu = @"
                SELECT COUNT(*) FROM Cagri C
                INNER JOIN Kullanici K ON C.AtananKullaniciID = K.KullaniciID
                WHERE (C.AtananKullaniciID = @ManagerID OR K.YoneticiID = @ManagerID)
                AND C.Durum IN ('Beklemede', 'Atandı')";

                int bugunBekleyen = 0;
                int devamEden = 0;

                using (SqlCommand cmd = new SqlCommand(bekleyenSorgu, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@ManagerID", managerID);
                    bugunBekleyen = (int)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand(devamEdenSorgu, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@ManagerID", managerID);
                    devamEden = (int)cmd.ExecuteScalar();
                }

                label1.Text = $"Bugün {bugunBekleyen} yeni bekleyen çağrı ve {devamEden} devam eden görev bulunmaktadır.";
            }
            catch (Exception ex)
            {
                label1.Text = "Durum bilgisi alınamadı.";
            }
            finally
            {
                if (baglanti.conn.State == ConnectionState.Open)
                    baglanti.conn.Close();
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            logger.LogEkle("Kapat butonuna tıklandı", "Buton", "ManagerTasks");
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            logger.LogEkle("Küçült butonuna tıklandı", "Buton", "ManagerTasks");
            WindowState = FormWindowState.Minimized;
        }

        private void gorevlerTab_Click(object sender, EventArgs e)
        {
            logger.LogEkle("Görevler sekmesine tıklandı", "Buton", "ManagerTasks");
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            logger.LogEkle("Anasayfa butonuna tıklandı", "Buton", "ManagerTasks");
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            logger.LogEkle("Görevler butonuna tıklandı", "Buton", "ManagerTasks");
            ManagerTasks managerTasks = new ManagerTasks();
            managerTasks.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            logger.LogEkle("Profil butonuna tıklandı", "Buton", "ManagerTasks");
            ManagerProfile manageProfile = new ManagerProfile();
            manageProfile.Show();
            this.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            logger.LogEkle("Çıkış butonuna tıklandı", "Buton", "ManagerTasks");
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            logger.LogEkle("Raporlar butonuna tıklandı", "Buton", "ManagerTasks");
            ManagerReportsPage reportPage = new ManagerReportsPage();
            reportPage.Show();
            this.Close();
        }

        private void btnEkipYonetimi_Click(object sender, EventArgs e)
        {
            logger.LogEkle("Ekip Yönetimi butonuna tıklandı", "Buton", "ManagerTasks");
            ManagerDashboard managerDashboardPage = new ManagerDashboard();
            managerDashboardPage.Show();
            this.Close();
        }

        private void ManagerTasks_Load(object sender, EventArgs e)
        {
            logger.LogEkle("ManagerTasks yüklenmeye başlandı", "Form", "ManagerTasks");
            try
            {
                LoadDataFromDatabase();
                logger.LogEkle("Çağrılar başarıyla yüklendi", "Okuma", "ManagerTasks");
                LoadTeamMembers();
                logger.LogEkle("Ekip üyeleri başarıyla yüklendi", "Okuma", "ManagerTasks");
                GuncelDurumLabeliniGuncelle();
                ConfigureBekleyenCagrilarDGV();
                ConfigureEkipUyeleriDGV();
                FormatDataGridViews();
                CagrilarDGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                CagrilarDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
                CagrilarDGV.EnableHeadersVisualStyles = false;

                foreach (DataGridViewColumn column in CagrilarDGV.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                foreach (DataGridViewColumn column in ekipUyeleriDGV.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                CagrilarDGV.CellContentClick += CagrilarDGV_CellContentClick;
                CagrilarDGV.DataBindingComplete += CagrilarDGV_DataBindingComplete;
                CagrilarDGV.CellFormatting += CagrilarDGV_CellFormatting;
                ekipUyeleriDGV.CellContentClick += ekipUyeleriDGV_CellContentClick;
                ekipUyeleriDGV.DataBindingComplete += ekipUyeleriDGV_DataBindingComplete;
                ekipUyeleriDGV.CellFormatting += ekipUyeleriDGV_CellFormatting;

                CagrilarDGV.RowTemplate.Height = 27;
                ekipUyeleriDGV.RowTemplate.Height = 27;

            }
            catch (Exception ex)
            {
                logger.LogEkle($"Veriler yüklenirken hata oluştu: {ex.Message}", "Hata", "ManagerTasks");
                MessageBox.Show($"Veriler yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();

                string managerID = UserInformation.KullaniciID;

                string queryForSelfAndTeam = @"
                SELECT 
                    '#' + CAST(C.CagriID AS NVARCHAR(10)) AS CagriNumarasi,
                    C.Baslik AS Baslik,
                    C.CagriKategori AS CagriKategori,
                    C.Oncelik AS Oncelik,
                    C.Durum AS Durum
                FROM 
                    [dbo].[Cagri] C
                INNER JOIN [dbo].[Kullanici] K ON C.AtananKullaniciID = K.KullaniciID
                WHERE 
                    C.AtananKullaniciID = @ManagerID OR
                    K.YoneticiID = @ManagerID
                ORDER BY 
                    CASE C.Oncelik
                        WHEN 'Yüksek' THEN 1
                        WHEN 'Orta' THEN 2
                        WHEN 'Normal' THEN 3
                        ELSE 4
                    END";

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

                using (SqlCommand cmd = new SqlCommand(queryForSelfAndTeam, baglanti.conn))
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


        private void LoadTeamMembers()
        {
            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();

                string managerID = UserInformation.KullaniciID;

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

        private void FormatDataGridViews()
        {
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
        }
        private void ekipUyeleriDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= ekipUyeleriDGV.Rows.Count)
                    return;

                if (ekipUyeleriDGV.Columns[e.ColumnIndex].Name != "gorevAtaButon")
                    return;

                DataGridViewRow selectedRow = ekipUyeleriDGV.Rows[e.RowIndex];
                string memberID = selectedRow.Tag?.ToString();

                if (string.IsNullOrEmpty(memberID))
                {
                    MessageBox.Show("Üye seçimi geçersiz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Tasks tasks = new Tasks(memberID, UserInformation.KullaniciID);
                tasks.SetMemberName(selectedRow.Cells["AdSoyad"].Value.ToString());
                tasks.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CagrilarDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == CagrilarDGV.Columns["islemButon"].Index)
                {
                    CagrilarDGV.Rows[e.RowIndex].Selected = true;

                    string cagriID = CagrilarDGV.Rows[e.RowIndex].Cells["CagriNumarasi"].Value.ToString().Replace("#", "");
                    LoadDataFromDatabase();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında bir hata oluştu: {ex.Message}", "Hata",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ConfigureBekleyenCagrilarDGV()
        {
            CagrilarDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            CagrilarDGV.DefaultCellStyle.SelectionBackColor = CagrilarDGV.DefaultCellStyle.BackColor;
            CagrilarDGV.DefaultCellStyle.SelectionForeColor = CagrilarDGV.DefaultCellStyle.ForeColor;
            CagrilarDGV.EnableHeadersVisualStyles = false;
            CagrilarDGV.ColumnHeadersDefaultCellStyle.SelectionBackColor = CagrilarDGV.ColumnHeadersDefaultCellStyle.BackColor;
            CagrilarDGV.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Transparent;
            CagrilarDGV.BorderStyle = BorderStyle.None;
            CagrilarDGV.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            CagrilarDGV.GridColor = Color.FromArgb(240, 240, 240);
            if (CagrilarDGV.Columns.Contains("islemButon"))
            {
                CagrilarDGV.Columns["islemButon"].DefaultCellStyle.BackColor = Color.FromArgb(126, 87, 194);
                CagrilarDGV.Columns["islemButon"].DefaultCellStyle.ForeColor = Color.White;
                CagrilarDGV.Columns["islemButon"].DefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }
        }

        private void ConfigureEkipUyeleriDGV()
        {
            ekipUyeleriDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ekipUyeleriDGV.DefaultCellStyle.SelectionBackColor = ekipUyeleriDGV.DefaultCellStyle.BackColor;
            ekipUyeleriDGV.DefaultCellStyle.SelectionForeColor = ekipUyeleriDGV.DefaultCellStyle.ForeColor;
            ekipUyeleriDGV.EnableHeadersVisualStyles = false;
            ekipUyeleriDGV.ColumnHeadersDefaultCellStyle.SelectionBackColor = ekipUyeleriDGV.ColumnHeadersDefaultCellStyle.BackColor;
            ekipUyeleriDGV.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Transparent;
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

        private void CagrilarDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Regular);

            if (CagrilarDGV.Columns[e.ColumnIndex].Name == "islemButon")
            {
                e.CellStyle.BackColor = Color.FromArgb(126, 87, 194);
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Regular);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                e.CellStyle.SelectionBackColor = Color.FromArgb(126, 87, 194); // Seçili durumda da rengi koru
                return;
            }

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

            if (CagrilarDGV.Rows[e.RowIndex].Selected)
            {
                if (CagrilarDGV.Columns[e.ColumnIndex].Name != "Oncelik" &&
                    CagrilarDGV.Columns[e.ColumnIndex].Name != "islemButon")
                {
                    e.CellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
                    e.CellStyle.SelectionForeColor = Color.Black;
                }
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }
        }
        private void ekipUyeleriDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (ekipUyeleriDGV.Columns[e.ColumnIndex].Name != "gorevAtaButon")
            {
                if (ekipUyeleriDGV.Rows[e.RowIndex].Selected)
                {
                    e.CellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
                    e.CellStyle.SelectionForeColor = Color.Black;
                }
            }
            else
            {
                e.CellStyle.BackColor = Color.FromArgb(126, 87, 194);
                e.CellStyle.ForeColor = Color.Black;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        private void CagrilarDGV_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            CagrilarDGV.ClearSelection();
        }

        private void ekipUyeleriDGV_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ekipUyeleriDGV.ClearSelection();
        }
        private int GetTalepEdenIDByCagriID(int cagriID)
        {
            int talepEdenID = 0;

            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();

                string query = "SELECT TalepEdenID FROM [dbo].[Cagri] WHERE CagriID = @CagriID";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@CagriID", cagriID);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        talepEdenID = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"TalepEden ID alınırken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.conn.State == ConnectionState.Open)
                    baglanti.conn.Close();
            }

            return talepEdenID;
        }
        private void CagrilarDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == CagrilarDGV.Columns["islemButon"].Index)
                {
                    CagrilarDGV.Rows[e.RowIndex].Selected = true;
                    string cagriID = CagrilarDGV.Rows[e.RowIndex].Cells["CagriNumarasi"].Value.ToString().Replace("#", "");
                    if (int.TryParse(cagriID, out int parsedCagriID))
                    {
                        int talepEdenID = GetTalepEdenIDByCagriID(parsedCagriID);

                        TaskDetail taskDetailForm = new TaskDetail(parsedCagriID, talepEdenID);
                        taskDetailForm.ShowDialog();
                        LoadDataFromDatabase();
                    }
                    else
                    {
                        MessageBox.Show("Geçersiz çağrı ID'si.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında bir hata oluştu: {ex.Message}", "Hata",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}