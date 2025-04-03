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
    public partial class ManagerTasks : Form
    {
        public ManagerTasks()
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

        private void gorevlerTab_Click(object sender, EventArgs e)
        {

        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerTasks managerTasks = new ManagerTasks();
            managerTasks.Show();
        }

        private void ManagerTasks_Load(object sender, EventArgs e)
        {
            OrnekVerileriYukle();
            ConfigureBekleyenCagrilarDGV();
            ConfigureEkipUyeleriDGV();
        }
        private void ConfigureBekleyenCagrilarDGV()
        {
            // Temel seçim ayarları
            bekleyenCagrilarDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            bekleyenCagrilarDGV.DefaultCellStyle.SelectionBackColor = bekleyenCagrilarDGV.DefaultCellStyle.BackColor;
            bekleyenCagrilarDGV.DefaultCellStyle.SelectionForeColor = bekleyenCagrilarDGV.DefaultCellStyle.ForeColor;

            // Başlık ayarları
            bekleyenCagrilarDGV.EnableHeadersVisualStyles = false;
            bekleyenCagrilarDGV.ColumnHeadersDefaultCellStyle.SelectionBackColor = bekleyenCagrilarDGV.ColumnHeadersDefaultCellStyle.BackColor;
            bekleyenCagrilarDGV.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Transparent;

            // Görsel iyileştirmeler
            bekleyenCagrilarDGV.BorderStyle = BorderStyle.None;
            bekleyenCagrilarDGV.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            bekleyenCagrilarDGV.GridColor = Color.FromArgb(240, 240, 240);

            // Event bağlantıları
            bekleyenCagrilarDGV.CellClick += (s, e) => bekleyenCagrilarDGV.ClearSelection();
            bekleyenCagrilarDGV.DataBindingComplete += (s, e) => bekleyenCagrilarDGV.ClearSelection();
        }

        private void ConfigureEkipUyeleriDGV()
        {
            // Özel renk şeması
            ekipUyeleriDGV.DefaultCellStyle.BackColor = Color.White;
            ekipUyeleriDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            // Seçim efekti kaldırma
            ekipUyeleriDGV.SelectionMode = DataGridViewSelectionMode.CellSelect;
            ekipUyeleriDGV.DefaultCellStyle.SelectionBackColor = ekipUyeleriDGV.DefaultCellStyle.BackColor;
            ekipUyeleriDGV.DefaultCellStyle.SelectionForeColor = ekipUyeleriDGV.DefaultCellStyle.ForeColor;


            //// Çift tıklamayı yönetme
            //ekipUyeleriDGV.CellDoubleClick += (s, e) => {
            //    if (e.RowIndex >= 0)
            //    {
            //        // Örnek: Seçili kullanıcıyı işleme alma
            //        var selectedUser = ekipUyeleriDGV.Rows[e.RowIndex].Cells["KullaniciAdi"].Value.ToString();
            //        MessageBox.Show($"{selectedUser} kullanıcısı seçildi");
            //    }
            //};
        }

        private void OrnekVerileriYukle()
        {
            // Bekleyen çağrılar için örnek veriler
            bekleyenCagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2460", "Mail sistemi bağlantı sorunu", "Altyapı", "Normal", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2461", "Fatura oluşturma problemi", "Destek", "Orta", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2460", "Mail sistemi bağlantı sorunu", "Altyapı", "Normal", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2461", "Fatura oluşturma problemi", "Destek", "Orta", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2460", "Mail sistemi bağlantı sorunu", "Altyapı", "Normal", "Bekleniyor");
            bekleyenCagrilarDGV.Rows.Add("#2461", "Fatura oluşturma problemi", "Destek", "Orta", "Bekleniyor");

            // oncelik hücrelerine renk verme
            foreach (DataGridViewRow row in bekleyenCagrilarDGV.Rows)
            {
                string oncelik = row.Cells["oncelik"].Value.ToString();
                if (oncelik == "Yüksek")
                    row.Cells["oncelik"].Style.BackColor = ColorTranslator.FromHtml("#f85c5c");
                else if (oncelik == "Orta")
                    row.Cells["oncelik"].Style.BackColor = ColorTranslator.FromHtml("#f0ad4e");
                else
                    row.Cells["oncelik"].Style.BackColor = ColorTranslator.FromHtml("#63c966");

                row.Cells["oncelik"].Style.ForeColor = Color.White;
                row.Cells["oncelik"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                row.Cells["ataButon"].Style.BackColor = ColorTranslator.FromHtml("#5d4e9d");
                row.Cells["ataButon"].Style.ForeColor = Color.White;
            }

            // Ekip üyeleri için örnek veriler
            ekipUyeleriDGV.Rows.Add("Ayşe Kaya", "3", "2", "85%", "3.2 saat");
            ekipUyeleriDGV.Rows.Add("Mustafa Şahin", "4", "1", "80%", "3.8 saat");
            ekipUyeleriDGV.Rows.Add("Elif Aksu", "1", "3", "95%", "2.5 saat");
            ekipUyeleriDGV.Rows.Add("Kemal Bulut", "0", "4", "90%", "2.8 saat");
            ekipUyeleriDGV.Rows.Add("Ayşe Kaya", "3", "2", "85%", "3.2 saat");
            ekipUyeleriDGV.Rows.Add("Mustafa Şahin", "4", "1", "80%", "3.8 saat");
            ekipUyeleriDGV.Rows.Add("Elif Aksu", "1", "3", "95%", "2.5 saat");
            ekipUyeleriDGV.Rows.Add("Kemal Bulut", "0", "4", "90%", "2.8 saat");
            ekipUyeleriDGV.Rows.Add("Ayşe Kaya", "3", "2", "85%", "3.2 saat");
            ekipUyeleriDGV.Rows.Add("Mustafa Şahin", "4", "1", "80%", "3.8 saat");
            ekipUyeleriDGV.Rows.Add("Elif Aksu", "1", "3", "95%", "2.5 saat");
            ekipUyeleriDGV.Rows.Add("Kemal Bulut", "0", "4", "90%", "2.8 saat");
            ekipUyeleriDGV.Rows.Add("Ayşe Kaya", "3", "2", "85%", "3.2 saat");
            ekipUyeleriDGV.Rows.Add("Mustafa Şahin", "4", "1", "80%", "3.8 saat");
            ekipUyeleriDGV.Rows.Add("Elif Aksu", "1", "3", "95%", "2.5 saat");
            ekipUyeleriDGV.Rows.Add("Kemal Bulut", "0", "4", "90%", "2.8 saat");

            // Aylık performans hücreleri için renk ayarları
            foreach (DataGridViewRow row in ekipUyeleriDGV.Rows)
            {
                string performans = row.Cells["aylikPerformans"].Value.ToString();
                int performansYuzde = int.Parse(performans.Replace("%", ""));

                Color performansRenk;
                if (performansYuzde >= 90)
                    performansRenk = ColorTranslator.FromHtml("#27ae60");
                else if (performansYuzde >= 80)
                    performansRenk = ColorTranslator.FromHtml("#2ecc71");
                else if (performansYuzde >= 70)
                    performansRenk = ColorTranslator.FromHtml("#f39c12");
                else
                    performansRenk = ColorTranslator.FromHtml("#e74c3c");

                row.Cells["aylikPerformans"].Style.ForeColor = performansRenk;
                row.Cells["aylikPerformans"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                row.Cells["gorevAtaButon"].Style.BackColor = ColorTranslator.FromHtml("#5d4e9d");
                row.Cells["gorevAtaButon"].Style.ForeColor = Color.White;
            }
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerProfile manageProfile = new ManagerProfile();
            manageProfile.Show();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {

        }
    }
}
