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
    public partial class ManagerProfile : Form
    {
        public ManagerProfile()
        {
            InitializeComponent();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerProfile manageProfile = new ManagerProfile();
            manageProfile.Show();
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

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void btnEkipYonetimi_Click(object sender, EventArgs e)
        {

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
            OrnekVerileriYukle();
            ConfigureEkibimDGV();
        }
        private void OrnekVerileriYukle()
        {
            // Ekip üyeleri için örnek veriler
            ekibimDGV.Rows.Add("Ayşe Kaya", "3", "85%", "3.2 saat");
            ekibimDGV.Rows.Add("Mustafa Şahin", "4", "70%", "3.8 saat");
            ekibimDGV.Rows.Add("Elif Aksu", "1", "95%", "2.5 saat");
            ekibimDGV.Rows.Add("Kemal Bulut", "0", "90%", "2.8 saat");

            // Aylık performans hücreleri için renk ayarları
            foreach (DataGridViewRow row in ekibimDGV.Rows)
            {
                string performans = row.Cells["ortCozumSuresi"].Value.ToString();
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

                row.Cells["ortCozumSuresi"].Style.ForeColor = performansRenk;
                row.Cells["ortCozumSuresi"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
            }
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

            // Event bağlantıları
            ekibimDGV.CellClick += (s, e) => ekibimDGV.ClearSelection();
            ekibimDGV.DataBindingComplete += (s, e) => ekibimDGV.ClearSelection();
        }

    }
}
