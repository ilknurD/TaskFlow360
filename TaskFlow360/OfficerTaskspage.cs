using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TaskFlow360
{
    public partial class OfficerTaskspage : Form
    {
        public OfficerTaskspage()
        {
            InitializeComponent();
        }

        private void GorevListesi(List<Gorev> gorevListesi)
        {
            // DataGridView ayarları
            dgvGorevler.Dock = DockStyle.None;
            dgvGorevler.Size = new Size(1250, 650);
            dgvGorevler.BackgroundColor = Color.White;
            dgvGorevler.GridColor = Color.LightGray;

            // Sütun başlıkları stil
            dgvGorevler.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 149, 237); // Mavi
            dgvGorevler.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGorevler.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);

            // Sütunlar ekle
            dgvGorevler.Columns.Add("GorevBasligi", "Görev Başlığı");
            dgvGorevler.Columns.Add("Atayan", "Atayan");
            dgvGorevler.Columns.Add("Durum", "Durum");
            dgvGorevler.Columns.Add("BaslangicTarihi", "Başlangıç Tarihi");
            dgvGorevler.Columns.Add("BitisTarihi", "Bitiş Tarihi");

            DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn();
            btnColumn.Name = "Islem";
            btnColumn.HeaderText = "İşlem";
            btnColumn.Text = "Düzenle";
            btnColumn.UseColumnTextForButtonValue = true;  // Bu, her satırda butonun "Düzenle" olarak görünmesini sağlar
            dgvGorevler.Columns.Add(btnColumn);

            dgvGorevler.ReadOnly = true; // Tüm hücreler okuma modunda olacak, düzenlenemez

            // Satırları ekleyip, durumlarına göre arka plan rengi ayarlama
            foreach (var gorev in gorevListesi)
            {
                int rowIndex = dgvGorevler.Rows.Add(gorev.GorevAdi, gorev.IlgiliKisi, gorev.Durum,
                    gorev.BaslangicTarihi.ToString("dd MMM yyyy"), gorev.BitisTarihi.ToString("dd MMM yyyy"), "Detay");

                // Duruma göre satır rengi
                DataGridViewRow row = dgvGorevler.Rows[rowIndex];
                Color backColor;

                if (gorev.Durum == "Atandı")
                    backColor = Color.FromArgb(230, 230, 250); // Açık mor
                else if (gorev.Durum == "Beklemede")
                    backColor = Color.FromArgb(255, 239, 213); // Açık turuncu
                else if (gorev.Durum == "Çözüm Bekliyor")
                    backColor = Color.FromArgb(173, 216, 230); // Açık mavi
                else
                    backColor = Color.White; // Varsayılan renk

                // Satırın arka plan rengini ayarlama
                row.DefaultCellStyle.BackColor = backColor;

                // Seçim sırasında arka plan rengini aynı yapmak için SelectionBackColor ayarlama
                row.DefaultCellStyle.SelectionBackColor = backColor;

                // Seçim metin rengini siyah yapmak
                row.DefaultCellStyle.SelectionForeColor = Color.Black;

                btnColumn.DefaultCellStyle.BackColor = backColor;  // Butonun arka plan rengini değiştir
                btnColumn.DefaultCellStyle.ForeColor = Color.Black;  // Buton yazı rengini değiştir
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

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerHomepage homepage = new OfficerHomepage();
            homepage.ShowDialog();
        }

        private void OfficerTaskspage_Load(object sender, EventArgs e)
        {
            txtArama.Text = "Ara..."; // Başlangıçta görünmesini istediğiniz metni yazın
            txtArama.ForeColor = Color.Gray; // Başlangıçta gri renkte olsun

            dgvGorevler.RowTemplate.Height = 40;
            dgvGorevler.DefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            List<Gorev> gorevListesi = new List<Gorev>
            {
                new Gorev("Haftalık raporun hazırlanması", "Mehmet Demir", new DateTime(2025, 3, 19), "Yüksek", "Atandı", new DateTime(2025, 3, 16), new DateTime(2025, 3, 19)),
                new Gorev("Müşteri toplantısı için sunum hazırlama", "Ayşe Kaya", new DateTime(2025, 3, 20), "Orta", "Beklemede", new DateTime(2025, 3, 16), new DateTime(2025, 3, 20)),
                new Gorev("Proje dokümanlarının gözden geçirilmesi", "Mustafa Şahin", new DateTime(2025, 3, 21), "Düşük", "Çözüm Bekliyor", new DateTime(2025, 3, 15), new DateTime(2025, 3, 21))
            };

            GorevListesi(gorevListesi);
        }

        private void txtArama_Enter_1(object sender, EventArgs e)
        {
            if (txtArama.Text == "Ara...")
            {
                txtArama.Text = ""; // Eğer kullanıcı tıklarsa metni temizler
                txtArama.ForeColor = Color.Black; // Yazı rengini normal renk yapar
            }
        }

        private void txtArama_Leave_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArama.Text)) // Eğer kullanıcı metin girmediyse
            {
                txtArama.Text = "Ara...";
                txtArama.ForeColor = Color.Gray;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            OfficerProfile profile = new OfficerProfile();
            profile.Show();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerReportsPage officerReportsPage = new OfficerReportsPage();
            officerReportsPage.Show();
        }
    }
}
