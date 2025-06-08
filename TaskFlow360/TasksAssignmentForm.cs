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
    public partial class TasksAssignmentForm : Form
    {
        private int cagriId;
        private string baslik;
        private string yoneticiId;
        Connection baglanti = new Connection();

        public TasksAssignmentForm(int cagriId, string baslik, string yoneticiId)
        {
            InitializeComponent();
            this.cagriId = cagriId;
            this.baslik = baslik;
            this.yoneticiId = yoneticiId;
        }

        private void TasksAssignmentForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDepartmanlar();
                EkipUyeleriniYukle();
                LoadCagriInfo();
                ConfigureDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Form yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCagriInfo()
        {
            try
            {
                baglanti.BaglantiAc();

                // Çağrı bilgilerini doğrudan SQL sorgusu ile çek
                string query = "SELECT CagriID, Baslik, CagriAciklama FROM Cagri WHERE CagriID = @CagriID";
                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@CagriID", cagriId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtCagriID.Text = reader["CagriID"].ToString();
                            txtBaslik.Text = reader["Baslik"].ToString();
                            txtAciklama.Text = reader["CagriAciklama"]?.ToString() ?? "";

                            txtCagriID.ReadOnly = true;
                            txtBaslik.ReadOnly = true;
                        }
                        else
                        {
                            MessageBox.Show("Çağrı bilgisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Çağrı bilgisi çekilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void ConfigureDataGrid()
        {
            // DataGridView ayarları
            foreach (DataGridViewColumn column in dataGridEkip.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            dataGridEkip.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridEkip.AllowUserToResizeColumns = false;
            dataGridEkip.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Seçim renkleri
            dataGridEkip.DefaultCellStyle.SelectionBackColor = Color.FromArgb(79, 34, 158);
            dataGridEkip.DefaultCellStyle.SelectionForeColor = Color.White;

            // Başlıklar seçilince renk değişmesin
            dataGridEkip.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridEkip.ColumnHeadersDefaultCellStyle.BackColor;
            dataGridEkip.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridEkip.ColumnHeadersDefaultCellStyle.ForeColor;
        }

        private void LoadDepartmanlar()
        {
            try
            {
                baglanti.BaglantiAc();

                // Sadece yöneticinin ekibinde bulunan departmanları getir
                string query = @"
                    SELECT DISTINCT d.DepartmanID, d.DepartmanAdi 
                    FROM Departman d
                    INNER JOIN Bolum b ON d.DepartmanID = b.BagliDepartmanID
                    INNER JOIN Kullanici k ON b.BolumID = k.BolumID
                    WHERE k.YoneticiID = @YoneticiID
                    ORDER BY d.DepartmanAdi";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@YoneticiID", Convert.ToInt32(yoneticiId));

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // "Tümü" seçeneğini en üste ekle
                        DataRow tumRow = dt.NewRow();
                        tumRow["DepartmanID"] = -1;
                        tumRow["DepartmanAdi"] = "Tümü";
                        dt.Rows.InsertAt(tumRow, 0);

                        comboBoxDepartman.DisplayMember = "DepartmanAdi";
                        comboBoxDepartman.ValueMember = "DepartmanID";
                        comboBoxDepartman.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Departmanlar yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        // Departman seçildiğinde, bağlı bölümleri yükle
        private void comboBoxDepartman_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDepartman.SelectedValue == null) return;

            if (int.TryParse(comboBoxDepartman.SelectedValue.ToString(), out int departmanID))
            {
                if (departmanID == -1) // "Tümü" seçildi
                {
                    LoadAllBolumler();
                    EkipUyeleriniYukle(); // Tüm ekibi göster
                }
                else
                {
                    LoadBolumler(departmanID);
                }
            }
        }

        private void LoadBolumler(int departmanID)
        {
            try
            {
                baglanti.BaglantiAc();

                // Sadece yöneticinin ekibinde bulunan ve seçilen departmana ait bölümleri getir
                string query = @"
                    SELECT DISTINCT b.BolumID, b.BolumAdi 
                    FROM Bolum b
                    INNER JOIN Kullanici k ON b.BolumID = k.BolumID
                    WHERE b.BagliDepartmanID = @DepartmanID 
                    AND k.YoneticiID = @YoneticiID
                    ORDER BY b.BolumAdi";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmanID", departmanID);
                    cmd.Parameters.AddWithValue("@YoneticiID", Convert.ToInt32(yoneticiId));

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // "Tümü" seçeneğini en üste ekle
                        DataRow tumRow = dt.NewRow();
                        tumRow["BolumID"] = -1;
                        tumRow["BolumAdi"] = "Tümü";
                        dt.Rows.InsertAt(tumRow, 0);

                        comboBoxBolum.DisplayMember = "BolumAdi";
                        comboBoxBolum.ValueMember = "BolumID";
                        comboBoxBolum.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bölümler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void LoadAllBolumler()
        {
            try
            {
                baglanti.BaglantiAc();

                // Yöneticinin ekibindeki tüm bölümleri getir
                string query = @"
                    SELECT DISTINCT b.BolumID, b.BolumAdi 
                    FROM Bolum b
                    INNER JOIN Kullanici k ON b.BolumID = k.BolumID
                    WHERE k.YoneticiID = @YoneticiID
                    ORDER BY b.BolumAdi";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@YoneticiID", Convert.ToInt32(yoneticiId));

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // "Tümü" seçeneğini en üste ekle
                        DataRow tumRow = dt.NewRow();
                        tumRow["BolumID"] = -1;
                        tumRow["BolumAdi"] = "Tümü";
                        dt.Rows.InsertAt(tumRow, 0);

                        comboBoxBolum.DisplayMember = "BolumAdi";
                        comboBoxBolum.ValueMember = "BolumID";
                        comboBoxBolum.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bölümler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void comboBoxBolum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxBolum.SelectedValue == null || comboBoxBolum.SelectedValue == DBNull.Value)
                return;

            if (int.TryParse(comboBoxBolum.SelectedValue.ToString(), out int bolumID))
            {
                if (bolumID == -1) // "Tümü" seçildi
                {
                    // Departman seçimine göre filtreleme yap
                    if (comboBoxDepartman.SelectedValue != null &&
                        int.TryParse(comboBoxDepartman.SelectedValue.ToString(), out int departmanID))
                    {
                        if (departmanID == -1) // Departman da "Tümü" seçili
                        {
                            EkipUyeleriniYukle(); // Tüm ekibi göster
                        }
                        else
                        {
                            ListeleByDepartman(departmanID); // Sadece o departmanı göster
                        }
                    }
                }
                else
                {
                    ListeleByBolum(bolumID);
                }
            }
        }

        private void ListeleByBolum(int bolumID)
        {
            try
            {
                baglanti.BaglantiAc();

                string sorgu = @"
            SELECT 
                k.KullaniciID, 
                k.Ad, 
                k.Soyad, 
                k.Email, 
                d.DepartmanAdi, 
                b.BolumAdi
            FROM Kullanici k
            INNER JOIN Bolum b ON k.BolumID = b.BolumID
            INNER JOIN Departman d ON b.BagliDepartmanID = d.DepartmanID
            WHERE k.BolumID = @BolumID AND k.YoneticiID = @YoneticiID";

                using (SqlCommand cmd = new SqlCommand(sorgu, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@BolumID", bolumID);
                    cmd.Parameters.AddWithValue("@YoneticiID", Convert.ToInt32(yoneticiId));

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridEkip.DataSource = dt;
                        SetColumnHeaders();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bölüme göre ekip listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void ListeleByDepartman(int departmanID)
        {
            try
            {
                baglanti.BaglantiAc();

                string sorgu = @"
            SELECT 
                k.KullaniciID, 
                k.Ad, 
                k.Soyad, 
                k.Email, 
                d.DepartmanAdi, 
                b.BolumAdi
            FROM Kullanici k
            INNER JOIN Bolum b ON k.BolumID = b.BolumID
            INNER JOIN Departman d ON b.BagliDepartmanID = d.DepartmanID
            WHERE d.DepartmanID = @DepartmanID AND k.YoneticiID = @YoneticiID";

                using (SqlCommand cmd = new SqlCommand(sorgu, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@DepartmanID", departmanID);
                    cmd.Parameters.AddWithValue("@YoneticiID", Convert.ToInt32(yoneticiId));

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridEkip.DataSource = dt;
                        SetColumnHeaders();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Departmana göre ekip listelenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                string aciklama = txtAciklama.Text.Trim();
                baglanti.BaglantiAc();

                // Açıklama güncelleme için direkt SQL kullan
                string updateQuery = "UPDATE Cagri SET CagriAciklama = @Aciklama WHERE CagriID = @CagriID";
                using (SqlCommand cmd = new SqlCommand(updateQuery, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@Aciklama", string.IsNullOrWhiteSpace(aciklama) ? (object)DBNull.Value : aciklama);
                    cmd.Parameters.AddWithValue("@CagriID", cagriId);
                    cmd.ExecuteNonQuery();
                }

                // Durum güncelleme
                Call.CagriDurumGuncelle(cagriId, "Açıklama Güncellendi", aciklama, Convert.ToInt32(yoneticiId), baglanti.conn, Convert.ToInt32(yoneticiId));

                MessageBox.Show("Açıklama güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                string sorgu = @"
        SELECT 
            k.KullaniciID, 
            k.Ad, 
            k.Soyad, 
            k.Email, 
            d.DepartmanAdi, 
            b.BolumAdi
        FROM Kullanici k
        INNER JOIN Bolum b ON k.BolumID = b.BolumID
        INNER JOIN Departman d ON b.BagliDepartmanID = d.DepartmanID
        WHERE k.YoneticiID = @YoneticiID";

                using (SqlCommand komut = new SqlCommand(sorgu, baglanti.conn))
                {
                    komut.Parameters.AddWithValue("@YoneticiID", Convert.ToInt32(yoneticiId));

                    using (SqlDataAdapter da = new SqlDataAdapter(komut))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridEkip.DataSource = dt;
                        SetColumnHeaders();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekip üyeleri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void SetColumnHeaders()
        {
            if (dataGridEkip.Columns.Contains("KullaniciID"))
            {
                dataGridEkip.Columns["KullaniciID"].HeaderText = "Kullanıcı ID";
                dataGridEkip.Columns["Ad"].HeaderText = "Ad";
                dataGridEkip.Columns["Soyad"].HeaderText = "Soyad";
                dataGridEkip.Columns["Email"].HeaderText = "Email";
                dataGridEkip.Columns["DepartmanAdi"].HeaderText = "Departman";
                dataGridEkip.Columns["BolumAdi"].HeaderText = "Bölüm";
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
        }

        private void btnAta_Click(object sender, EventArgs e)
        {
            if (dataGridEkip.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int atananKullaniciId = Convert.ToInt32(dataGridEkip.SelectedRows[0].Cells["KullaniciID"].Value);

            try
            {
                baglanti.BaglantiAc();

                // 1. Çağrıyı kullanıcıya ata
                string sorgu = "UPDATE Cagri SET AtananKullaniciID = @atananId, Durum = @durum WHERE CagriID = @cagriId";
                using (SqlCommand cmd = new SqlCommand(sorgu, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@atananId", atananKullaniciId);
                    cmd.Parameters.AddWithValue("@durum", "Atandı");
                    cmd.Parameters.AddWithValue("@cagriId", cagriId);

                    int etkilenen = cmd.ExecuteNonQuery();
                    if (etkilenen > 0)
                    {
                        // 2. Çağrı durum geçmişine kayıt
                        string aciklama = $"Çağrı {atananKullaniciId} ID'li kullanıcıya atandı.";
                        int kullaniciId = Convert.ToInt32(yoneticiId);

                        Call.CagriDurumGuncelle(cagriId, "Görev Atandı", aciklama, kullaniciId, baglanti.conn, kullaniciId);

                        MessageBox.Show("Çağrı başarıyla atandı ve durumu güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                        ManagerHomepage managerHomepage = new ManagerHomepage();
                        managerHomepage.Show();
                    }
                    else
                    {
                        MessageBox.Show("Atama başarısız oldu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        // Form kapatılırken bağlantıları temizle
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                baglanti?.BaglantiKapat();
            }
            catch { }
            base.OnFormClosed(e);
        }
    }
}