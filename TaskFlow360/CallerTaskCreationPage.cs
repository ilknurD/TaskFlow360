﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class CallerTaskCreationPage : Form
    {
        private readonly Logger _logger;
        private const string EMAIL_PATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private const string PHONE_PATTERN = @"^[0-9]{10}$";

        public CallerTaskCreationPage()
        {
            InitializeComponent();
            _logger = new Logger();
            SetupTextBoxes();
        }

        private void SetupTextBoxes()
        {
            // Placeholder metinleri ayarla
            txtTalepAdSoyad.Text = "Ad Soyad";
            txtTalepAdres.Text = "Adres";
            txtTalepTelefon.Text = "5XX XXX XX XX";
            txtTalepMail.Text = "ornek@mail.com";

            // TextBox'ların özelliklerini ayarla
            foreach (TextBox txt in new[] { txtTalepAdSoyad, txtTalepAdres, txtTalepTelefon, txtTalepMail })
            {
                txt.ForeColor = Color.Gray;
                txt.Enter += TextBox_Enter;
                txt.Leave += TextBox_Leave;
            }

            // Telefon ve mail için özel kontroller
            txtTalepTelefon.KeyPress += TxtTalepTelefon_KeyPress;
            txtTalepMail.Leave += TxtTalepMail_Leave;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (txt.Text == "Ad Soyad" || txt.Text == "Adres" || 
                txt.Text == "5XX XXX XX XX" || txt.Text == "ornek@mail.com")
            {
                txt.Text = "";
                txt.ForeColor = Color.Black;
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                switch (txt.Name)
                {
                    case "txtTalepAdSoyad":
                        txt.Text = "Ad Soyad";
                        break;
                    case "txtTalepAdres":
                        txt.Text = "Adres";
                        break;
                    case "txtTalepTelefon":
                        txt.Text = "5XX XXX XX XX";
                        break;
                    case "txtTalepMail":
                        txt.Text = "ornek@mail.com";
                        break;
                }
                txt.ForeColor = Color.Gray;
            }
        }

        private void TxtTalepTelefon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtTalepMail_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTalepMail.Text) && txtTalepMail.Text != "ornek@mail.com")
            {
                if (!Regex.IsMatch(txtTalepMail.Text, EMAIL_PATTERN))
                {
                    MessageBox.Show("Lütfen geçerli bir e-posta adresi giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTalepMail.Focus();
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Sistem", "CallerTaskCreationPage", "Uygulama kapatıldı");
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Sistem", "CallerTaskCreationPage", "Uygulama küçültüldü");
            WindowState = FormWindowState.Minimized;
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Yönlendirme", "CallerTaskCreationPage", "Ana sayfaya yönlendirildi");
            CallerHomepage assistantHomepage = new CallerHomepage();
            assistantHomepage.Show();
            this.Close();
        }

        private void btnCagriTakip_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Yönlendirme", "CallerTaskCreationPage", "Çağrı takip sayfasına yönlendirildi");
            CallerTasks assistantTasks = new CallerTasks();
            assistantTasks.Show();
            this.Close();
        }

        private bool GirdilerGecerliMi()
        {
            if (string.IsNullOrWhiteSpace(txtBaslik.Text) ||
                string.IsNullOrWhiteSpace(richTextAciklama.Text) ||
                txtTalepAdSoyad.Text == "Ad Soyad" ||
                txtTalepAdres.Text == "Adres" ||
                txtTalepTelefon.Text == "5XX XXX XX XX" ||
                txtTalepMail.Text == "ornek@mail.com" ||
                cmbKategori.SelectedIndex == -1 ||
                cmbOncelik.SelectedIndex == -1 ||
                cmbDurum.SelectedIndex == -1 ||
                cmbEkipYoneticisi.SelectedIndex == -1 ||
                cmbDepartman.SelectedIndex == -1 ||
                cmbHedefSure.SelectedIndex == -1)
            {
                _logger.LogEkle("Hata", "CallerTaskCreationPage", "Eksik bilgi girişi tespit edildi");
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Telefon formatı kontrolü
            if (!Regex.IsMatch(txtTalepTelefon.Text, PHONE_PATTERN))
            {
                MessageBox.Show("Lütfen geçerli bir telefon numarası giriniz (10 haneli).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTalepTelefon.Focus();
                return false;
            }

            return true;
        }

        private void EkipYoneticileriniGetir()
        {
            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT KullaniciID, Ad + ' ' + Soyad AS AdSoyad FROM Kullanici WHERE Rol = 'Ekip Yöneticisi'", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbEkipYoneticisi.DataSource = dt;
                cmbEkipYoneticisi.DisplayMember = "AdSoyad";
                cmbEkipYoneticisi.ValueMember = "KullaniciID";
            }
        }
        private void DepartmanlariGetir()
        {
            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT DepartmanID, DepartmanAdi FROM Departman", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbDepartman.DataSource = dt;
                cmbDepartman.DisplayMember = "DepartmanAdi";
                cmbDepartman.ValueMember = "DepartmanID";
            }
        }
        private void StatikVerileriYukle()
        {
            cmbKategori.Items.AddRange(new string[] { "Yazılım", "Donanım", "Ağ", "Rapor", "Sistem", "Diğer" });
            cmbOncelik.Items.AddRange(new string[] { "Düşük", "Orta", "Yüksek" });
            cmbDurum.Items.AddRange(new string[] { "Atandı", "Devam Ediyor", "Beklemede" });
            cmbHedefSure.Items.AddRange(new object[] { 1, 2, 4, 8, 24, 48 });
        }
        private void FormuTemizle()
        {
            txtBaslik.Text = "";
            richTextAciklama.Text = "";
            txtTalepAdSoyad.Text = "Ad Soyad";
            txtTalepAdres.Text = "Adres";
            txtTalepTelefon.Text = "5XX XXX XX XX";
            txtTalepMail.Text = "ornek@mail.com";

            cmbKategori.SelectedIndex = -1;
            cmbOncelik.SelectedIndex = -1;
            cmbDurum.SelectedIndex = -1;
            cmbDepartman.SelectedIndex = -1;
            cmbEkipYoneticisi.SelectedIndex = -1;
            cmbHedefSure.SelectedIndex = -1;

            dtpBitisTarihi.Value = DateTime.Now;
        }

        private void btnOlustur_Click(object sender, EventArgs e)
        {
            if (!GirdilerGecerliMi())
                return;

            string kullaniciID = UserInformation.KullaniciID;

            string talepAdSoyad = txtTalepAdSoyad.Text.Trim();
            string talepAdres = txtTalepAdres.Text.Trim();
            string talepTelefon = txtTalepTelefon.Text.Trim();
            string talepEmail = txtTalepMail.Text.Trim();

            string baslik = txtBaslik.Text.Trim();
            string aciklama = richTextAciklama.Text.Trim();
            string kategori = cmbKategori.SelectedItem?.ToString();
            string oncelik = cmbOncelik.SelectedItem?.ToString();
            string durum = cmbDurum.SelectedItem?.ToString();
            DateTime teslimTarihi = dtpBitisTarihi.Value;
            int hedefSure = Convert.ToInt32(cmbHedefSure.SelectedItem);
            string atananID = cmbEkipYoneticisi.SelectedValue?.ToString();
            int departmanID = Convert.ToInt32(cmbDepartman.SelectedValue);

            using (SqlConnection conn = Connection.BaglantiGetir())
            {
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    SqlCommand cmdTalep = new SqlCommand(@"
                INSERT INTO TalepEdenler (TalepEden, TalepEdenAdres, TalepEdenTelefon, TalepEdenEmail)
                OUTPUT INSERTED.TalepEdenID
                VALUES (@AdSoyad, @Adres, @Telefon, @Email)", conn, trans);

                    cmdTalep.Parameters.AddWithValue("@AdSoyad", talepAdSoyad);
                    cmdTalep.Parameters.AddWithValue("@Adres", talepAdres);
                    cmdTalep.Parameters.AddWithValue("@Telefon", talepTelefon);
                    cmdTalep.Parameters.AddWithValue("@Email", talepEmail);

                    int talepEdenID = (int)cmdTalep.ExecuteScalar();

                    SqlCommand cmdCagri = new SqlCommand(@"
                INSERT INTO Cagri 
                (Baslik, CagriAciklama, CagriKategori, Oncelik, Durum, OlusturmaTarihi, TeslimTarihi, 
                 AtananKullaniciID, OlusturanKullaniciID, HedefSure, TalepEdenID)
                VALUES 
                (@Baslik, @Aciklama, @Kategori, @Oncelik, @Durum, @OlusturmaTarihi, @TeslimTarihi, 
                 @AtananID, @OlusturanID, @HedefSure, @TalepEdenID)", conn, trans);

                    cmdCagri.Parameters.AddWithValue("@Baslik", baslik);
                    cmdCagri.Parameters.AddWithValue("@Aciklama", aciklama);
                    cmdCagri.Parameters.AddWithValue("@Kategori", kategori);
                    cmdCagri.Parameters.AddWithValue("@Oncelik", oncelik);
                    cmdCagri.Parameters.AddWithValue("@Durum", durum);
                    cmdCagri.Parameters.AddWithValue("@OlusturmaTarihi", DateTime.Now);
                    cmdCagri.Parameters.AddWithValue("@TeslimTarihi", teslimTarihi);
                    cmdCagri.Parameters.AddWithValue("@AtananID", atananID);
                    cmdCagri.Parameters.AddWithValue("@OlusturanID", kullaniciID);
                    cmdCagri.Parameters.AddWithValue("@HedefSure", hedefSure);
                    cmdCagri.Parameters.AddWithValue("@TalepEdenID", talepEdenID);

                    cmdCagri.ExecuteNonQuery();

                    trans.Commit();

                    string olusturanAdSoyad = "";
                    string talepEdenAdSoyad = "";
                    string atananAdSoyad = "";

                    using (SqlCommand cmd = new SqlCommand(@"
                SELECT Ad + ' ' + Soyad FROM Kullanici WHERE KullaniciID = @OlusturanID;
                SELECT TalepEden FROM TalepEdenler WHERE TalepEdenID = @TalepEdenID;
                SELECT Ad + ' ' + Soyad FROM Kullanici WHERE KullaniciID = @AtananID;
            ", conn))
                    {
                        cmd.Parameters.AddWithValue("@OlusturanID", kullaniciID);
                        cmd.Parameters.AddWithValue("@TalepEdenID", talepEdenID);
                        cmd.Parameters.AddWithValue("@AtananID", atananID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                olusturanAdSoyad = reader.GetString(0);

                            if (reader.NextResult() && reader.Read())
                                talepEdenAdSoyad = reader.GetString(0);

                            if (reader.NextResult() && reader.Read())
                                atananAdSoyad = reader.GetString(0);
                        }
                    }

                    string logDetaylari = $"Yeni görev oluşturuldu - Başlık: {baslik}, Kategori: {kategori}, Öncelik: {oncelik}, Atanan: {atananAdSoyad}";
                    _logger.LogEkle("Ekleme", "Cagri", logDetaylari);

                    FormuTemizle();

                    string mesaj = $"{olusturanAdSoyad} tarafından\n" +
                    $"{talepEdenAdSoyad} adlı kişi adına\n" +
                    $"{atananAdSoyad} e yeni görev atanmıştır.";
                    MessageBox.Show(mesaj, "Görev Atama Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                }
            }
        }

        private void CallerTaskCreationPage_Load(object sender, EventArgs e)
        {
            EkipYoneticileriniGetir();
            DepartmanlariGetir();
            StatikVerileriYukle();
            _logger.LogEkle("Görüntüleme", "CallerTaskCreationPage", "Görev oluşturma sayfası açıldı");
        }

        private void btniptal_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("İşlem", "CallerTaskCreationPage", "Görev oluşturma işlemi iptal edildi");
            FormuTemizle();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Yönlendirme", "CallerTaskCreationPage", "Profil sayfasına yönlendirildi");
            CallerProfile callerProfile = new CallerProfile();
            callerProfile.Show();
            this.Close();
        }

        private void btnCagriOlustur_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Yönlendirme", "CallerTaskCreationPage", "Çağrı oluşturma sayfasına yönlendirildi");
            CallerTaskCreationPage assistantTaskCreationPage = new CallerTaskCreationPage();
            assistantTaskCreationPage.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Yönlendirme", "CallerTaskCreationPage", "Raporlar sayfasına yönlendirildi");
            CallerReports assistantReports = new CallerReports();
            assistantReports.Show();
            this.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Çıkış", "CallerTaskCreationPage", "Uygulamadan çıkış yapıldı");
            UserInformation.BilgileriTemizle();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}
