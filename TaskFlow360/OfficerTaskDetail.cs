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
    public partial class OfficerTaskDetail : Form
    {
        Baglanti bgl = new Baglanti();
        private int _talepEdenID;
        public OfficerTaskDetail(int talepEdenID)
        {
            InitializeComponent();
            _talepEdenID = talepEdenID;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void OfficerTaskDetail_Load(object sender, EventArgs e)
        {

            
            Label lblLoading = new Label();
            lblLoading.Text = "Çağrılar yükleniyor...";
            lblLoading.Location = new Point(pnlgecmisCagrilar.Width / 2 - 60, pnlgecmisCagrilar.Height / 2);
            lblLoading.AutoSize = true;
            pnlgecmisCagrilar.Controls.Add(lblLoading);
            

            int y = 20;

            bgl.BaglantiAc();

            string sorgu = @"SELECT CagriID, Baslik, Durum, OlusturmaTarihi, TeslimTarihi, CagriAciklama
                     FROM Cagri 
                     WHERE TalepEdenID = @TalepEdenID
                     ORDER BY OlusturmaTarihi DESC";

            SqlCommand cmd = new SqlCommand(sorgu, bgl.conn);
            cmd.Parameters.AddWithValue("@TalepEdenID", _talepEdenID);

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string tarihSaat = Convert.ToDateTime(dr["OlusturmaTarihi"]).ToString("dd.MM.yyyy HH:mm");
                string baslik = dr["Baslik"].ToString();
                string aciklama = dr["CagriAciklama"].ToString();

                AddGecmisCagriItem(tarihSaat, baslik, aciklama, ref y);
            }

            dr.Close();
            bgl.BaglantiKapat();

            pnlgecmisCagrilar.Controls.Remove(lblLoading); 
        }



        private void AddGecmisCagriItem(string dateTime, string title, string description, ref int y)
        {
            // Tarih Zaman
            Label lblDate = new Label();
            lblDate.Text = dateTime;
            lblDate.Location = new Point(20, y);
            lblDate.AutoSize = true;
            lblDate.ForeColor = Color.Gray;
            lblDate.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            pnlgecmisCagrilar.Controls.Add(lblDate);

            y += 20;

            // Başlık
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Location = new Point(20, y);
            lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTitle.ForeColor = Color.Black;
            lblTitle.AutoSize = true;
            pnlgecmisCagrilar.Controls.Add(lblTitle);

            y += 22;

            // Açıklama
            Label lblDesc = new Label();
            lblDesc.Text = description;
            lblDesc.Location = new Point(20, y);
            lblDesc.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblDesc.Size = new Size(pnlgecmisCagrilar.Width - 40, 40);
            lblDesc.ForeColor = Color.Black;
            lblDesc.AutoEllipsis = true;
            lblDesc.MaximumSize = new Size(pnlgecmisCagrilar.Width - 40, 0); // Otomatik satır atlaması için
            lblDesc.AutoSize = true;
            pnlgecmisCagrilar.Controls.Add(lblDesc);

            y += lblDesc.Height + 15;

            // Ayırıcı çizgi (opsiyonel)
            Panel separator = new Panel();
            separator.BackColor = Color.LightGray;
            separator.Height = 1;
            separator.Width = pnlgecmisCagrilar.Width - 40;
            separator.Location = new Point(20, y);
            pnlgecmisCagrilar.Controls.Add(separator);

            y += 10;
        }
    }
}
