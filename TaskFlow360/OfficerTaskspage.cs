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
    public partial class OfficerTaskspage : Form
    {
        public OfficerTaskspage()
        {
            InitializeComponent();
        }

        private void GorevListesi(List<Gorev> gorevListesi)
        {
            //DataGridView dgvGorevler = new DataGridView();
            dgvGorevler.Dock = DockStyle.None;
            dgvGorevler.Size = new Size(1250,650);
            dgvGorevler.BackgroundColor = Color.White;
            dgvGorevler.GridColor = Color.LightGray;

            // Sütun başlıkları stil
            dgvGorevler.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 149, 237); // Mavi
            dgvGorevler.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGorevler.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Bold);

            // Sütunlar ekle
            dgvGorevler.Columns.Add("GorevBasligi", "Görev Başlığı");
            dgvGorevler.Columns.Add("Atayan", "Atayan");
            dgvGorevler.Columns.Add("Durum", "Durum");
            dgvGorevler.Columns.Add("BaslangicTarihi", "Başlangıç Tarihi");
            dgvGorevler.Columns.Add("BitisTarihi", "Bitiş Tarihi");
            dgvGorevler.Columns.Add("Islem", "İşlem");

            foreach (var gorev in gorevListesi)
            {
                int rowIndex = dgvGorevler.Rows.Add(gorev.GorevAdi, gorev.IlgiliKisi, gorev.Durum,
                    gorev.BaslangicTarihi.ToString("dd MMM yyyy"), gorev.BitisTarihi.ToString("dd MMM yyyy"), "Detay");

                // Duruma göre satır rengi
                DataGridViewRow row = dgvGorevler.Rows[rowIndex];
                if (gorev.Durum == "Atandı")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(230, 230, 250); // Açık mor
                else if (gorev.Durum == "Beklemede")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 239, 213); // Açık turuncu
                else if (gorev.Durum == "Çözüm Bekliyor")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(173, 216, 230); // Açık mavi
            }

            // DataGridView'i forma ekle
            this.Controls.Add(dgvGorevler);
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
            List<Gorev> gorevListesi = new List<Gorev>
            {
                new Gorev("Haftalık raporun hazırlanması", "Mehmet Demir", new DateTime(2025, 3, 19), "Yüksek", "Atandı", new DateTime(2025, 3, 16), new DateTime(2025, 3, 19)),
                new Gorev("Müşteri toplantısı için sunum hazırlama", "Ayşe Kaya", new DateTime(2025, 3, 20), "Orta", "Beklemede", new DateTime(2025, 3, 16), new DateTime(2025, 3, 20)),
                new Gorev("Proje dokümanlarının gözden geçirilmesi", "Mustafa Şahin", new DateTime(2025, 3, 21), "Düşük", "Çözüm Bekliyor", new DateTime(2025, 3, 15), new DateTime(2025, 3, 21))
            };

            GorevListesi(gorevListesi);
        }
    }
}
