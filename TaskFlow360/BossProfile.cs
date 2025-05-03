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
    public partial class BossProfile : Form
    {
        Baglanti baglanti = new Baglanti();
        private string yoneticiId;
        public BossProfile()
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

        private void BossProfile_Load(object sender, EventArgs e)
        {
            yoneticiId = KullaniciBilgi.KullaniciID;
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
                cmd.Parameters.AddWithValue("@KullaniciID", yoneticiId);

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
                    lblBolum.Text = dr["BolumAdi"].ToString() + " " + "(Bölüm)";

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
                lblKullaniciID.Text = yoneticiId.ToString();
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

        private void btnProfil_Click(object sender, EventArgs e)
        {
            BossProfile bossProfile = new BossProfile();
            bossProfile.Show();
            this.Close();
        }

        private void btnKullaniciIslem_Click(object sender, EventArgs e)
        {
            BossUsersControl bossUsersControl = new BossUsersControl();
            bossUsersControl.Show();
            this.Close();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            BossHomepage bossHomepage = new BossHomepage();
            bossHomepage.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            BossReports bossReports = new BossReports();
            bossReports.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UsersSalary usersSalary = new UsersSalary();
            usersSalary.Show();
            this.Close();
        }
    }
}
