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
    public partial class BossUsersControl : Form
    {
        Baglanti baglanti = new Baglanti();
        public BossUsersControl()
        {
            InitializeComponent();
            Sutunlar();
            DepartmanlariYukle();
            KullanicilariYukle();
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
            BossProfile bossProfile = new BossProfile();
            bossProfile.Show();
            this.Close();
        }

        private void btnKullaniciIslem_Click(object sender, EventArgs e)
        {
            BossUsersControl kullaniciIslem = new BossUsersControl();
            kullaniciIslem.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {

        }
        private void Sutunlar()
        {
            dataGridViewKullanicilar.AutoGenerateColumns = false;
            dataGridViewKullanicilar.AllowUserToAddRows = false;
            dataGridViewKullanicilar.AllowUserToDeleteRows = false;
            dataGridViewKullanicilar.ReadOnly = true;
            dataGridViewKullanicilar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Sütunlar
            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "KullaniciID",
                HeaderText = "ID",
                DataPropertyName = "KullaniciID",
                Width = 60
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Ad",
                HeaderText = "Ad",
                DataPropertyName = "Ad",
                Width = 100
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Soyad",
                HeaderText = "Soyad",
                DataPropertyName = "Soyad",
                Width = 100
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email",
                DataPropertyName = "Email",
                Width = 150
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Sifre",
                HeaderText = "Şifre",
                DataPropertyName = "Sifre",
                Width = 80
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Rol",
                HeaderText = "Rol",
                DataPropertyName = "Rol",
                Width = 120
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Maas",
                HeaderText = "Maaş",
                DataPropertyName = "Maas",
                Width = 100,
                DefaultCellStyle = { Format = "N2" } // Para formatı
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Prim",
                HeaderText = "Prim",
                DataPropertyName = "Prim",
                Width = 100,
                DefaultCellStyle = { Format = "N2" } // Para formatı
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Departman",
                HeaderText = "Departman",
                DataPropertyName = "Departman",
                Width = 100
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Telefon",
                HeaderText = "Telefon",
                DataPropertyName = "Telefon",
                Width = 120
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cinsiyet",
                HeaderText = "Cinsiyet",
                DataPropertyName = "Cinsiyet",
                Width = 80
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DogumTar",
                HeaderText = "Doğum Tarihi",
                DataPropertyName = "DogumTar",
                Width = 120,
                DefaultCellStyle = { Format = "dd.MM.yyyy" } // Tarih formatı
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IseBaslamaTar",
                HeaderText = "İşe Başlama",
                DataPropertyName = "IseBaslamaTar",
                Width = 120,
                DefaultCellStyle = { Format = "dd.MM.yyyy" } // Tarih formatı
            });
        }
        private void KullanicilariYukle()
        {
            dataGridViewKullanicilar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewKullanicilar.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridViewKullanicilar.ScrollBars = ScrollBars.Both;
            try
            {
                string query = @"
                SELECT 
                    k.KullaniciID,
                    k.Ad,
                    k.Soyad,
                    k.Email,
                    k.Sifre,
                    k.Rol,
                    k.Maas,
                    k.Prim,
                    ISNULL(d.DepartmanAdi, 'Departman Yok') as Departman,
                    k.Telefon,
                    k.Cinsiyet,
                    k.DogumTar,
                    k.IseBaslamaTar
                FROM Kullanici k
                LEFT JOIN Departman d ON k.DepartmanID = d.DepartmanID
                ORDER BY k.KullaniciID";

                DataTable dt = new DataTable();

                using (SqlConnection conn = Baglanti.BaglantiGetir()) // Bağlantı sınıfınızın metodunu kullanın
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                dataGridViewKullanicilar.DataSource = dt;

                // Satır sayısını göster
                lblKullaniciSayisi.Text = $"Toplam Kullanıcı: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcılar yüklenirken hata oluştu: {ex.Message}",
                              "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtArama_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = txtArama.Text.Trim();

            if (string.IsNullOrEmpty(aramaMetni))
            {
                KullanicilariYukle();
                return;
            }

            try
            {
                string query = @"
                SELECT 
                    k.KullaniciID,
                    k.Ad,
                    k.Soyad,
                    k.Email,
                    k.Sifre,
                    k.Rol,
                    k.Maas,
                    k.Prim,
                    ISNULL(d.DepartmanAdi, 'Departman Yok') as Departman,
                    k.Telefon,
                    k.Cinsiyet,
                    k.DogumTar,
                    k.IseBaslamaTar
                FROM Kullanici k
                LEFT JOIN Departman d ON k.DepartmanID = d.DepartmanID
                WHERE k.Ad LIKE @arama 
                   OR k.Soyad LIKE @arama 
                   OR k.Email LIKE @arama 
                   OR k.Rol LIKE @arama
                   OR d.DepartmanAdi LIKE @arama
                ORDER BY k.KullaniciID";

                DataTable dt = new DataTable();

                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@arama", $"%{aramaMetni}%");
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                dataGridViewKullanicilar.DataSource = dt;
                lblKullaniciSayisi.Text = $"Bulunan Kullanıcı: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Arama yapılırken hata oluştu: {ex.Message}",
                              "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewKullanicilar_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewKullanicilar.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewKullanicilar.SelectedRows[0];

                // Seçili kullanıcının bilgilerini al
                int kullaniciID = Convert.ToInt32(selectedRow.Cells["KullaniciID"].Value);
                string ad = selectedRow.Cells["Ad"].Value?.ToString();
                string soyad = selectedRow.Cells["Soyad"].Value?.ToString();

                // Burada seçili kullanıcı ile ilgili işlemler yapabilirsiniz
                lblSeciliKullanici.Text = $"Seçili: {ad} {soyad} (ID: {kullaniciID})";
            }

        }
        private void DepartmanlariYukle()
        {
            try
            {
                string query = "SELECT DepartmanAdi FROM Departman ORDER BY DepartmanAdi";

                cmbDepartman.Items.Clear();
                cmbDepartman.Items.Add("Tümü");

                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbDepartman.Items.Add(reader["DepartmanAdi"].ToString());
                            }
                        }
                    }
                }

                cmbDepartman.SelectedIndex = 0; // "Tümü" seçili olsun
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Departmanlar yüklenirken hata oluştu: {ex.Message}",
                              "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbDepartman_SelectedIndexChanged(object sender, EventArgs e)
        {
            string secilenDepartman = cmbDepartman.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(secilenDepartman) || secilenDepartman == "Tümü")
            {
                KullanicilariYukle();
                return;
            }

            try
            {
                string query = @"
                SELECT 
                    k.KullaniciID,
                    k.Ad,
                    k.Soyad,
                    k.Email,
                    k.Sifre
                    k.Rol,
                    k.Maas,
                    k.Prim,
                    ISNULL(d.DepartmanAdi, 'Departman Yok') as Departman,
                    k.Telefon,
                    k.Cinsiyet,
                    k.DogumTar,
                    k.IseBaslamaTar
                FROM Kullanici k
                LEFT JOIN Departman d ON k.DepartmanID = d.DepartmanID
                WHERE d.DepartmanAdi = @departman
                ORDER BY k.KullaniciID";

                DataTable dt = new DataTable();

                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@departman", secilenDepartman);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                dataGridViewKullanicilar.DataSource = dt;
                lblKullaniciSayisi.Text = $"{secilenDepartman} Departmanı: {dt.Rows.Count} kullanıcı";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Departman filtresi uygulanırken hata oluştu: {ex.Message}",
                              "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void StilUygula()
        {
            dataGridViewKullanicilar.Font = new Font("Century Gothic", 10);
            dataGridViewKullanicilar.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 11, FontStyle.Bold);
            dataGridViewKullanicilar.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(126, 87, 194); // Mor
            dataGridViewKullanicilar.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewKullanicilar.EnableHeadersVisualStyles = false;

            dataGridViewKullanicilar.DefaultCellStyle.BackColor = Color.White;
            dataGridViewKullanicilar.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewKullanicilar.DefaultCellStyle.SelectionBackColor = Color.FromArgb(179, 157, 219); // Açık mor
            dataGridViewKullanicilar.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridViewKullanicilar.RowHeadersVisible = false;
            dataGridViewKullanicilar.BorderStyle = BorderStyle.None;
            dataGridViewKullanicilar.GridColor = Color.FromArgb(230, 230, 250); // Morumsu grid çizgisi

            dataGridViewKullanicilar.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 240, 255); // Hafif mor ton
        }

        private void BossUsersControl_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn column in dataGridViewKullanicilar.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            StilUygula();
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            cmbDepartman.SelectedIndex = -1;
            txtArama.Text = "Ara...";
            txtArama.ForeColor = Color.Gray;
            KullanicilariYukle();
        }

        private void btnDuzenle_Click(object sender, EventArgs e)
        {
            // Seçili satır kontrolü
            if (dataGridViewKullanicilar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz kullanıcıyı seçin!",
                               "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow selectedRow = dataGridViewKullanicilar.SelectedRows[0];

                // Seçili kullanıcının bilgilerini al
                int kullaniciID = Convert.ToInt32(selectedRow.Cells["KullaniciID"].Value);
                string ad = selectedRow.Cells["Ad"].Value?.ToString() ?? "";
                string soyad = selectedRow.Cells["Soyad"].Value?.ToString() ?? "";
                string email = selectedRow.Cells["Email"].Value?.ToString() ?? "";
                string sifre = selectedRow.Cells["Sifre"].Value?.ToString() ?? ""; // ← ŞİFREYİ EKLEDİK
                string rol = selectedRow.Cells["Rol"].Value?.ToString() ?? "";
                decimal maas = selectedRow.Cells["Maas"].Value != null ?
                              Convert.ToDecimal(selectedRow.Cells["Maas"].Value) : 0;
                decimal prim = selectedRow.Cells["Prim"].Value != null ?
                              Convert.ToDecimal(selectedRow.Cells["Prim"].Value) : 0;
                string departman = selectedRow.Cells["Departman"].Value?.ToString() ?? "";
                string telefon = selectedRow.Cells["Telefon"].Value?.ToString() ?? "";
                string cinsiyet = selectedRow.Cells["Cinsiyet"].Value?.ToString() ?? "";
                DateTime? dogumTar = selectedRow.Cells["DogumTar"].Value != null ?
                                    Convert.ToDateTime(selectedRow.Cells["DogumTar"].Value) : (DateTime?)null;
                DateTime? iseBaslamaTar = selectedRow.Cells["IseBaslamaTar"].Value != null ?
                                         Convert.ToDateTime(selectedRow.Cells["IseBaslamaTar"].Value) : (DateTime?)null;

                // EditUsers formunu oluştur ve bilgileri aktar
                EditUsers editUsers = new EditUsers(
                    kullaniciID, ad, soyad, email, sifre, rol, maas, prim,
                    departman, telefon, cinsiyet, dogumTar, iseBaslamaTar
                );

                editUsers.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı bilgileri alınırken hata oluştu: {ex.Message}",
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnSil_Click(object sender, EventArgs e)
        {

        }
    }
}
