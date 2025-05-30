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
                Visible = false,
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
                Name = "YoneticiID",
                HeaderText = "Yönetici",
                DataPropertyName = "YoneticiID",
                Width = 100,
            });

            dataGridViewKullanicilar.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Adres",
                HeaderText = "Adres",
                DataPropertyName = "Adres",
                Width = 100,
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
                    k.YoneticiID,
                    k.Adres,
                    ISNULL(d.DepartmanAdi, 'Departman Yok') as Departman,
                    k.Telefon,
                    k.Cinsiyet,
                    k.DogumTar,
                    k.IseBaslamaTar
                FROM Kullanici k
                LEFT JOIN Departman d ON k.DepartmanID = d.DepartmanID
                ORDER BY k.KullaniciID";

                DataTable dt = new DataTable();

                using (SqlConnection conn = Baglanti.BaglantiGetir())
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
                    k.YoneticiID,
                    k.Adres,
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
                     OR k.Telefon LIKE @arama
                        OR k.Cinsiyet LIKE @arama
                        OR k.Adres LIKE @arama
                        OR k.DogumTar LIKE @arama
                        OR k.IseBaslamaTar LIKE @arama
                        OR k.Sifre LIKE @arama
                        OR k.YoneticiID LIKE @arama
                        OR k.KullaniciID LIKE @arama
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
        private void DepartmanlariYukle(int? secilecekDepartmanID = null)
        {
            try
            {
                // Event'i geçici olarak kapat
                cmbDepartman.SelectedIndexChanged -= cmbDepartman_SelectedIndexChanged;

                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    SqlCommand cmd = new SqlCommand("SELECT DepartmanID, DepartmanAdi FROM Departman ORDER BY DepartmanAdi", conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Yeni DataTable oluştur
                    DataTable newDt = new DataTable();
                    newDt.Columns.Add("DepartmanID", typeof(int));
                    newDt.Columns.Add("DepartmanAdi", typeof(string));

                    // "Tümü" seçeneği ekle
                    DataRow tumRow = newDt.NewRow();
                    tumRow["DepartmanID"] = 0;
                    tumRow["DepartmanAdi"] = "Tümü";
                    newDt.Rows.Add(tumRow);

                    // Diğer departmanları ekle
                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow newRow = newDt.NewRow();
                        newRow["DepartmanID"] = row["DepartmanID"];
                        newRow["DepartmanAdi"] = row["DepartmanAdi"];
                        newDt.Rows.Add(newRow);
                    }

                    cmbDepartman.DataSource = newDt;
                    cmbDepartman.DisplayMember = "DepartmanAdi";
                    cmbDepartman.ValueMember = "DepartmanID";

                    // Seçim yap
                    if (secilecekDepartmanID.HasValue)
                    {
                        // Güvenli seçim
                        foreach (DataRowView item in cmbDepartman.Items)
                        {
                            if (Convert.ToInt32(item["DepartmanID"]) == secilecekDepartmanID.Value)
                            {
                                cmbDepartman.SelectedItem = item;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // İlk seçenek "Tümü" olacak
                        cmbDepartman.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Departmanlar yüklenirken hata oluştu: {ex.Message}",
                              "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Event'i geri aç
                cmbDepartman.SelectedIndexChanged += cmbDepartman_SelectedIndexChanged;
            }
        }



        private void cmbDepartman_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ComboBox seçimi yoksa çık
            if (cmbDepartman.SelectedIndex == -1 || cmbDepartman.SelectedItem == null)
            {
                return;
            }

            try
            {
                // DataRowView'dan doğru şekilde veri al
                DataRowView selectedRow = (DataRowView)cmbDepartman.SelectedItem;
                int selectedDepartmanID = Convert.ToInt32(selectedRow["DepartmanID"]);
                string departmanAdi = selectedRow["DepartmanAdi"].ToString();

                // Eğer "Tümü" seçildiyse (ID = 0) tüm kullanıcıları yükle
                if (selectedDepartmanID == 0)
                {
                    KullanicilariYukle();
                    lblKullaniciSayisi.Text = $"Toplam Kullanıcı: {dataGridViewKullanicilar.Rows.Count}";
                    return;
                }

                // Belirli departman seçildiyse filtrele
                string query = @"
            SELECT 
                k.KullaniciID,
                k.Ad,
                k.Soyad,
                k.Email,
                k.Sifre,
                k.Rol,
                k.YoneticiID,
                k.Adres,
                ISNULL(d.DepartmanAdi, 'Departman Yok') as Departman,
                k.Telefon,
                k.Cinsiyet,
                k.DogumTar,
                k.IseBaslamaTar
            FROM Kullanici k
            LEFT JOIN Departman d ON k.DepartmanID = d.DepartmanID
            WHERE k.DepartmanID = @departmanID
            ORDER BY k.KullaniciID";

                DataTable dt = new DataTable();

                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@departmanID", selectedDepartmanID);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }

                dataGridViewKullanicilar.DataSource = dt;
                lblKullaniciSayisi.Text = $"{departmanAdi} Departmanı: {dt.Rows.Count} kullanıcı";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Departman filtresi uygulanırken hata oluştu: {ex.Message}",
                              "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                KullanicilariYukle(); // Hata durumunda tüm kullanıcıları yükle
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
            cmbDepartman.SelectedIndex = 0;
            txtArama.Text = "Ara...";
            txtArama.ForeColor = Color.Gray;
            KullanicilariYukle();
        }

        private void btnDuzenle_Click(object sender, EventArgs e)
        {
            if (dataGridViewKullanicilar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlemek istediğiniz kullanıcıyı seçin!",
                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow selectedRow = dataGridViewKullanicilar.SelectedRows[0];
                string rol = selectedRow.Cells["Rol"].Value?.ToString() ?? "";
                int kullaniciID = Convert.ToInt32(selectedRow.Cells["KullaniciID"].Value);

                // En üst düzey yönetici kontrolü (YoneticiID null olan)
                bool enUstDuzeyYonetici = selectedRow.Cells["YoneticiID"].Value == null ||
                                          selectedRow.Cells["YoneticiID"].Value == DBNull.Value;

                // Güvenlik kontrolleri
                if (rol.ToLower().Contains("ekip yöneticisi") || rol.ToLower().Contains("ekip yoneticisi"))
                {
                    if (!YoneticiSifreKontrolu())
                    {
                        MessageBox.Show("Ekip Yöneticisi hesabını düzenlemek için yönetici şifresi gereklidir.",
                                      "Güvenlik Kontrolü", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (rol.ToLower().Contains("müdür") || rol.ToLower().Contains("mudur"))
                {
                    // Müdür rolü için özel kontrol
                    if (enUstDuzeyYonetici)
                    {
                        // En üst düzey müdür - özel yetki kontrolü
                        DialogResult result = MessageBox.Show(
                            "Bu kullanıcı en üst düzey yöneticidir (YoneticiID boş).\n" +
                            "Bu hesabı düzenlemek sistem güvenliği açısından riskli olabilir.\n\n" +
                            "Devam etmek istediğinizden emin misiniz?",
                            "Kritik Yetki Uyarısı",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (!YoneticiSifreKontrolu())
                    {
                        MessageBox.Show("Müdür hesabını düzenlemek için yönetici şifresi gereklidir.",
                                      "Güvenlik Kontrolü", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Kullanıcı bilgilerini al ve EditUsers formunu aç
                string ad = selectedRow.Cells["Ad"].Value?.ToString() ?? "";
                string soyad = selectedRow.Cells["Soyad"].Value?.ToString() ?? "";
                string email = selectedRow.Cells["Email"].Value?.ToString() ?? "";
                string sifre = selectedRow.Cells["Sifre"].Value?.ToString() ?? "";
                string adres = selectedRow.Cells["Adres"].Value?.ToString() ?? "";

                int? yoneticiID = selectedRow.Cells["YoneticiID"].Value != null &&
                                  selectedRow.Cells["YoneticiID"].Value != DBNull.Value ?
                                  Convert.ToInt32(selectedRow.Cells["YoneticiID"].Value) : (int?)null;

                int? departmanID = null;
                try
                {
                    using (SqlConnection conn = Baglanti.BaglantiGetir())
                    {
                        string query = "SELECT DepartmanID FROM Kullanici WHERE KullaniciID = @kullaniciID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@kullaniciID", kullaniciID);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            departmanID = Convert.ToInt32(result);
                        }
                    }
                }
                catch (Exception dbEx)
                {
                    MessageBox.Show($"Departman ID alınırken hata: {dbEx.Message}");
                }

                string telefon = selectedRow.Cells["Telefon"].Value?.ToString() ?? "";
                string cinsiyet = selectedRow.Cells["Cinsiyet"].Value?.ToString() ?? "";
                DateTime? dogumTar = selectedRow.Cells["DogumTar"].Value != null ?
                                     Convert.ToDateTime(selectedRow.Cells["DogumTar"].Value) : (DateTime?)null;
                DateTime? iseBaslamaTar = selectedRow.Cells["IseBaslamaTar"].Value != null ?
                                          Convert.ToDateTime(selectedRow.Cells["IseBaslamaTar"].Value) : (DateTime?)null;

                // EditUsers formunu aç
                EditUsers editUsers = new EditUsers(
                    kullaniciID, ad, soyad, email, sifre, rol,
                    adres, yoneticiID, departmanID, telefon, cinsiyet, dogumTar, iseBaslamaTar
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

        private bool YoneticiSifreKontrolu()
        {
            using (var sifreForm = new Form())
            {
                sifreForm.Text = "Yönetici Şifre Kontrolü";
                sifreForm.Size = new Size(350, 180);
                sifreForm.StartPosition = FormStartPosition.CenterParent;
                sifreForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                sifreForm.MaximizeBox = false;
                sifreForm.MinimizeBox = false;

                var lblBilgi = new Label
                {
                    Text = "Müdür/Yönetici işlemleri için şifrenizi girin:",
                    Location = new Point(20, 20),
                    Size = new Size(300, 20),
                    Font = new Font("Century Gothic", 9)
                };

                var txtSifre = new TextBox
                {
                    Location = new Point(20, 50),
                    Size = new Size(280, 25),
                    UseSystemPasswordChar = true,
                    Font = new Font("Century Gothic", 10)
                };

                var btnTamam = new Button
                {
                    Text = "Tamam",
                    Location = new Point(140, 90),
                    Size = new Size(75, 30),
                    DialogResult = DialogResult.OK,
                    Font = new Font("Century Gothic", 9)
                };

                var btnIptal = new Button
                {
                    Text = "İptal",
                    Location = new Point(225, 90),
                    Size = new Size(75, 30),
                    DialogResult = DialogResult.Cancel,
                    Font = new Font("Century Gothic", 9)
                };

                sifreForm.Controls.AddRange(new Control[] { lblBilgi, txtSifre, btnTamam, btnIptal });
                sifreForm.AcceptButton = btnTamam;
                sifreForm.CancelButton = btnIptal;

                txtSifre.Focus();

                if (sifreForm.ShowDialog() == DialogResult.OK)
                {
                    string girilenSifre = txtSifre.Text.Trim();

                    if (string.IsNullOrEmpty(girilenSifre))
                    {
                        MessageBox.Show("Şifre girilmedi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    bool sonuc = SifreKontrolEt(girilenSifre);

                    if (!sonuc)
                    {
                        MessageBox.Show("Hatalı şifre girdiniz.", "Şifre Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return sonuc;
                }

                // Kullanıcı 'İptal' derse
                return false;
            }
        }

        private bool SifreKontrolEt(string girilenSifre)
        {
            try
            {
                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    string mevcutKullaniciID = KullaniciBilgi.KullaniciID; // Giriş yapan kullanıcının ID'si

                    string query = "SELECT Sifre FROM Kullanici WHERE KullaniciID = @kullaniciID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@kullaniciID", mevcutKullaniciID);

                        object result = cmd.ExecuteScalar();

                        if (result == DBNull.Value || result == null)
                        {
                            MessageBox.Show("Kullanıcı bulunamadı veya şifre NULL.");
                            return false;
                        }

                        string veritabaniSifre = result.ToString();
                        return girilenSifre == veritabaniSifre;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şifre kontrolü yapılırken hata: {ex.Message}");
                return false;
            }
        }



        private int GetCurrentUserID()
        {
                return Properties.Settings.Default["CurrentUserID"] != null
                ? Convert.ToInt32(Properties.Settings.Default["CurrentUserID"])
                : -1; // Varsayılan değer  
        }



        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dataGridViewKullanicilar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz kullanıcıyı seçin!",
                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow selectedRow = dataGridViewKullanicilar.SelectedRows[0];
                int kullaniciID = Convert.ToInt32(selectedRow.Cells["KullaniciID"].Value);
                string ad = selectedRow.Cells["Ad"].Value?.ToString() ?? "";
                string soyad = selectedRow.Cells["Soyad"].Value?.ToString() ?? "";
                string rol = selectedRow.Cells["Rol"].Value?.ToString() ?? "";

                // En üst düzey yönetici kontrolü
                bool enUstDuzeyYonetici = selectedRow.Cells["YoneticiID"].Value == null ||
                                          selectedRow.Cells["YoneticiID"].Value == DBNull.Value;

                int mevcutKullaniciID = GetCurrentUserID();
                if (kullaniciID == mevcutKullaniciID)
                {
                    MessageBox.Show("Kendi hesabınızı silemezsiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (enUstDuzeyYonetici && (rol.ToLower().Contains("müdür") || rol.ToLower().Contains("mudur")))
                {
                    MessageBox.Show(
                        "Bu kullanıcı en üst düzey yöneticidir (YoneticiID boş).\n" +
                        "Sistem güvenliği açısından bu hesap silinemez.\n\n" +
                        "Bu hesabı silmek için sistem yöneticinizle iletişime geçin.",
                        "Kritik Hesap Koruması",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop);
                    return;
                }

                // Onay penceresi
                DialogResult result = MessageBox.Show(
                    $"'{ad} {soyad}' kullanıcısını silmek istediğinizden emin misiniz?\n\n" +
                    $"Rol: {rol}\n" +
                    $"Bu işlem geri alınamaz!",
                    "Kullanıcı Silme Onayı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Yönetici rolleri için ek güvenlik
                    if (rol.ToLower().Contains("müdür") || rol.ToLower().Contains("mudur") ||
                        rol.ToLower().Contains("ekip yöneticisi") || rol.ToLower().Contains("ekip yoneticisi"))
                    {
                        if (!YoneticiSifreKontrolu())
                        {
                            MessageBox.Show("Yönetici hesabını silmek için yönetici şifresi gereklidir.",
                                          "Güvenlik", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    using (SqlConnection conn = Baglanti.BaglantiGetir())
                    {
                        // Önce bağımlı kayıtları kontrol et
                        string kontrolQuery = @"
                    SELECT COUNT(*) FROM Cagri WHERE AtananKullaniciID = @kullaniciID
                    UNION ALL
                    SELECT COUNT(*) FROM Kullanici WHERE YoneticiID = @kullaniciID";

                        SqlCommand kontrolCmd = new SqlCommand(kontrolQuery, conn);
                        kontrolCmd.Parameters.AddWithValue("@kullaniciID", kullaniciID);

                        using (SqlDataReader reader = kontrolCmd.ExecuteReader())
                        {
                            int gorevSayisi = 0;
                            int astSayisi = 0;

                            if (reader.Read())
                                gorevSayisi = reader.GetInt32(0);
                            if (reader.Read())
                                astSayisi = reader.GetInt32(0);

                            reader.Close();

                            if (gorevSayisi > 0 || astSayisi > 0)
                            {
                                string mesaj = "Bu kullanıcı silinemez çünkü:\n";
                                if (gorevSayisi > 0)
                                    mesaj += $"• {gorevSayisi} adet görev atanmış\n";
                                if (astSayisi > 0)
                                    mesaj += $"• {astSayisi} adet alt çalışanı var\n";
                                mesaj += "\nÖnce bu bağımlılıkları çözün.";

                                MessageBox.Show(mesaj, "Silme İşlemi Engellenmiştir",
                                              MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                        }

                        // Kullanıcıyı sil
                        string deleteQuery = "DELETE FROM Kullanici WHERE KullaniciID = @kullaniciID";
                        SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                        deleteCmd.Parameters.AddWithValue("@kullaniciID", kullaniciID);

                        int etkilenenSatir = deleteCmd.ExecuteNonQuery();

                        if (etkilenenSatir > 0)
                        {
                            MessageBox.Show($"'{ad} {soyad}' kullanıcısı başarıyla silindi.",
                                          "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Listeyi yenile
                            KullanicilariYukle();
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı silinemedi. Lütfen tekrar deneyin.",
                                          "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı silinirken hata oluştu: {ex.Message}",
                              "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            EditUsers editUsers = new EditUsers();
            editUsers.Show();
            this.Close();
        }
    }
}