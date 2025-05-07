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
    public partial class ManagerAdminPage : Form
    {
        private int yoneticiId;
        public ManagerAdminPage(int yoneticiId = 0)
        {
            InitializeComponent();
            this.yoneticiId = yoneticiId;
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
            this.Close();
            ManagerHomepage managerHomepage = new ManagerHomepage();
            managerHomepage.Show();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerProfile managerProfile = new ManagerProfile();
            managerProfile.Show();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerTasks managerTasks = new ManagerTasks();
            managerTasks.Show();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            
        }

        private void btnEkipYonetimi_Click(object sender, EventArgs e)
        {
            this.Close();
            ManagerAdminPage managerAdminPage = new ManagerAdminPage();
            managerAdminPage.Show();
        }

        private void ManagerAdminPage_Load(object sender, EventArgs e)
        {
            OrnekVerileriYukle();
        }

        private void OrnekVerileriYukle()
        {
            ekibimDGV.Rows.Add("Ayşe Kaya", "3", "2", "85%", "3.2 saat");
            ekibimDGV.Rows.Add("Mustafa Şahin", "4", "1", "80%", "3.8 saat");
            ekibimDGV.Rows.Add("Elif Aksu", "1", "3", "95%", "2.5 saat");
            ekibimDGV.Rows.Add("Kemal Bulut", "0", "4", "90%", "2.8 saat");
        }




    }
}
