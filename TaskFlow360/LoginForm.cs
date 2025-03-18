using System;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.Load += LoginForm_Load;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtUsername.Text = "Kullanıcı Adı";
            txtPassword.Text = "Şifre";

            txtUsername.Enter += TextBox_Enter;
            txtUsername.Leave += TextBox_Leave;
            txtPassword.Enter += TextBox_Enter;
            txtPassword.Leave += TextBox_Leave;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Text == "Kullanıcı Adı" || textBox.Text == "Şifre")
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
                    if (textBox == txtUsername)
                    {
                        textBox.Text = "Kullanıcı Adı";
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
            OfficerHomepage officerHomepage = new OfficerHomepage();
            officerHomepage.Show();
        }
    }
}