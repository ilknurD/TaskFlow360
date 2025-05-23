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
        Baglanti baglanti = new Baglanti();

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
                baglanti.BaglantiAc();
                EkipUyeleriniYukle();
                LoadDepartmanlar();


                // Cagri sınıfından bilgileri çek
                Cagri seciliCagri = Cagri.CagriGetir(cagriId, baglanti.conn);

                if (seciliCagri != null)
                {
                    txtCagriID.Text = seciliCagri.CagriID.ToString();
                    txtBaslik.Text = seciliCagri.Baslik;
                    txtAciklama.Text = seciliCagri.CagriAciklama;

                    txtCagriID.ReadOnly = true;
                    txtBaslik.ReadOnly = true;
                }
                else
                {
                    MessageBox.Show("Çağrı bilgisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                // DataGridView ayarları
                foreach (DataGridViewColumn column in dataGridEkip.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                dataGridEkip.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridEkip.AllowUserToResizeColumns = false;
                dataGridEkip.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // Sıralamayı kapat
                foreach (DataGridViewColumn column in dataGridEkip.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                // Seçim renkleri
                dataGridEkip.DefaultCellStyle.SelectionBackColor = Color.FromArgb(79, 34, 158);
                dataGridEkip.DefaultCellStyle.SelectionForeColor = Color.White;

                // Başlıklar seçilince renk değişmesin
                dataGridEkip.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridEkip.ColumnHeadersDefaultCellStyle.BackColor;
                dataGridEkip.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridEkip.ColumnHeadersDefaultCellStyle.ForeColor;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Çağrı bilgisi çekilirken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void LoadDepartmanlar()
        {
            try
            {
                baglanti.BaglantiAc();
                string query = "SELECT DepartmanID, DepartmanAdi FROM Departman";
                SqlDataAdapter da = new SqlDataAdapter(query, baglanti.conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboBoxDepartman.DisplayMember = "DepartmanAdi";
                comboBoxDepartman.ValueMember = "DepartmanID";
                comboBoxDepartman.DataSource = dt;
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

            int departmanID = Convert.ToInt32(comboBoxDepartman.SelectedValue);
            LoadBolumler(departmanID);
        }

        private void LoadBolumler(int departmanID)
        {
            try
            {
                baglanti.BaglantiAc();

                string query = "SELECT BolumID, BolumAdi FROM Bolum WHERE BagliDepartmanID = @DepartmanID";
                SqlCommand cmd = new SqlCommand(query, baglanti.conn);
                cmd.Parameters.AddWithValue("@DepartmanID", departmanID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboBoxBolum.DisplayMember = "BolumAdi";
                comboBoxBolum.ValueMember = "BolumID";
                comboBoxBolum.DataSource = dt;
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
                ListeleByBolum(bolumID);
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

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridEkip.DataSource = dt;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bölüme göre ekip listelenirken hata oluştu: " + ex.Message);
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

                Cagri.CagriAciklamaGuncelle(cagriId, string.IsNullOrWhiteSpace(aciklama) ? null : aciklama, baglanti.conn);

                int kullaniciId = Convert.ToInt32(yoneticiId); 

                Cagri.CagriDurumGuncelle(cagriId, "Açıklama Güncellendi", aciklama, Convert.ToInt32(yoneticiId), baglanti.conn, kullaniciId);

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

                    SqlDataAdapter da = new SqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridEkip.DataSource = dt;

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
                SqlCommand cmd = new SqlCommand(sorgu, baglanti.conn);
                cmd.Parameters.AddWithValue("@atananId", atananKullaniciId);
                cmd.Parameters.AddWithValue("@durum", "Atandı"); 
                cmd.Parameters.AddWithValue("@cagriId", cagriId);

                int etkilenen = cmd.ExecuteNonQuery();
                if (etkilenen > 0)
                {
                    // 2. Çağrı durum geçmişine kayıt
                    string aciklama = $"Çağrı {atananKullaniciId} ID'li kullanıcıya atandı.";
                    int kullaniciId = Convert.ToInt32(yoneticiId);

                    Cagri.CagriDurumGuncelle(cagriId, "Görev Atandı", aciklama, kullaniciId, baglanti.conn, kullaniciId);

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
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

    }
}
