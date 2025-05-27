using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class EditUsers : Form
    {
        private int secilenKullaniciID;
        public EditUsers()
        {
            InitializeComponent();
            secilenKullaniciID = 0;
        }

        public EditUsers(int kullaniciID, string ad, string soyad, string email, string sifre, string rol,
                decimal maas, decimal prim, string departman, string telefon,
                string cinsiyet, DateTime? dogumTar, DateTime? iseBaslamaTar)
        {
            InitializeComponent();
            secilenKullaniciID = kullaniciID;

            BilgileriDoldur(kullaniciID, ad, soyad, email, sifre, rol, maas, prim,
                           departman, telefon, cinsiyet, dogumTar, iseBaslamaTar);
        }

        private void BilgileriDoldur(int kullaniciID, string ad, string soyad, string email, string sifre, string rol,
                              decimal maas, decimal prim, string departman, string telefon,
                              string cinsiyet, DateTime? dogumTar, DateTime? iseBaslamaTar)
        {
            try
            {
                lblKullaniciID.Text = kullaniciID.ToString();
                txtAd.Text = ad;
                txtSoyad.Text = soyad;
                txtEmail.Text = email;
                txtSifre.Text = sifre;
                txtMaas.Text = maas.ToString("N2");
                txtPrim.Text = prim.ToString("N2");
                txtTelefon.Text = telefon;

                // Rol eşleştir
                if (!string.IsNullOrWhiteSpace(rol) && cmbRol.Items.Contains(rol.Trim()))
                    cmbRol.SelectedItem = rol.Trim();
                else
                    cmbRol.SelectedIndex = -1;

                // Departman eşleştir
                if (!string.IsNullOrWhiteSpace(departman) && cmbDepartman.Items.Contains(departman.Trim()))
                    cmbDepartman.SelectedItem = departman.Trim();
                else
                    cmbDepartman.SelectedIndex = -1;

                // Cinsiyet eşleştir
                if (!string.IsNullOrWhiteSpace(cinsiyet) && cmbCinsiyet.Items.Contains(cinsiyet.Trim()))
                    cmbCinsiyet.SelectedItem = cinsiyet.Trim();
                else
                    cmbCinsiyet.SelectedIndex = -1;

                if (dogumTar.HasValue)
                    dtpDogumTarihi.Value = dogumTar.Value;

                if (iseBaslamaTar.HasValue)
                    dtpIseBaslamaTarihi.Value = iseBaslamaTar.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bilgiler doldurulurken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            try
            {
                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    string updateQuery = @"
                UPDATE Kullanici SET
                    Ad = @ad,
                    Soyad = @soyad,
                    Email = @email,
                    Sifre = @sifre,
                    Rol = @rol,
                    Maas = @maas,
                    Prim = @prim,
                    DepartmanID = @departmanID,
                    Telefon = @telefon,
                    Cinsiyet = @cinsiyet,
                    DogumTar = @dogumTar,
                    IseBaslamaTar = @iseBaslamaTar
                WHERE KullaniciID = @kullaniciID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ad", txtAd.Text.Trim());
                        cmd.Parameters.AddWithValue("@soyad", txtSoyad.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@sifre", txtSifre.Text.Trim());
                        cmd.Parameters.AddWithValue("@rol", cmbRol.Text);
                        cmd.Parameters.AddWithValue("@maas", decimal.TryParse(txtMaas.Text, out var maasVal) ? maasVal : 0);
                        cmd.Parameters.AddWithValue("@prim", decimal.TryParse(txtPrim.Text, out var primVal) ? primVal : 0);
                        cmd.Parameters.AddWithValue("@departmanID", departmanID);
                        cmd.Parameters.AddWithValue("@telefon", txtTelefon.Text.Trim());
                        cmd.Parameters.AddWithValue("@cinsiyet", cmbCinsiyet.Text);
                        cmd.Parameters.AddWithValue("@dogumTar", dtpDogumTarihi.Value.Date);
                        cmd.Parameters.AddWithValue("@iseBaslamaTar", dtpIseBaslamaTarihi.Value.Date);
                        cmd.Parameters.AddWithValue("@kullaniciID", Convert.ToInt32(lblKullaniciID.Text));

                        int etkilenen = cmd.ExecuteNonQuery();

                        if (etkilenen > 0)
                        {
                            MessageBox.Show("Kullanıcı bilgileri başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            BossUsersControl bossUsersControl = new BossUsersControl();
                            bossUsersControl.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme yapılmadı! Kullanıcı bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Güncelleme sırasında hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DepartmanlariYukle()
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
                cmbDepartman.SelectedIndex = -1;                
            }
        }


        private void EditUsers_Load(object sender, EventArgs e)
        {
            DepartmanlariYukle();

            // Roller
            cmbRol.Items.Clear();
            cmbRol.Items.AddRange(new string[] { "Çağrı Merkezi", "Ekip Yöneticisi", "Müdür", "Ekip Üyesi" });

            // Cinsiyet
            cmbCinsiyet.Items.Clear();
            cmbCinsiyet.Items.AddRange(new string[] { "Kadın", "Erkek" });
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
    }
}
