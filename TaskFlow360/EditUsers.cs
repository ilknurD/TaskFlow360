using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        public EditUsers(int kullaniciID, string ad, string soyad, string email, string rol,
                    decimal maas, decimal prim, string departman, string telefon,
                    string cinsiyet, DateTime? dogumTar, DateTime? iseBaslamaTar)
        {
            InitializeComponent();
            secilenKullaniciID = kullaniciID;

            // Bilgileri label'lara veya textbox'lara aktar
            BilgileriDoldur(kullaniciID, ad, soyad, email, rol, maas, prim,
                           departman, telefon, cinsiyet, dogumTar, iseBaslamaTar);
        }
        private void BilgileriDoldur(int kullaniciID, string ad, string soyad, string email, string rol,
                                decimal maas, decimal prim, string departman, string telefon,
                                string cinsiyet, DateTime? dogumTar, DateTime? iseBaslamaTar)
        {
            try
            {
                // Label'lara bilgileri aktar (label adlarınızı kendi form'unuza göre değiştirin)
                lblKullaniciID.Text = kullaniciID.ToString();
                txtAd.Text = ad;
                txtSoyad.Text = soyad;
                txtEmail.Text = email;
                cmbRol.Text = rol;
                txtMaas.Text = maas.ToString("N2");
                txtPrim.Text = prim.ToString("N2");
                cmbDepartman.Text = departman != "Departman Yok" ? departman : "";
                txtTelefon.Text = telefon;
                cmbCinsiyet.Text = cinsiyet;

                if (dogumTar.HasValue)
                    dtpDogumTarihi.Value = dogumTar.Value;

                if (iseBaslamaTar.HasValue)
                    dtpIseBaslamaTarihi.Value = iseBaslamaTar.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bilgiler doldurulurken hata oluştu: {ex.Message}",
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
