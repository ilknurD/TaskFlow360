using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class EditUsers : Form
    {
        private int secilenKullaniciID;
        private bool yeniKayitMi = false;

        public EditUsers()
        {
            InitializeComponent();
            secilenKullaniciID = 0;
            yeniKayitMi = true;
        }

        public EditUsers(int kullaniciID, string ad, string soyad, string email, string sifre, string rol,
            string adres, int? yoneticiID, int? departmanID, string telefon,
            string cinsiyet, DateTime? dogumTar, DateTime? iseBaslamaTar)
        {
            InitializeComponent();
            secilenKullaniciID = kullaniciID;
            yeniKayitMi = false;

            BilgileriDoldur(kullaniciID, ad, soyad, email, sifre, rol, adres, yoneticiID,
                departmanID, telefon, cinsiyet, dogumTar, iseBaslamaTar);
        }

        private void BilgileriDoldur(int kullaniciID, string ad, string soyad, string email, string sifre, string rol,
            string adres, int? yoneticiID, int? departman, string telefon,
            string cinsiyet, DateTime? dogumTar, DateTime? iseBaslamaTar)
        {
            try
            {
                // Önce tüm combobox verileri yüklenir
                DepartmanlariYukle(); // cmbDepartman

                // ROL ComboBox'ını doldur
                cmbRol.Items.Clear();
                cmbRol.Items.AddRange(new string[] { "Çağrı Merkezi", "Ekip Yöneticisi", "Müdür", "Ekip Üyesi" });
                if (!string.IsNullOrEmpty(rol))
                {
                    foreach (var item in cmbRol.Items)
                    {
                        if (item.ToString().Equals(rol.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            cmbRol.SelectedItem = item;
                            break;
                        }
                    }
                }


                // CİNSİYET ComboBox'ını doldur
                cmbCinsiyet.Items.Clear();
                cmbCinsiyet.Items.AddRange(new string[] { "Kadın", "Erkek" });

                // Rol seçimi (büyük/küçük harf ve boşluklara duyarsız)


                // Cinsiyet seçimi
                if (!string.IsNullOrEmpty(cinsiyet))
                {
                    foreach (var item in cmbCinsiyet.Items)
                    {
                        if (item.ToString().Equals(cinsiyet.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            cmbCinsiyet.SelectedItem = item;
                            break;
                        }
                    }
                }

                // Temel bilgileri doldur
                lblKullaniciID.Text = kullaniciID.ToString();
                txtAd.Text = ad;
                txtSoyad.Text = soyad;
                txtEmail.Text = email;
                txtSifre.Text = sifre;
                txtAdres.Text = adres;
                txtTelefon.Text = telefon;

                if (!string.IsNullOrEmpty(rol))
                {
                    cmbRol.SelectedItem = rol; // Doğrudan seçim yap
                }

                // Cinsiyet seçimi
                if (!string.IsNullOrEmpty(cinsiyet))
                {
                    cmbCinsiyet.SelectedItem = cinsiyet; // Doğrudan seçim yap
                }

                // Rol seçildikten SONRA yöneticileri yükle
                Application.DoEvents(); // UI güncellemelerini bekle
                YoneticileriYukle();

                // YÖNETİCİ seçimi
                if (yoneticiID.HasValue && cmbYonetici.DataSource != null)
                {
                    cmbYonetici.SelectedValue = yoneticiID.Value;

                    // Seçim başarısız olduysa kontrol et
                    if (cmbYonetici.SelectedIndex == -1)
                    {
                        MessageBox.Show($"Yönetici atanamadı: ID = {yoneticiID.Value}", "Uyarı");
                    }
                }
                else if (!yoneticiID.HasValue)
                {
                    // Yönetici yoksa -1 seç
                    cmbYonetici.SelectedIndex = -1;
                }

                // DEPARTMAN seçimi - DataSource kullanıldığı için SelectedValue ile yapılmalı
                if (departman.HasValue && cmbDepartman.DataSource != null)
                {
                    cmbDepartman.SelectedValue = departman.Value;
                }
                Application.DoEvents(); // UI güncellemeleri için
                YoneticileriYukle(); // Yöneticileri yükle

                // TARİHLER
                if (dogumTar.HasValue)
                    dtpDogumTarihi.Value = dogumTar.Value;
                else
                    dtpDogumTarihi.Value = DateTime.Now; // Varsayılan değer

                if (iseBaslamaTar.HasValue)
                    dtpIseBaslamaTarihi.Value = iseBaslamaTar.Value;
                else
                    dtpIseBaslamaTarihi.Value = DateTime.Now; // Varsayılan değer
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bilgi doldurma hatası: " + ex.Message);
            }
        }




        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (cmbDepartman.SelectedValue == null)
            {
                MessageBox.Show("Lütfen bir departman seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int departmanID = Convert.ToInt32(cmbDepartman.SelectedValue);
            int? yoneticiID = null;

            if (cmbRol.Text == "Ekip Yöneticisi" || cmbRol.Text == "Çağrı Merkezi")
            {
                yoneticiID = MüdürIDBul();
            }
            else if (cmbRol.Text == "Ekip Üyesi")
            {
                yoneticiID = cmbYonetici.SelectedValue != null ? (int?)Convert.ToInt32(cmbYonetici.SelectedValue) : null;
            }

            try
            {
                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    SqlCommand cmd;

                    if (yeniKayitMi)
                    {
                        string insertQuery = @"
                            INSERT INTO Kullanici 
                                (Ad, Soyad, Email, Sifre, Rol, Adres, YoneticiID, DepartmanID, Telefon, Cinsiyet, DogumTar, IseBaslamaTar)
                            VALUES 
                                (@ad, @soyad, @email, @sifre, @rol, @adres, @yoneticiID, @departmanID, @telefon, @cinsiyet, @dogumTar, @iseBaslamaTar)";
                        cmd = new SqlCommand(insertQuery, conn);
                    }
                    else
                    {
                        string updateQuery = @"
                            UPDATE Kullanici SET
                                Ad = @ad,
                                Soyad = @soyad,
                                Email = @email,
                                Sifre = @sifre,
                                Rol = @rol,
                                Adres = @adres,
                                YoneticiID = @yoneticiID,
                                DepartmanID = @departmanID,
                                Telefon = @telefon,
                                Cinsiyet = @cinsiyet,
                                DogumTar = @dogumTar,
                                IseBaslamaTar = @iseBaslamaTar
                            WHERE KullaniciID = @kullaniciID";
                        cmd = new SqlCommand(updateQuery, conn);
                        cmd.Parameters.AddWithValue("@kullaniciID", secilenKullaniciID);
                    }

                    cmd.Parameters.AddWithValue("@ad", txtAd.Text.Trim());
                    cmd.Parameters.AddWithValue("@soyad", txtSoyad.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@sifre", txtSifre.Text.Trim());
                    cmd.Parameters.AddWithValue("@rol", cmbRol.Text);
                    cmd.Parameters.AddWithValue("@adres", txtAdres.Text.Trim());
                    cmd.Parameters.AddWithValue("@yoneticiID", yoneticiID ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@departmanID", departmanID);
                    cmd.Parameters.AddWithValue("@telefon", txtTelefon.Text.Trim());
                    cmd.Parameters.AddWithValue("@cinsiyet", cmbCinsiyet.Text);
                    cmd.Parameters.AddWithValue("@dogumTar", dtpDogumTarihi.Value.Date);
                    cmd.Parameters.AddWithValue("@iseBaslamaTar", dtpIseBaslamaTarihi.Value.Date);

                    int sonuc = cmd.ExecuteNonQuery();

                    if (sonuc > 0)
                    {
                        MessageBox.Show(yeniKayitMi ? "Yeni kullanıcı eklendi." : "Kullanıcı bilgileri güncellendi.",
                                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        BossUsersControl bossUsersControl = new BossUsersControl();
                        bossUsersControl.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("İşlem başarısız oldu!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DepartmanlariYukle(int? secilecekDepartmanID = null)
        {
            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                SqlCommand cmd = new SqlCommand("SELECT DepartmanID, DepartmanAdi FROM Departman", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbDepartman.DataSource = dt;
                cmbDepartman.DisplayMember = "DepartmanAdi";
                cmbDepartman.ValueMember = "DepartmanID";
                if (secilecekDepartmanID.HasValue)
                {
                    cmbDepartman.SelectedValue = secilecekDepartmanID.Value;
                }
            }
        }

        private void YoneticileriYukle()
        {
            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("KullaniciID", typeof(int));
                dt.Columns.Add("YoneticiAdi", typeof(string));

                if (cmbRol.Text == "Ekip Üyesi")
                {
                    // Sadece Ekip Yöneticilerini getir
                    string query = "SELECT KullaniciID, Ad + ' ' + Soyad AS YoneticiAdi FROM Kullanici WHERE Rol = 'Ekip Yöneticisi'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    cmbYonetici.Enabled = true;
                }
                else if (cmbRol.Text == "Ekip Yöneticisi" || cmbRol.Text == "Çağrı Merkezi" || cmbRol.Text=="Müdür")
                {
                    // Müdür ID'sini ekle
                    int mudurID = MüdürIDBul();
                    if (mudurID > 0)
                    {
                        DataRow mudurRow = dt.NewRow();
                        mudurRow["KullaniciID"] = mudurID;
                        mudurRow["YoneticiAdi"] = "Müdür (Otomatik)";
                        dt.Rows.Add(mudurRow);
                    }

                    cmbYonetici.Enabled = false; // Pasif yap
                }
                else // Müdür
                {
                    // Müdürün yöneticisi yok
                    cmbYonetici.Enabled = false;
                }

                cmbYonetici.DataSource = dt;
                cmbYonetici.DisplayMember = "YoneticiAdi";
                cmbYonetici.ValueMember = "KullaniciID";

                // Otomatik atama
                if (cmbRol.Text == "Ekip Yöneticisi" || cmbRol.Text == "Çağrı Merkezi")
                {
                    int mudurID = MüdürIDBul();
                    if (mudurID > 0)
                    {
                        cmbYonetici.SelectedValue = mudurID;
                    }
                }
                else if (cmbRol.Text == "Müdür")
                {
                    cmbYonetici.SelectedIndex = -1;
                }
            }
        }

        private int MüdürIDBul()
        {
            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                string query = "SELECT KullaniciID FROM Kullanici WHERE Rol = 'Müdür'";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private void EditUsers_Load(object sender, EventArgs e)
        {
            if (cmbRol.Items.Count == 0) // Sadece boşsa ekle (gereksiz tekrarları önle)
            {
                cmbRol.Items.AddRange(new string[] { "Çağrı Merkezi", "Ekip Yöneticisi", "Müdür", "Ekip Üyesi" });
                cmbCinsiyet.Items.AddRange(new string[] { "Kadın", "Erkek" });
            }
            if (yeniKayitMi)
            {
                DepartmanlariYukle();
                YoneticileriYukle();
                lblKullaniciID.Text = "Yeni Kayıt";
            }
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            BossUsersControl bossUsersControl = new BossUsersControl();
            bossUsersControl.Show();
            this.Close();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            YoneticileriYukle();
        }
    }
}