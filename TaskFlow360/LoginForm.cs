using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace TaskFlow360
{
    public partial class LoginForm : Form
    {
        Connection baglanti = new Connection();
        public LoginForm()
        {
            InitializeComponent();
            this.Load += LoginForm_Load;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtMail.Text = "Mail";
            txtPassword.Text = "Şifre";

            if (Properties.Settings.Default.BeniHatirla)
            {
                txtMail.Text = Properties.Settings.Default.Email;
                txtPassword.Text = Properties.Settings.Default.Sifre;
                txtPassword.UseSystemPasswordChar = true;
                chkBeniHatirla.Checked = true;
            }
            else
            {
                txtMail.Text = "";
                txtPassword.Text = "";
                txtPassword.UseSystemPasswordChar = false;
                chkBeniHatirla.Checked = false;
            }

            txtMail.Enter += TextBox_Enter;
            txtMail.Leave += TextBox_Leave;
            txtPassword.Enter += TextBox_Enter;
            txtPassword.Leave += TextBox_Leave;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text == "Mail" || textBox.Text == "Şifre")
                {
                    textBox.Text = "";
                    if (textBox == txtPassword)
                    {
                        textBox.UseSystemPasswordChar = true;
                    }
                }
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (textBox == txtMail)
                    {
                        textBox.Text = "Mail";
                    }
                    else if (textBox == txtPassword)
                    {
                        textBox.Text = "Şifre";
                        textBox.UseSystemPasswordChar = false;
                    }
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

        private void button1_Click(object sender, EventArgs e)
        {
            string email = txtMail.Text.Trim();
            string sifre = txtPassword.Text.Trim();

            if (email == "Mail" || sifre == "Şifre" || email == "" || sifre == "")
            {
                MessageBox.Show("Lütfen e-posta ve şifrenizi girin.");
                return;
            }

            try
            {
                baglanti.BaglantiAc();

                string query = "SELECT KullaniciID, Rol, Ad, Soyad FROM Kullanici WHERE Email = @Email AND Sifre = @Sifre";
                SqlCommand cmd = new SqlCommand(query, baglanti.conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Sifre", sifre);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Giriş başarılıysa kullanıcı bilgilerini ayarla
                        UserInformation.KullaniciID = reader["KullaniciID"]?.ToString() ?? "";
                        UserInformation.Rol = reader["Rol"]?.ToString() ?? "";
                        UserInformation.Ad = reader["Ad"]?.ToString() ?? "";
                        UserInformation.Soyad = reader["Soyad"]?.ToString() ?? "";

                        // ✅ Beni Hatırla ayarlarını kaydet
                        if (chkBeniHatirla.Checked)
                        {
                            Properties.Settings.Default.BeniHatirla = true;
                            Properties.Settings.Default.Email = email;
                            Properties.Settings.Default.Sifre = sifre;
                        }
                        else
                        {
                            Properties.Settings.Default.BeniHatirla = false;
                            Properties.Settings.Default.Email = "";
                            Properties.Settings.Default.Sifre = "";
                        }
                        Properties.Settings.Default.Save();

                        // Rol'e göre formu aç
                        Form homepage = null;
                        switch (UserInformation.Rol)
                        {
                            case "1":
                            case "Ekip Üyesi":
                                homepage = new OfficerHomepage();
                                break;
                            case "2":
                            case "Ekip Yöneticisi":
                                homepage = new ManagerHomepage();
                                break;
                            case "3":
                            case "Çağrı Merkezi":
                                homepage = new CallerHomepage();
                                break;
                            case "4":
                            case "Müdür":
                                homepage = new BossHomepage();
                                break;
                            default:
                                MessageBox.Show("Tanımlanamayan kullanıcı rolü: " + UserInformation.Rol);
                                return;
                        }

                        homepage.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Geçersiz e-posta veya şifre.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void SifremiUnuttum_Click(object sender, EventArgs e)
        {
            string email = txtMail.Text.Trim();
            
            if (email == "Mail" || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Lütfen e-posta adresinizi girin.");
                return;
            }

            try
            {
                baglanti.BaglantiAc();

                string query = "SELECT Sifre FROM Kullanici WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, baglanti.conn);
                cmd.Parameters.AddWithValue("@Email", email);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string sifre = result.ToString();

                    // Gmail SMTP ayarları
                    string smtpEmail = "ilknurmduman60@gmail.com";
                    string smtpPassword = "blav kjlp ppjw baty";
                    string smtpHost = "smtp.gmail.com";
                    int smtpPort = 587;
                    // Gmail için SSL portu

                    SmtpClient smtp = new SmtpClient(smtpHost, smtpPort);
                    smtp.EnableSsl = true;
                    
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(smtpEmail, smtpPassword);

                    // Kullanıcıya mail gönder
                    MailMessage userMail = new MailMessage();
                    userMail.From = new MailAddress(smtpEmail);
                    userMail.To.Add(email);
                    userMail.Subject = "TaskFlow360 - Şifre Bilgilendirmesi";
                    userMail.Body = $"Merhaba,\n\nŞifreniz: {sifre}\n\nGüvenliğiniz için lütfen şifrenizi kimseyle paylaşmayınız.\n\nSaygılarımızla,\nTaskFlow360 Ekibi";
                    smtp.Send(userMail);

                    // Müdüre bilgilendirme maili gönder
                    MailMessage mudurMail = new MailMessage();
                    mudurMail.From = new MailAddress(smtpEmail);
                    mudurMail.To.Add("ilknurmduman60@gmail.com");
                    mudurMail.Subject = "TaskFlow360 - Şifre Sıfırlama Bildirimi";
                    mudurMail.Body = $"Sayın Müdürümüz,\n\n{email} adresli kullanıcı şifre sıfırlama talebinde bulunmuştur.\n\nSaygılarımızla,\nTaskFlow360 Sistemi";
                    smtp.Send(mudurMail);

                    MessageBox.Show("Şifreniz e-posta adresinize gönderilmiştir.");
                }
                else
                {
                    MessageBox.Show("Bu e-posta adresi sistemde kayıtlı değil.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }
    }
}