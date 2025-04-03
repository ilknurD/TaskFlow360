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
    public partial class OfficerProfile : Form
    {
        public OfficerProfile()
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

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            OfficerHomepage officerHomepage = new OfficerHomepage();
            officerHomepage.Show();
            this.Close();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerProfile profile = new OfficerProfile();
            profile.Show();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerReportsPage officerReportsPage = new OfficerReportsPage();
            officerReportsPage.Show();
        }
    }
}
