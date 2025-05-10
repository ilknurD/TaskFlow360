using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class ManagerProfile : Form
    {
        Baglanti baglanti = new Baglanti();
        public ManagerProfile()
        {
            InitializeComponent();
            ekibimDGV.DataError += ekibimDGV_DataError;
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            ManagerProfile manageProfile = new ManagerProfile();
            manageProfile.Show();
            this.Close();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            ManagerTasks managerTasks = new ManagerTasks();
            managerTasks.Show();
            this.Close();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void btnEkipYonetimi_Click(object sender, EventArgs e)
        {
            // Ekip yönetimi butonu işlevi
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void ManagerProfile_Load(object sender, EventArgs e)
        {
            ConfigureEkibimDGV();
            LoadManagedTeamMembers(); // Yönetilen ekip üyelerini yükle
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

                if (dr.HasRows && dr.Read())
                {
                    lblAdSoyad.Text = $"{dr["Ad"]} {dr["Soyad"]}";
                    lblEmail.Text = dr["Email"].ToString();
                    lblTelefon.Text = dr["Telefon"].ToString();
                    lblAdres.Text = dr["Adres"].ToString();
                    lblDogumTarihi.Text = Convert.ToDateTime(dr["DogumTar"]).ToShortDateString();
                    lblIseBaslama.Text = Convert.ToDateTime(dr["IseBaslamaTar"]).ToShortDateString();
                    lblDepartmanb.Text = dr["DepartmanAdi"].ToString();
                    lblBolum.Text = dr["BolumAdi"].ToString() + " " + "(Rol)";

                    string yoneticiAdSoyad = dr["YoneticiAd"] != DBNull.Value ? $"{dr["YoneticiAd"]} {dr["YoneticiSoyad"]}" : "Yönetici yok";
                    lblYoneticib.Text = yoneticiAdSoyad;

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

        private void LoadManagedTeamMembers()
        {
            string managerID = KullaniciBilgi.KullaniciID;

            try
            {
                baglanti.BaglantiAc();

                string query = @"SELECT 
                K.Ad + ' ' + K.Soyad AS [Personel Adı],
                COUNT(C.CagriID) AS [Aktif Görev Sayısı],
                ISNULL(AVG(DATEDIFF(HOUR, C.OlusturmaTarihi, C.TeslimTarihi)), 0) AS [Ortalama Çözüm Süresi (Saat)]
                FROM Kullanici K
                LEFT JOIN Cagri C ON K.KullaniciID = C.OlusturanKullaniciID
                WHERE K.YoneticiID = @YoneticiID
                GROUP BY K.Ad, K.Soyad
                ORDER BY K.Ad, K.Soyad";

                SqlCommand cmd = new SqlCommand(query, baglanti.conn);
                cmd.Parameters.AddWithValue("@YoneticiID", managerID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ekibimDGV.DataSource = dt;

                FormatEkibimDGV(); // Stil ve format ayarlarını ayrı bir metodda yaptık
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekip üyeleri yüklenirken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void FormatEkibimDGV()
        {
            if (ekibimDGV.Columns.Count >= 4)
            {
                ekibimDGV.Columns["Ortalama Çözüm Süresi (Saat)"].DefaultCellStyle.Format = "N1";

                // Hizalama ayarları
                ekibimDGV.Columns["Personel Adı"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                ekibimDGV.Columns["Aktif Görev Sayısı"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                ekibimDGV.Columns["Ortalama Çözüm Süresi (Saat)"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void ekibimDGV_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
             MessageBox.Show("Veri işleme sırasında bir sorun oluştu. Lütfen yöneticiyle iletişime geçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ConfigureEkibimDGV()
        {
            // Temel seçim ayarları
            ekibimDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ekibimDGV.DefaultCellStyle.SelectionBackColor = ekibimDGV.DefaultCellStyle.BackColor;
            ekibimDGV.DefaultCellStyle.SelectionForeColor = ekibimDGV.DefaultCellStyle.ForeColor;

            // Başlık ayarları
            ekibimDGV.EnableHeadersVisualStyles = false;
            ekibimDGV.ColumnHeadersDefaultCellStyle.SelectionBackColor = ekibimDGV.ColumnHeadersDefaultCellStyle.BackColor;
            ekibimDGV.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Transparent;
            ekibimDGV.RowHeadersDefaultCellStyle.ForeColor = Color.Black;

            // Görsel iyileştirmeler
            ekibimDGV.BorderStyle = BorderStyle.None;
            ekibimDGV.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            ekibimDGV.GridColor = Color.FromArgb(240, 240, 240);

            // Yazı tipi ve hizalama
            ekibimDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            ekibimDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ekibimDGV.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            ekibimDGV.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Event bağlantıları
            ekibimDGV.CellClick += (s, e) => ekibimDGV.ClearSelection();
            ekibimDGV.DataBindingComplete += (s, e) => ekibimDGV.ClearSelection();
        }

        private void pnlIletisimBilgi_Paint(object sender, PaintEventArgs e)
        {
            // Panel boyama işlemleri
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            ManagerReportsPage managerReportsPage = new ManagerReportsPage();
            managerReportsPage.Show();
            this.Close();
        }
    }
}