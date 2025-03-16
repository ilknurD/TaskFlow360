using System;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.Load += LoginForm_Load; // Formun Load olayını bağla
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde TextBox'lara varsayılan metinleri ata
            txtUsername.Text = "Kullanıcı Adı";
            txtPassword.Text = "Şifre";

            // TextBox'ların Enter ve Leave olaylarını bağla
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
                // Eğer TextBox'ta varsayılan metin varsa, temizle
                if (textBox.Text == "Kullanıcı Adı" || textBox.Text == "Şifre")
                {
                    textBox.Text = "";
                    // Şifre TextBox'ı için karakterleri gizle
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
                // Eğer TextBox boşsa, varsayılan metni geri yükle
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
            Application.Exit();
        }
    }
}