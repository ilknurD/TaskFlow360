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
    public partial class ManagerHomepage : Form
    {
       
        public ManagerHomepage()
        {
            InitializeComponent();
            //IstatislikleriGoster();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }
        // Tasarım zamanında veya Form_Load'da çağırın
        private void ConfigureBekleyenCagrilarDGV()
        {
            // Temel seçim ayarları
            bekleyenCagrilarDGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            bekleyenCagrilarDGV.DefaultCellStyle.SelectionBackColor = bekleyenCagrilarDGV.DefaultCellStyle.BackColor;
            bekleyenCagrilarDGV.DefaultCellStyle.SelectionForeColor = bekleyenCagrilarDGV.DefaultCellStyle.ForeColor;

            // Başlık ayarları
            bekleyenCagrilarDGV.EnableHeadersVisualStyles = false;
            bekleyenCagrilarDGV.ColumnHeadersDefaultCellStyle.SelectionBackColor = bekleyenCagrilarDGV.ColumnHeadersDefaultCellStyle.BackColor;
            bekleyenCagrilarDGV.RowHeadersDefaultCellStyle.SelectionBackColor = Color.Transparent;

            // Görsel iyileştirmeler
            bekleyenCagrilarDGV.BorderStyle = BorderStyle.None;
            bekleyenCagrilarDGV.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            bekleyenCagrilarDGV.GridColor = Color.FromArgb(240, 240, 240);

            // Event bağlantıları
            bekleyenCagrilarDGV.CellClick += (s, e) => bekleyenCagrilarDGV.ClearSelection();
            bekleyenCagrilarDGV.DataBindingComplete += (s, e) => bekleyenCagrilarDGV.ClearSelection();
        }

        private void ConfigureEkipUyeleriDGV()
        {
            // Özel renk şeması
            ekipUyeleriDGV.DefaultCellStyle.BackColor = Color.White;
            ekipUyeleriDGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            // Seçim efekti kaldırma
            ekipUyeleriDGV.SelectionMode = DataGridViewSelectionMode.CellSelect;
            ekipUyeleriDGV.DefaultCellStyle.SelectionBackColor = ekipUyeleriDGV.DefaultCellStyle.BackColor;
            ekipUyeleriDGV.DefaultCellStyle.SelectionForeColor = ekipUyeleriDGV.DefaultCellStyle.ForeColor;


            //// Çift tıklamayı yönetme
            //ekipUyeleriDGV.CellDoubleClick += (s, e) => {
            //    if (e.RowIndex >= 0)
            //    {
            //        // Örnek: Seçili kullanıcıyı işleme alma
            //        var selectedUser = ekipUyeleriDGV.Rows[e.RowIndex].Cells["KullaniciAdi"].Value.ToString();
            //        MessageBox.Show($"{selectedUser} kullanıcısı seçildi");
            //    }
            //};
        }

        private void OrnekVerileriYukle()
        {
            // Bekleyen çağrılar için örnek veriler
            bekleyenCagrilarDGV.Rows.Add("#2458", "Rapor oluşturma sorunu", "Teknik", "Yüksek");
            bekleyenCagrilarDGV.Rows.Add("#2459", "Yeni müşteri kaydı hatası", "Yazılım", "Orta");
            bekleyenCagrilarDGV.Rows.Add("#2460", "Mail sistemi bağlantı sorunu", "Altyapı", "Normal");
            bekleyenCagrilarDGV.Rows.Add("#2461", "Fatura oluşturma problemi", "Destek", "Orta");

            // Aciliyet hücrelerine renk verme
            foreach (DataGridViewRow row in bekleyenCagrilarDGV.Rows)
            {
                string aciliyet = row.Cells["aciliyet"].Value.ToString();
                if (aciliyet == "Yüksek")
                    row.Cells["aciliyet"].Style.BackColor = ColorTranslator.FromHtml("#f85c5c");
                else if (aciliyet == "Orta")
                    row.Cells["aciliyet"].Style.BackColor = ColorTranslator.FromHtml("#f0ad4e");
                else
                    row.Cells["aciliyet"].Style.BackColor = ColorTranslator.FromHtml("#63c966");

                row.Cells["aciliyet"].Style.ForeColor = Color.White;
                row.Cells["aciliyet"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                row.Cells["ataButon"].Style.BackColor = ColorTranslator.FromHtml("#5d4e9d");
                row.Cells["ataButon"].Style.ForeColor = Color.White;
            }

            // Ekip üyeleri için örnek veriler
            ekipUyeleriDGV.Rows.Add("Ayşe Kaya", "3", "2", "85%", "3.2 saat");
            ekipUyeleriDGV.Rows.Add("Mustafa Şahin", "4", "1", "80%", "3.8 saat");
            ekipUyeleriDGV.Rows.Add("Elif Aksu", "1", "3", "95%", "2.5 saat");
            ekipUyeleriDGV.Rows.Add("Kemal Bulut", "0", "4", "90%", "2.8 saat");

            // Aylık performans hücreleri için renk ayarları
            foreach (DataGridViewRow row in ekipUyeleriDGV.Rows)
            {
                string performans = row.Cells["aylikPerformans"].Value.ToString();
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

                row.Cells["aylikPerformans"].Style.ForeColor = performansRenk;
                row.Cells["aylikPerformans"].Style.Font = new Font("Arial", 10, FontStyle.Bold);
                row.Cells["gorevAtaButon"].Style.BackColor = ColorTranslator.FromHtml("#5d4e9d");
                row.Cells["gorevAtaButon"].Style.ForeColor = Color.White;
            }
        }

        //private void GorevleriGoster(List<Gorev> gorevListesi)
        //{
        //    PnlGorevler.Controls.Clear();
        //    PnlGorevler.FlowDirection = FlowDirection.TopDown; // Dikey 
        //    PnlGorevler.WrapContents = false;
        //    PnlGorevler.AutoScroll = true;

        //    // Her bir görev için bir panel oluştur
        //    foreach (var gorev in gorevListesi)
        //    {
        //        Panel gorevPaneli = new Panel();
        //        int panelYuksekligi = 120; // Varsayılan yükseklik
        //        int panelGenisligi = PnlGorevler.Width - 20; // PnlGorevler genişliğine göre ayarlayın
        //        gorevPaneli.Size = new Size(panelGenisligi, panelYuksekligi);
        //        gorevPaneli.BorderStyle = BorderStyle.FixedSingle;
        //        gorevPaneli.BackColor = Color.White;

        //        Label lblGorevAdi = new Label();
        //        lblGorevAdi.Text = gorev.GorevAdi;
        //        lblGorevAdi.Location = new Point(10, 10);
        //        lblGorevAdi.Font = new Font("Century Gothic", 12, FontStyle.Bold);
        //        lblGorevAdi.AutoSize = true;

        //        // İlgili kişiyi gösteren label
        //        Label lblIlgiliKisi = new Label();
        //        lblIlgiliKisi.Text = "İlgili Kişi: " + gorev.IlgiliKisi;
        //        lblIlgiliKisi.Location = new Point(10, 30);
        //        lblIlgiliKisi.Font = new Font("Century Gothic", 10);
        //        lblIlgiliKisi.AutoSize = true;

        //        // Teslim tarihini gösteren label
        //        Label lblTeslimTarihi = new Label();
        //        lblTeslimTarihi.Text = "Teslim: " + gorev.TeslimTarihi.ToString("dd MMMM yyyy HH:mm");
        //        lblTeslimTarihi.Location = new Point(10, 50);
        //        lblTeslimTarihi.Font = new Font("Century Gothic", 9);
        //        lblTeslimTarihi.AutoSize = true;

        //        // Öncelik durumu için label
        //        Label lblOncelik = new Label();
        //        lblOncelik.Text = gorev.Oncelik;
        //        lblOncelik.Location = new Point(gorevPaneli.Width - 80, 10);
        //        lblOncelik.Font = new Font("Century Gothic", 10, FontStyle.Bold);
        //        lblOncelik.AutoSize = true;

        //        // Önceliğe göre arka plan rengi
        //        if (gorev.Oncelik == "Yüksek")
        //        {
        //            lblOncelik.ForeColor = Color.Red;
        //        }
        //        else if (gorev.Oncelik == "Orta")
        //        {
        //            lblOncelik.ForeColor = Color.Orange;
        //        }
        //        else
        //        {
        //            lblOncelik.ForeColor = Color.Green;
        //        }

        //        gorevPaneli.Controls.Add(lblGorevAdi);
        //        gorevPaneli.Controls.Add(lblIlgiliKisi);
        //        gorevPaneli.Controls.Add(lblTeslimTarihi);
        //        gorevPaneli.Controls.Add(lblOncelik);

        //        PnlGorevler.Controls.Add(gorevPaneli);
        //    }
        //}

        //private void IstatislikleriGoster()
        //{
        //    flowLayoutPanel1.Controls.Clear();
        //    flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
        //    flowLayoutPanel1.WrapContents = false;
        //    flowLayoutPanel1.AutoScroll = true;

        //    Size panelBoyut = new Size(270, 100);
        //    Padding panelMargin = new Padding(15, 0, 15, 0); // Sol, üst, sağ, alt boşluk

        //    // Tamamlanan Görevler Kutucuğu
        //    Panel tamamlananPanel = new Panel();
        //    tamamlananPanel.Size = panelBoyut;
        //    tamamlananPanel.BackColor = Color.LightGreen;
        //    tamamlananPanel.Margin = panelMargin; // Panel aralarındaki mesafe
        //    Label lblTamamlananSayisi = new Label();
        //    lblTamamlananSayisi.Text = "Tamamlanan Görevler";
        //    lblTamamlananSayisi.Font = new Font("Arial", 18, FontStyle.Bold);
        //    lblTamamlananSayisi.Location = new Point(50, 20);
        //    Label lblTamamlanan = new Label();
        //    lblTamamlanan.Text = "24";
        //    lblTamamlanan.Location = new Point(30, 60);
        //    lblTamamlanan.Font = new Font("Arial", 10);

        //    // Ortalama Çözüm Süresi Görevler Kutucuğu
        //    Panel ortalamaCozumSuresiPanel = new Panel();
        //    ortalamaCozumSuresiPanel.Size = panelBoyut;
        //    ortalamaCozumSuresiPanel.BackColor = Color.LightSalmon;
        //    ortalamaCozumSuresiPanel.Margin = panelMargin;
        //    Label lblortalamaCozumSuresiPanel = new Label();
        //    lblortalamaCozumSuresiPanel.Text = "Ortalama Çözüm Süresi";
        //    lblortalamaCozumSuresiPanel.Font = new Font("Arial", 18, FontStyle.Bold);
        //    lblortalamaCozumSuresiPanel.Location = new Point(50, 20);
        //    Label lblortalamaCozum = new Label();
        //    lblortalamaCozum.Text = "1.2 Gün";
        //    lblortalamaCozum.Location = new Point(30, 60);
        //    lblortalamaCozum.Font = new Font("Arial", 10);

        //    // Performans Görevler Kutucuğu
        //    Panel performansPanel = new Panel();
        //    performansPanel.Size = panelBoyut;
        //    performansPanel.BackColor = Color.LightCoral;
        //    performansPanel.Margin = panelMargin;
        //    Label lblperformansPanel = new Label();
        //    lblperformansPanel.Text = "Performans Puanı";
        //    lblperformansPanel.Font = new Font("Arial", 18, FontStyle.Bold);
        //    lblperformansPanel.Location = new Point(50, 20);
        //    Label lblPerformans = new Label();
        //    lblPerformans.Text = "87/100";
        //    lblPerformans.Location = new Point(50, 60);
        //    lblPerformans.Font = new Font("Arial", 10);

        //    // Prim Tahmini Görevler Kutucuğu
        //    Panel primPanel = new Panel();
        //    primPanel.Size = panelBoyut;
        //    primPanel.BackColor = Color.LightBlue;
        //    primPanel.Margin = panelMargin;
        //    Label lblprimTahmin = new Label();
        //    lblprimTahmin.Text = "Aylık Prim Tahmini";
        //    lblprimTahmin.Font = new Font("Arial", 18, FontStyle.Bold);
        //    lblprimTahmin.Location = new Point(50, 20);
        //    Label lblPrim = new Label();
        //    lblPrim.Text = "₺ 1,500";
        //    lblPrim.Location = new Point(50, 60);
        //    lblPrim.Font = new Font("Arial", 10);

        //    // Kutucukları FlowLayoutPanel'e ekleyin
        //    tamamlananPanel.Controls.Add(lblTamamlananSayisi);
        //    tamamlananPanel.Controls.Add(lblTamamlanan);
        //    flowLayoutPanel1.Controls.Add(tamamlananPanel);

        //    ortalamaCozumSuresiPanel.Controls.Add(lblortalamaCozumSuresiPanel);
        //    ortalamaCozumSuresiPanel.Controls.Add(lblortalamaCozum);
        //    flowLayoutPanel1.Controls.Add(ortalamaCozumSuresiPanel);

        //    performansPanel.Controls.Add(lblperformansPanel);
        //    performansPanel.Controls.Add(lblPerformans);
        //    flowLayoutPanel1.Controls.Add(performansPanel);

        //    primPanel.Controls.Add(lblprimTahmin);
        //    primPanel.Controls.Add(lblPrim);
        //    flowLayoutPanel1.Controls.Add(primPanel);
        //}

        private void ManagerHomepage_Load(object sender, EventArgs e)
        {
            OrnekVerileriYukle();
            ConfigureBekleyenCagrilarDGV();
            ConfigureEkipUyeleriDGV();
            //List<Gorev> gorevListesi = new List<Gorev>()
            //{
            //    new Gorev()
            //    {
            //        GorevAdi = "Haftalık raporun hazırlanması",
            //        IlgiliKisi = "Mehmet Demir",
            //        TeslimTarihi = DateTime.Now.AddHours(5),
            //        Oncelik = "Yüksek",
            //        Durum = "Atandı",
            //        BaslangicTarihi = DateTime.Now,
            //        BitisTarihi = DateTime.Now.AddHours(5)
            //    },
            //    new Gorev()
            //    {
            //        GorevAdi = "Müşteri toplantısı için sunum hazırlama",
            //        IlgiliKisi = "Ayşe Kaya",
            //        TeslimTarihi = DateTime.Now.AddHours(3),
            //        Oncelik = "Orta",
            //        Durum = "Beklemede",
            //        BaslangicTarihi = DateTime.Now,
            //        BitisTarihi = DateTime.Now.AddHours(2)
            //    },
            //    new Gorev()
            //    {
            //        GorevAdi = "Proje dökümanlarının gözden geçirilmesi",
            //        IlgiliKisi = "Mustafa Şahin",
            //        TeslimTarihi = DateTime.Now.AddDays(1),
            //        Oncelik = "Normal",
            //        Durum = "Çözüm Bekliyor",
            //        BaslangicTarihi = DateTime.Now.AddDays(-1),
            //        BitisTarihi = DateTime.Now.AddDays(1)
            //    }
            //};

            //GorevleriGoster(gorevListesi);
        }

        private void bekleyenCagrilarDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // "Ata" butonuna tıklandığında görev atama işlemi
            if (e.ColumnIndex == 4 && e.RowIndex >= 0) // 4 = "Ata" butonu kolonu
            {
                string cagriId = bekleyenCagrilarDGV.Rows[e.RowIndex].Cells["cagriId"].Value.ToString();
                string baslik = bekleyenCagrilarDGV.Rows[e.RowIndex].Cells["baslik"].Value.ToString();

                // Görev atama penceresi veya diyalog kutusu gösterilebilir
                MessageBox.Show($"'{baslik}' başlıklı çağrı (ID: {cagriId}) için görev atama işlemi başlatıldı.");
            }
        }

        private void ekipUyeleriDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ekipUyeleriDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // "Görev Ata" butonuna tıklandığında çalışana görev atama işlemi
            if (e.ColumnIndex == 5 && e.RowIndex >= 0) // 5 = "Görev Ata" butonu kolonu
            {
                string calisan = ekipUyeleriDGV.Rows[e.RowIndex].Cells["calisan"].Value.ToString();

                // Çalışana görev atama penceresi veya diyalog kutusu gösterilebilir
                MessageBox.Show($"{calisan} için görev atama işlemi başlatıldı.");
            }
        }

        private void gecikenKutu_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
