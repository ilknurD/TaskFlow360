using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;

namespace TaskFlow360
{
    public partial class LoginForm : Form
    {
        private string connectionString = "Data Source=DESKTOP-57F0A7E\\SQLEXPRESS;Initial Catalog=TaskFlow360;Integrated Security=True;Encrypt=False";
        SqlConnection conn;
        SqlDataReader dr;
        SqlCommand cmd;

        public LoginForm()
        {
            InitializeComponent();
            this.Load += LoginForm_Load;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtMail.Text = "Mail";
            txtPassword.Text = "Şifre";

            txtMail.Enter += TextBox_Enter;
            txtMail.Leave += TextBox_Leave;
            txtPassword.Enter += TextBox_Enter;
            txtPassword.Leave += TextBox_Leave;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text == "Mail" || textBox.Text == "Şifre")
                {
                    textBox.Text = "";
                    if (textBox == txtPassword)
                    {
                        textBox.UseSystemPasswordChar = true;
                    }
                }
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (textBox == txtMail)
                    {
                        textBox.Text = "Mail";
                    }
                    else if (textBox == txtPassword)
                    {
                        textBox.Text = "Şifre";
                        textBox.UseSystemPasswordChar = false;
                    }
                }
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

        private void button1_Click(object sender, EventArgs e)
        {
            //String mail = txtMail.Text;
            //String password = txtPassword.Text;
            //Baglanti baglanti = new Baglanti();
            //baglanti.BaglantiAc();
            //KullaniciGiris kullaniciGiris = new KullaniciGiris(baglanti);
            //bool dogruMu = kullaniciGiris.GirisDogrula(mail, password);
            //if (dogruMu)
            //{
            //    MessageBox.Show("Giriş Başarılı");
            //    OfficerHomepage officerHomepage = new OfficerHomepage();
            //    this.Hide();
            //    officerHomepage.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Mail veya şifre hatalı!");
            //}
           OfficerHomepage officerHomepage = new OfficerHomepage();
           officerHomepage.Show();
           this.Hide();
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}