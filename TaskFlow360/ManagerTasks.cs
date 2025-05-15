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
            foreach (DataGridViewRow row in CagrilarDGV.Rows)
            {
                if (row.Cells["Oncelik"].Value != null) // Büyük/küçük harfe dikkat!
                {
                    string oncelik = row.Cells["Oncelik"].Value.ToString();
                    if (oncelik == "Yüksek")
                        row.Cells["Oncelik"].Style.BackColor = ColorTranslator.FromHtml("#f85c5c");
                    else if (oncelik == "Orta")
                        row.Cells["Oncelik"].Style.BackColor = ColorTranslator.FromHtml("#f0ad4e");
                    else
                        row.Cells["Oncelik"].Style.BackColor = ColorTranslator.FromHtml("#63c966");

                    row.Cells["Oncelik"].Style.ForeColor = Color.White;
                    row.Cells["Oncelik"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                }
            }
        }
        private void CagriAtama(string memberID, string taskId)
        {
            try
            {
                // Bağlantıyı açın
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
                        // Bağlantıyı kapatıp tekrar açıp verileri yeniliyoruz
                        if (baglanti.conn.State == ConnectionState.Open)
                            baglanti.conn.Close();

                        LoadDataFromDatabase(); // Verileri yenile
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
                // İşlem bittiğinde bağlantıyı kapatın
                if (baglanti.conn.State == ConnectionState.Open)
                    baglanti.conn.Close();
            }
        }

        private void ekipUyeleriDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = ekipUyeleriDGV.Rows[e.RowIndex];
                string memberID = row.Cells["KullaniciID"].Value.ToString(); // Üye ID'si
                string taskId = row.Cells["CagriNumarasi"].Value.ToString().Replace("#", ""); // Çağrı ID'si

                // Görev Ata Butonu tıklandıysa
                if (ekipUyeleriDGV.Columns[e.ColumnIndex].Name == "gorevAtaButon")
                {
                    CagriAtama(memberID, taskId);
                }
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

            CagrilarDGV.CellFormatting += CagrilarDGV_CellFormatting;
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
        private void LoadTeamMembers()
        {
            try
            {
                // Bağlantıyı açın
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
                                string.Format("{0:0.00} saat", reader["OrtalamaSureSaat"] is DBNull ? 0 : reader["OrtalamaSureSaat"]),
                                "Detay" 
                            );
                            ekipUyeleriDGV.Rows[rowIndex].Tag = reader["KullaniciID"].ToString();
                        }
                    }
                }
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

        private void txtAra_TextChanged_1(object sender, EventArgs e)
        {
            string searchText = txtAra.Text.Trim();

            foreach (DataGridViewRow row in CagrilarDGV.Rows)
            {
                if (row.IsNewRow) continue;

                bool match = false;

                // Çağrı ID veya Başlıkta arama yap
                if (row.Cells["CagriNumarasi"].Value.ToString().Contains(searchText) ||
                    row.Cells["Baslik"].Value.ToString().Contains(searchText))
                {
                    match = true;
                }

                row.Visible = match;
            }
        }

        private void CagrilarDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            //if (CagrilarDGV.Rows[e.RowIndex].Selected)
            //{
            //    e.CellStyle.ForeColor = Color.Black; 
            //}
            //else
            //{
            //    e.CellStyle.ForeColor = CagrilarDGV.DefaultCellStyle.ForeColor;
            //}
        }

        private void CagrilarDGV_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            CagrilarDGV.ClearSelection();
        }
    }
}