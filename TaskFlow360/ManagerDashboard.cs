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
    public partial class ManagerDashboard : Form
    {
        Baglanti baglanti = new Baglanti();
        public ManagerDashboard()
        {
            InitializeComponent();
        }

        private void ManagerDashboard_Load(object sender, EventArgs e)
        {
            EkibindekiKullanicilariGetir();
            SonCagrilariGetir();
        }
        private void EkibindekiKullanicilariGetir()
        {
            // DataGrid ayarları - boyutun korunması ve kaydırma çubuklarının eklenmesi  
            EkipDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            EkipDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            EkipDGV.ScrollBars = ScrollBars.Both;

            SqlConnection connection = null;

            try
            {
                connection = Baglanti.BaglantiGetir();
                baglanti.BaglantiAc();

                string query = @"
            SELECT   
                k.KullaniciID,   
                k.Ad,   
                k.Soyad,   
                k.Email,   
                k.Telefon,   
                b.BolumAdi,  
                COUNT(c.CagriID) AS AktifCagriSayisi  
            FROM Kullanici k  
            LEFT JOIN Bolum b ON k.BolumID = b.BolumID  
            LEFT JOIN Cagri c ON k.KullaniciID = c.AtananKullaniciID AND c.Durum = 'Beklemede'  
            WHERE k.YoneticiID = @YoneticiID  
            GROUP BY k.KullaniciID, k.Ad, k.Soyad, k.Email, k.Telefon, b.BolumAdi  
            ORDER BY k.Ad, k.Soyad";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@YoneticiID", KullaniciBilgi.KullaniciID);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    EkipDGV.DataSource = dataTable;

                    if (EkipDGV.Columns.Count > 0)
                    {
                        EkipDGV.Columns["KullaniciID"].HeaderText = "ID";
                        EkipDGV.Columns["Ad"].HeaderText = "Adı";
                        EkipDGV.Columns["Soyad"].HeaderText = "Soyadı";
                        EkipDGV.Columns["Email"].HeaderText = "E-posta";
                        EkipDGV.Columns["Telefon"].HeaderText = "Telefon";
                        EkipDGV.Columns["BolumAdi"].HeaderText = "Bölüm";
                        EkipDGV.Columns["AktifCagriSayisi"].HeaderText = "Aktif Çağrı Sayısı";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekip üyeleri getirilirken bir hata oluştu: " + ex.Message, "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat(); 
            }
        }

        private void SonCagrilariGetir()
        {
            // DataGrid ayarları
            SonGorevlerDGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SonGorevlerDGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            SonGorevlerDGV.ScrollBars = ScrollBars.Both;

            SqlConnection connection = null;

            try
            {
                connection = Baglanti.BaglantiGetir();
                baglanti.BaglantiAc();

                string query = @"
            SELECT TOP 10
                c.CagriID,
                c.Baslik,
                c.Durum,
                c.OlusturmaTarihi,
                k.Ad + ' ' + k.Soyad AS AtananKullanici,
                b.BolumAdi
            FROM Cagri c
            LEFT JOIN Kullanici k ON c.AtananKullaniciID = k.KullaniciID
            LEFT JOIN Bolum b ON k.BolumID = b.BolumID
            WHERE k.YoneticiID = @YoneticiID
            ORDER BY c.OlusturmaTarihi DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@YoneticiID", KullaniciBilgi.KullaniciID);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    SonGorevlerDGV.DataSource = dataTable;

                    if (SonGorevlerDGV.Columns.Count > 0)
                    {
                        SonGorevlerDGV.Columns["CagriID"].HeaderText = "Çağrı ID";
                        SonGorevlerDGV.Columns["Baslik"].HeaderText = "Başlık";
                        SonGorevlerDGV.Columns["Durum"].HeaderText = "Durum";
                        SonGorevlerDGV.Columns["OlusturmaTarihi"].HeaderText = "Oluşturulma Tarihi";
                        SonGorevlerDGV.Columns["AtananKullanici"].HeaderText = "Atanan Kişi";
                        SonGorevlerDGV.Columns["BolumAdi"].HeaderText = "Bölüm";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Son çağrılar getirilirken bir hata oluştu: " + ex.Message, "Hata",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            ManagerProfile managerProfile = new ManagerProfile();
            managerProfile.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            ManagerTasks managerTasks = new ManagerTasks();
            managerTasks.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            ManagerReportsPage managerReportsPage = new ManagerReportsPage();
            managerReportsPage.Show();
            this.Close();
        }

        private void btnEkipYonetimi_Click(object sender, EventArgs e)
        {
            ManagerDashboard managerDashboard = new ManagerDashboard();
            managerDashboard.Show();
            this.Close();
        }

    }
}
