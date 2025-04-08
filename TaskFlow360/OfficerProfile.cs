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
    public partial class OfficerProfile : Form
    {
        Baglanti baglanti = new Baglanti();
        public OfficerProfile()
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

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            OfficerHomepage officerHomepage = new OfficerHomepage();
            officerHomepage.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
            this.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            OfficerProfile profile = new OfficerProfile();
            profile.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            OfficerReportsPage officerReportsPage = new OfficerReportsPage();
            officerReportsPage.Show();
            this.Close();
        }

        private void OfficerProfile_Load(object sender, EventArgs e)
        {
            string kullaniciID = KullaniciBilgi.KullaniciID;

            try
            {
                baglanti.BaglantiAc();

                string query = @"SELECT 
                K.Ad, K.Soyad, K.Email, K.Telefon, K.Adres, 
                K.DogumTar, K.IseBaslamaTar, 
                D.DepartmanAdi, B.BolumAdi,
                YK.Ad AS YoneticiAd, YK.Soyad AS YoneticiSoyad,
                K.Cinsiyet
                FROM Kullanici K
                LEFT JOIN Departman D ON K.DepartmanID = D.DepartmanID
                LEFT JOIN Bolum B ON K.BolumID = B.BolumID
                LEFT JOIN Kullanici YK ON K.YoneticiID = YK.KullaniciID
                WHERE K.KullaniciID = @KullaniciID";

                SqlCommand cmd = new SqlCommand(query, baglanti.conn);
                cmd.Parameters.AddWithValue("@KullaniciID", kullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblAdSoyad.Text = $"{dr["Ad"]} {dr["Soyad"]}";
                    lblEmail.Text = dr["Email"].ToString();
                    lblTelefon.Text = dr["Telefon"].ToString();
                    lblAdres.Text = dr["Adres"].ToString();
                    lblDogumTarihi.Text = Convert.ToDateTime(dr["DogumTar"]).ToShortDateString();
                    lblIseBaslama.Text = Convert.ToDateTime(dr["IseBaslamaTar"]).ToShortDateString();
                    lblDepartman.Text = dr["DepartmanAdi"].ToString();
                    lblBolum.Text = dr["BolumAdi"].ToString() + " " + "(Rol)";

                    string yoneticiAdSoyad = dr["YoneticiAd"] != DBNull.Value ? $"{dr["YoneticiAd"]} {dr["YoneticiSoyad"]}" : "Yönetici yok";
                    lblYonetici.Text = yoneticiAdSoyad;

                    string cinsiyet = dr["Cinsiyet"].ToString();

                    if (cinsiyet == "Kadın")
                        pctrProfil.Image = Properties.Resources.kadin;
                    else if (cinsiyet == "Erkek")
                        pctrProfil.Image = Properties.Resources.erkek;

                }

                dr.Close();
                lblKullaniciID.Text = kullaniciID;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı bilgileri yüklenirken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }


        private void lbltlfn_Click(object sender, EventArgs e)
        {

        }
    }
}
