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
        private string yoneticiId;
        public ManagerProfile()
        {
            InitializeComponent();
            ekibimDGV.DataError += ekibimDGV_DataError;
            SetupDataGridViewColumns();
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
            ManagerDashboard managerDashboard = new ManagerDashboard();
            managerDashboard.Show();
            this.Close();
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
            yoneticiId = KullaniciBilgi.KullaniciID;
            ConfigureEkibimDGV();
            LoadManagedTeamMembers();

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
                    lblDepartmanb.Text = dr["DepartmanAdi"].ToString();
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
                lblKullaniciID.Text = yoneticiId.ToString(); // Fix: Convert int to string explicitly  
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

        private void SetupDataGridViewColumns()
        {
            if (ekibimDGV.Columns.Count == 0)
            {
                ekibimDGV.Columns.Add("calisan", "Çalışan");
                ekibimDGV.Columns.Add("aktifGorev", "Aktif Görevler");
                ekibimDGV.Columns.Add("tamamlananGorev", "Tamamlanan Görevler");
                ekibimDGV.Columns.Add("aylikPerformans", "Aylık Performans");
                ekibimDGV.Columns.Add("ortalamaSure", "Ortalama Çözüm Süresi");
                ekibimDGV.Columns.Add("gorevAtaButon", "İşlem");
            }
        }
        private void LoadManagedTeamMembers()
        {
            try
            {
                baglanti.BaglantiAc();

                string kontrolSorgusu =
                    @"SELECT COUNT(*) FROM Kullanici 
            WHERE Rol = 'Ekip Üyesi'
            AND (DepartmanID = (SELECT DepartmanID FROM Kullanici WHERE KullaniciID = @yoneticiId)
            OR YoneticiID = @yoneticiId)";

                SqlCommand kontrolKomut = new SqlCommand(kontrolSorgusu, baglanti.conn);
                kontrolKomut.Parameters.AddWithValue("@yoneticiId", yoneticiId);

                int kullaniciSayisi = Convert.ToInt32(kontrolKomut.ExecuteScalar());

                if (kullaniciSayisi == 0)
                {
                    ekibimDGV.Rows.Clear();

                    if (!icerikPanel.Controls.ContainsKey("lblBosEkip"))
                    {
                        Label lblBosEkip = new Label();
                        lblBosEkip.Name = "lblBosEkip";
                        lblBosEkip.Text = "Departmanınızda ekip üyesi bulunmuyor.";
                        lblBosEkip.AutoSize = false;
                        lblBosEkip.Size = new Size(icerikPanel.Width - 20, 60);
                        lblBosEkip.TextAlign = ContentAlignment.MiddleCenter;
                        lblBosEkip.Font = new Font(lblBosEkip.Font.FontFamily, 10, FontStyle.Regular);
                        lblBosEkip.ForeColor = Color.Gray;
                        lblBosEkip.Location = new Point(10, icerikPanel.Height / 2 - 30);
                        icerikPanel.Controls.Add(lblBosEkip);
                    }

                    ekibimDGV.Visible = false;
                    return;
                }
                else
                {
                    if (icerikPanel.Controls.ContainsKey("lblBosEkip"))
                    {
                        icerikPanel.Controls.RemoveByKey("lblBosEkip");
                    }

                    ekibimDGV.Visible = true;
                }

                string anaSorgu =
                    @"SELECT 
            k.KullaniciID, 
            k.Ad + ' ' + k.Soyad AS AdSoyad, 
            COUNT(CASE WHEN c.Durum = 'Beklemede' OR c.Durum = 'Atandı' THEN 1 END) AS AktifGorevSayisi, 
            COUNT(CASE WHEN c.Durum = 'Tamamlandı' THEN 1 END) AS TamamlananGorevSayisi,
            AVG(CASE 
                WHEN c.HedefSure IS NOT NULL THEN
                    TRY_CONVERT(float, REPLACE(REPLACE(REPLACE(c.HedefSure, ' Saat', ''), ' saat', ''), ',', '.'))
                ELSE NULL
            END) AS OrtalamaSure
          FROM Kullanici k 
          LEFT JOIN Cagri c ON k.KullaniciID = c.AtananKullaniciID 
          WHERE k.Rol = 'Ekip Üyesi'
            AND (k.DepartmanID = (SELECT DepartmanID FROM Kullanici WHERE KullaniciID = @yoneticiId)
            OR k.YoneticiID = @yoneticiId)
          GROUP BY k.KullaniciID, k.Ad, k.Soyad";

                SqlCommand komut = new SqlCommand(anaSorgu, baglanti.conn);
                komut.Parameters.AddWithValue("@yoneticiId", yoneticiId);
                SqlDataReader dr = komut.ExecuteReader();

                ekibimDGV.Rows.Clear();

                while (dr.Read())
                {
                    int rowIndex = ekibimDGV.Rows.Add();
                    DataGridViewRow row = ekibimDGV.Rows[rowIndex];

                    int kullaniciID = Convert.ToInt32(dr["KullaniciID"]);
                    row.Cells["calisan"].Value = dr["AdSoyad"].ToString();
                    row.Cells["aktifGorev"].Value = dr["AktifGorevSayisi"].ToString();
                    row.Cells["tamamlananGorev"].Value = dr["TamamlananGorevSayisi"].ToString();

                    // Hesaplanan performans yüzdesi
                    int tamamlananGorevSayisi = Convert.ToInt32(dr["TamamlananGorevSayisi"]);
                    int aktifGorevSayisi = Convert.ToInt32(dr["AktifGorevSayisi"]);
                    int toplamGorevSayisi = tamamlananGorevSayisi + aktifGorevSayisi;
                    int performansYuzdesi = toplamGorevSayisi > 0 ? (tamamlananGorevSayisi * 100) / toplamGorevSayisi : 0;

                    row.Cells["aylikPerformans"].Value = performansYuzdesi.ToString() + "%";

                    if (!dr.IsDBNull(dr.GetOrdinal("OrtalamaSure")))
                    {
                        double sure = Convert.ToDouble(dr["OrtalamaSure"]);
                        row.Cells["ortalamaSure"].Value = $"{Math.Round(sure, 1)} saat";
                    }
                    else
                    {
                        row.Cells["ortalamaSure"].Value = "Veri yok";
                    }

                    row.Tag = kullaniciID;
                }
                dr.Close();

                if (ekibimDGV.Rows.Count == 0 && kullaniciSayisi > 0)
                {
                    MessageBox.Show("Ekip üyeleri grid'e yüklenemedi.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekip üyeleri yüklenirken hata oluştu: " + ex.Message, "Veri Yükleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
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
            ekibimDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            ekibimDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ekibimDGV.DefaultCellStyle.Font = new Font("Century Gothic", 12);
            ekibimDGV.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Event bağlantıları
            ekibimDGV.CellClick += (s, e) => ekibimDGV.ClearSelection();
            ekibimDGV.DataBindingComplete += (s, e) => ekibimDGV.ClearSelection();
        }

        private void pnlIletisimBilgi_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            ManagerReportsPage managerReportsPage = new ManagerReportsPage();
            managerReportsPage.Show();
            this.Close();
        }

        private void ekibimDGV_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Regular);

            // Performans yüzdesini renklendirme
            if (e.ColumnIndex == ekibimDGV.Columns["aylikPerformans"].Index && e.Value != null && e.RowIndex >= 0)
            {
                string performansStr = e.Value.ToString();
                if (!string.IsNullOrEmpty(performansStr))
                {
                    int performans = int.Parse(performansStr.Replace("%", ""));

                    if (performans >= 80)
                        e.CellStyle.ForeColor = System.Drawing.Color.Green;
                    else if (performans >= 50)
                        e.CellStyle.ForeColor = System.Drawing.Color.Orange;
                    else
                        e.CellStyle.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}