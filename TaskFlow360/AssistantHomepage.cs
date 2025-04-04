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
    public partial class AssistantHomepage : Form
    {
        public AssistantHomepage()
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

        private void AssistantHomepage_Load(object sender, EventArgs e)
        {
            TarihVeSaatGoster();
            IstatislikleriGoster();
            OrnekVerileriYukle();
        }
        private void TarihVeSaatGoster()
        {
            DateTime suAn = DateTime.Now;
            string formatliTarihSaat = suAn.ToString("dd MMMM yyyy, dddd HH:mm");
            lblTarihSaat.Text = formatliTarihSaat;
        }
        private void IstatislikleriGoster()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.AutoScroll = true;

            Size panelBoyut = new Size(200, 100); // Panel boyutları
            Padding panelMargin = new Padding(15, 0, 15, 0); // Panel aralarındaki mesafe

            void PanelEkle(Color arkaPlan, string sayi, string metin)
            {
                Panel panel = new Panel
                {
                    Size = panelBoyut,
                    BackColor = arkaPlan,
                    Margin = panelMargin
                };

                Label lblSayi = new Label
                {
                    Text = sayi,
                    Font = new Font("Arial", 18, FontStyle.Bold),
                    AutoSize = false,
                    Size = new Size(panel.Width, 30), // Genişlik panel kadar
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };

                Label lblMetin = new Label
                {
                    Text = metin,
                    Font = new Font("Arial", 10),
                    AutoSize = false,
                    Size = new Size(panel.Width, 30), // Genişlik panel kadar
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };

                // Label'ları dikey olarak ortalamak için konum ayarla
                lblSayi.Location = new Point(0, 20);
                lblMetin.Location = new Point(0, 60);

                panel.Controls.Add(lblSayi);
                panel.Controls.Add(lblMetin);
                flowLayoutPanel1.Controls.Add(panel);
            }

            // Panelleri ekleyelim
            PanelEkle(Color.LightBlue, "24", "Tamamlanan");
            PanelEkle(Color.LightCoral, "12", "Devam Eden");
            PanelEkle(Color.MediumAquamarine, "3", "Geciken");
            PanelEkle(Color.Khaki, "39", "Toplam");
        }

        private void OrnekVerileriYukle()
        {
            // Bekleyen çağrılar için örnek veriler
            SonAcilanCagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek", "14/04/2022");
            SonAcilanCagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta", "04/04/2022");
            SonAcilanCagrilarDGV.Rows.Add("#2460", "Mail sistemi bağlantı sorunu", "Altyapı", "Normal", "03/04/2022");
            SonAcilanCagrilarDGV.Rows.Add("#2461", "Fatura oluşturma problemi", "Destek", "Orta", "20/04/2022");
            SonAcilanCagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek", "14/04/2022");
            SonAcilanCagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta", "04/04/2022");
            SonAcilanCagrilarDGV.Rows.Add("#2460", "Mail sistemi bağlantı sorunu", "Altyapı", "Normal", "03/04/2022");
            SonAcilanCagrilarDGV.Rows.Add("#2461", "Fatura oluşturma problemi", "Destek", "Orta", "20/04/2022");
            SonAcilanCagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek", "14/04/2022");
            SonAcilanCagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta", "04/04/2022");

            // oncelik hücrelerine renk verme
            foreach (DataGridViewRow row in SonAcilanCagrilarDGV.Rows)
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
        }

    }
}
