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
    public partial class CallerProfile : Form
    {
        private string kullaniciId;
        Connection baglanti = new Connection();
        public CallerProfile()
        {
            InitializeComponent();
        }
        private void AsistantProfile_Load(object sender, EventArgs e)
        {
            kullaniciId = UserInformation.KullaniciID;
            EkipYoneticileriniListele();

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
                cmd.Parameters.AddWithValue("@KullaniciID", kullaniciId);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows && dr.Read())
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
                else
                {
                    MessageBox.Show("Kullanıcı bilgileri bulunamadı.");
                }

                dr.Close();
                lblKullaniciID.Text = kullaniciId.ToString();
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
        private void EkipYoneticileriniListele()
        {
            yoneticilerDGV.AutoGenerateColumns = false;
            try
            {
                baglanti.BaglantiAc();

                string query = @"SELECT 
            K.Ad + ' ' + K.Soyad AS [Ad Soyad],
            D.DepartmanAdi AS Departman,
            COUNT(CASE WHEN C.Durum IN ('Beklemede', 'Atandı', 'Gecikti') THEN 1 END) AS [Aktif Görevler],
            COUNT(CASE WHEN C.Durum = 'Tamamlandı' THEN 1 END) AS [Tamamlanan Görevler],
            COUNT(DISTINCT K2.KullaniciID) AS [Ekip Üye Sayısı]
            FROM Kullanici K
            LEFT JOIN Departman D ON K.DepartmanID = D.DepartmanID
            LEFT JOIN Cagri C ON K.KullaniciID = C.AtananKullaniciID
            LEFT JOIN Kullanici K2 ON K.KullaniciID = K2.YoneticiID
            WHERE K.Rol = 'Ekip Yöneticisi'
            GROUP BY K.KullaniciID, K.Ad, K.Soyad, D.DepartmanAdi
            ORDER BY K.Ad, K.Soyad";

                SqlDataAdapter da = new SqlDataAdapter(query, baglanti.conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                yoneticilerDGV.DataSource = dt;
                foreach (DataGridViewColumn column in yoneticilerDGV.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                yoneticilerDGV.Columns["adSoyad"].DataPropertyName = "Ad Soyad";
                yoneticilerDGV.Columns["departman"].DataPropertyName = "Departman";
                yoneticilerDGV.Columns["aktifGorev"].DataPropertyName = "Aktif Görevler";
                yoneticilerDGV.Columns["tamamlananGorev"].DataPropertyName = "Tamamlanan Görevler";
                yoneticilerDGV.Columns["ekipUyeSayi"].DataPropertyName = "Ekip Üye Sayısı";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken hata: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
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

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            CallerHomepage anasayfa = new CallerHomepage();
            anasayfa.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            CallerProfile asistantProfile = new CallerProfile();
            asistantProfile.Show();
            this.Close();
        }

        private void btnCagriOlustur_Click(object sender, EventArgs e)
        {
            CallerTaskCreationPage assistantTaskCreationPage = new CallerTaskCreationPage();
            assistantTaskCreationPage.Show();
            this.Close();
        }

        private void btnCagriTakip_Click(object sender, EventArgs e)
        {
            CallerTasks assistantTasks = new CallerTasks();
            assistantTasks.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {

        }

        
    }
}
