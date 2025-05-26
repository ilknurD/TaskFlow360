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
    public partial class AssistantHomepage : Form
    {
        Baglanti baglanti = new Baglanti();
        private string kullaniciID;
        public AssistantHomepage()
        {
            InitializeComponent();
            kullaniciID = KullaniciBilgi.KullaniciID;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void AssistantHomepage_Load(object sender, EventArgs e)
        {
            // Eğer kullaniciID boşsa, KullaniciBilgi'den tekrar almayı deneyelim
            if (string.IsNullOrEmpty(kullaniciID))
            {
                kullaniciID = KullaniciBilgi.KullaniciID;

                // Hala boşsa, kullanıcıyı login formuna yönlendirelim
                if (string.IsNullOrEmpty(kullaniciID))
                {
                    MessageBox.Show("Oturum bilgileriniz geçersiz. Lütfen tekrar giriş yapın.", "Oturum Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                    return;
                }
            }

            lblisim.Text = KullaniciBilgi.TamAd();
            lblHosgeldiniz.Text = $"Hoş Geldiniz, {KullaniciBilgi.TamAd()}";

            try
            {
                TarihVeSaatGoster();
                IstatislikleriGoster();
                SonCagrilariGetir();
                BugunkuCagrilariGoster();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TarihVeSaatGoster()
        {
            DateTime suAn = DateTime.Now;
            string formatliTarihSaat = suAn.ToString("dd MMMM yyyy, dddd HH:mm");
            lblTarihSaat.Text = formatliTarihSaat;
        }

        private void BugunkuCagrilariGoster()
        {
            try
            {
                baglanti.BaglantiAc();

                string query = @"
            SELECT 
                SUM(CASE WHEN CAST(c.OlusturmaTarihi AS DATE) = CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS BugunkuYeniCagri,
                SUM(CASE WHEN Durum = 'Atandı' AND CAST(c.OlusturmaTarihi AS DATE) = CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS BugunkuAtananCagri
            FROM Cagri c";

                using (SqlCommand command = new SqlCommand(query, baglanti.conn))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int bugunkuYeni = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        int bugunkuAtanan = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);

                        lblBugunkuDurum.Text = $"Bugün {bugunkuYeni} yeni çağrı ve {bugunkuAtanan} atanan çağrı bulunmaktadır.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bugünkü çağrılar alınırken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void IstatislikleriGoster()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.AutoScroll = true;

            Size panelBoyut = new Size(200, 100);
            Padding panelMargin = new Padding(15, 0, 15, 0);

            void PanelEkle(Color arkaPlan, string sayi, string metin)
            {
                Panel panel = new Panel
                {
                    Size = panelBoyut,
                    BackColor = arkaPlan,
                    Margin = panelMargin
                };

                Label lblSayi = new Label
                {
                    Text = sayi,
                    Font = new Font("Arial", 18, FontStyle.Bold),
                    AutoSize = false,
                    Size = new Size(panel.Width, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Label lblMetin = new Label
                {
                    Text = metin,
                    Font = new Font("Arial", 10),
                    AutoSize = false,
                    Size = new Size(panel.Width, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                lblSayi.Location = new Point(0, 20);
                lblMetin.Location = new Point(0, 60);

                panel.Controls.Add(lblSayi);
                panel.Controls.Add(lblMetin);
                flowLayoutPanel1.Controls.Add(panel);
            }

            try
            {
                baglanti.BaglantiAc();

                string query = @"
                SELECT 
                    SUM(CASE WHEN Durum = 'Tamamlandı' THEN 1 ELSE 0 END) AS Tamamlanan,
                    SUM(CASE WHEN Durum = 'Beklemede' THEN 1 ELSE 0 END) AS DevamEden,
                    SUM(CASE WHEN Durum = 'Gecikti' THEN 1 ELSE 0 END) AS Geciken,
                    SUM(CASE WHEN Durum = 'Atandı' THEN 1 ELSE 0 END) AS Atanan,
                    COUNT(*) AS Toplam
                FROM Cagri";

                using (SqlCommand command = new SqlCommand(query, baglanti.conn))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int tamamlanan = reader.GetInt32(0);
                        int devamEden = reader.GetInt32(1);
                        int geciken = reader.GetInt32(2);
                        int atanan = reader.GetInt32(3);
                        int toplam = reader.GetInt32(4);

                        PanelEkle(Color.FromArgb(103, 230, 179), tamamlanan.ToString(), "Tamamlanan");
                        PanelEkle(Color.FromArgb(255, 204, 128), devamEden.ToString(), "Devam Eden");
                        PanelEkle(Color.FromArgb(239, 83, 80), geciken.ToString(), "Geciken");
                        PanelEkle(Color.FromArgb(179, 136, 255), atanan.ToString(), "Atanan");
                        PanelEkle(Color.FromArgb(126, 87, 194), toplam.ToString(), "Toplam");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }



        private void SonCagrilariGetir()
        {
            try
            {
                baglanti.BaglantiAc();

                string query = @"
        SELECT TOP 14
            '#' + CAST(c.CagriID AS VARCHAR) AS [Çağrı No],
            c.Baslik AS [Başlık],
            c.Durum AS [Durum],
            FORMAT(c.OlusturmaTarihi, 'dd.MM.yyyy HH:mm') AS [Oluşturulma Tarihi],
            CASE 
                WHEN c.AtananKullaniciID IS NULL THEN 'Atanmadı'
                ELSE k.Ad + ' ' + k.Soyad 
            END AS [Atanan Personel],
            ISNULL(k.Rol, 'Belirtilmedi') AS [Rol]
        FROM Cagri c
        LEFT JOIN Kullanici k ON c.AtananKullaniciID = k.KullaniciID
        ORDER BY c.OlusturmaTarihi DESC";

                using (SqlCommand command = new SqlCommand(query, baglanti.conn))
                {
                    DataTable dataTable = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);

                    SonAcilanCagrilarDGV.DataSource = dataTable;

                    foreach (DataGridViewColumn column in SonAcilanCagrilarDGV.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }

                    // Tema ayarları
                    SonAcilanCagrilarDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    SonAcilanCagrilarDGV.EnableHeadersVisualStyles = false;

                    // Başlık tasarımı
                    SonAcilanCagrilarDGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(103, 58, 183); // Koyu mor
                    SonAcilanCagrilarDGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    SonAcilanCagrilarDGV.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Bold);
                    SonAcilanCagrilarDGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // Satır ayarları
                    SonAcilanCagrilarDGV.DefaultCellStyle.BackColor = Color.White;
                    SonAcilanCagrilarDGV.DefaultCellStyle.ForeColor = Color.Black;
                    SonAcilanCagrilarDGV.DefaultCellStyle.Font = new Font("Century Gothic", 10);
                    SonAcilanCagrilarDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(232, 222, 248); // Açık mor
                    SonAcilanCagrilarDGV.GridColor = Color.Gainsboro;

                    // Seçili satır tasarımı
                    SonAcilanCagrilarDGV.DefaultCellStyle.SelectionBackColor = Color.FromArgb(126, 87, 194); // Ana mor renk
                    SonAcilanCagrilarDGV.DefaultCellStyle.SelectionForeColor = Color.White;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }





        private void btnCagriOlustur_Click(object sender, EventArgs e)
        {
            AssistantTaskCreationPage assistantTaskCreationPage = new AssistantTaskCreationPage();
            assistantTaskCreationPage.Show();
            //this.Close();
        }
    }
}
