using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using System.Net;

namespace TaskFlow360
{
    public partial class OfficerTaskspage : Form
    {
        public OfficerTaskspage()
        {
            InitializeComponent();
            dgvGorevler.CellClick += new DataGridViewCellEventHandler(dgvGorevler_CellClick);
            dgvGorevler.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dgvGorevler_ColumnHeaderMouseClick);
            LogEkle("OfficerTaskspage formu başlatıldı", "Form", "OfficerTaskspage");
        }

        private Baglanti baglantiNesnesi = new Baglanti();

        private void LogEkle(string islemDetaylari, string islemTipi, string tabloAdi)
        {
            try
            {
                baglantiNesnesi.BaglantiAc();
                string sorgu = @"INSERT INTO Log (IslemTarihi, KullaniciID, IslemTipi, TabloAdi, IslemDetaylari, IPAdresi) 
                                VALUES (@IslemTarihi, @KullaniciID, @IslemTipi, @TabloAdi, @IslemDetaylari, @IPAdresi)";

                using (SqlCommand cmd = new SqlCommand(sorgu, baglantiNesnesi.conn))
                {
                    cmd.Parameters.AddWithValue("@IslemTarihi", DateTime.Now);
                    cmd.Parameters.AddWithValue("@KullaniciID", KullaniciBilgi.KullaniciID);
                    cmd.Parameters.AddWithValue("@IslemTipi", islemTipi);
                    cmd.Parameters.AddWithValue("@TabloAdi", tabloAdi);
                    cmd.Parameters.AddWithValue("@IslemDetaylari", islemDetaylari);
                    cmd.Parameters.AddWithValue("@IPAdresi", GetLocalIPAddress());

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Log kayıt hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglantiNesnesi.BaglantiKapat();
            }
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "IP Adresi Bulunamadı";
        }

        private void CagrilariYukle()
        {
            try
            {
                LogEkle("Çağrılar yüklenmeye başlandı", "Okuma", "Cagri");
                baglantiNesnesi.BaglantiAc();

                if (string.IsNullOrEmpty(KullaniciBilgi.KullaniciID))
                {
                    MessageBox.Show("Kullanıcı bilgisi alınamadı. Lütfen tekrar giriş yapın.",
                                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Sorguyu düzeltelim - te.TalepEden alanını string olarak alıyoruz
                string sorgu = @"SELECT c.CagriID, c.Baslik, te.TalepEden, c.Durum, c.OlusturmaTarihi, 
                   c.TeslimTarihi, c.AtananKullaniciID, c.CagriKategori, c.Oncelik, c.HedefSure 
                   FROM Cagri c
                   LEFT JOIN TalepEdenler te ON c.TalepEdenID = te.TalepEdenID
                   WHERE c.AtananKullaniciID = @KullaniciID
                   ORDER BY c.OlusturmaTarihi DESC";

                SqlCommand komut = new SqlCommand(sorgu, baglantiNesnesi.conn);
                int kullaniciID;

                if (int.TryParse(KullaniciBilgi.KullaniciID, out kullaniciID))
                {
                    komut.Parameters.AddWithValue("@KullaniciID", kullaniciID);

                    // Veri çekme işlemini dene
                    SqlDataAdapter adapter = new SqlDataAdapter(komut);
                    DataTable veriTablosu = new DataTable();
                    adapter.Fill(veriTablosu);

                    // Veri geldi mi kontrol et
                    if (veriTablosu.Rows.Count > 0)
                    {
                        LogEkle($"{veriTablosu.Rows.Count} adet çağrı başarıyla yüklendi", "Okuma", "Cagri");
                        // DataGridView'i temizle ve yeniden ayarla
                        dgvGorevler.DataSource = null;
                        dgvGorevler.Columns.Clear();
                        dgvGorevler.DataSource = veriTablosu;
                        SutunAyarla();
                        SetRenkler();
                    }
                    else
                    {
                        LogEkle("Bu kullanıcıya atanmış görev bulunamadı", "Okuma", "Cagri");
                        MessageBox.Show("Bu kullanıcıya atanmış görev bulunamadı.",
                                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Kullanıcı ID'si geçersiz.",
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LogEkle($"Hata: Çağrı verileri yüklenirken hata oluştu - {ex.Message}", "Hata", "Cagri");
                MessageBox.Show("Çağrı verileri yüklenirken hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglantiNesnesi.BaglantiKapat();
            }
        }

        private void SetRenkler()
        {
            foreach (DataGridViewRow row in dgvGorevler.Rows)
            {
                if (row.Cells["Durum"].Value != null)
                {
                    string durum = row.Cells["Durum"].Value.ToString();

                    Color textColor;
                    Color backColor;
                    FontStyle fontStyle = FontStyle.Bold;

                    switch (durum)
                    {
                        case "Atandı":
                            textColor = Color.MediumSlateBlue;
                            backColor = Color.FromArgb(235, 230, 250);
                            break;
                        case "Beklemede":
                            textColor = Color.OrangeRed;
                            backColor = Color.FromArgb(255, 245, 230);
                            break;
                        case "Tamamlandı":
                            textColor = Color.SeaGreen;
                            backColor = Color.FromArgb(230, 255, 240);
                            break;
                        case "İptal Edildi":
                            textColor = Color.IndianRed;
                            backColor = Color.FromArgb(250, 230, 230);
                            break;
                        case "Gecikti":
                            textColor = Color.Crimson;
                            backColor = Color.FromArgb(255, 230, 230);
                            break;
                        default:
                            textColor = Color.Black;
                            backColor = Color.White;
                            fontStyle = FontStyle.Regular;
                            break;
                    }

                    var durumCell = row.Cells["Durum"];
                    durumCell.Style.ForeColor = textColor;
                    durumCell.Style.BackColor = backColor;
                    durumCell.Style.Font = new Font("Century Gothic", 10, fontStyle);
                }

                if (row.Cells["Oncelik"].Value != null)
                {
                    string oncelik = row.Cells["Oncelik"].Value.ToString();

                    Color textColor;
                    FontStyle fontStyle = FontStyle.Bold;

                    switch (oncelik)
                    {
                        case "Düşük":
                            textColor = Color.Green;
                            break;
                        case "Orta":
                            textColor = Color.Orange;
                            break;
                        case "Yüksek":
                            textColor = Color.Red;
                            break;
                        default:
                            textColor = Color.Black;
                            fontStyle = FontStyle.Regular;
                            break;
                    }

                    var oncelikCell = row.Cells["Oncelik"];
                    oncelikCell.Style.ForeColor = textColor;
                    oncelikCell.Style.Font = new Font("Century Gothic", 10, fontStyle);
                }
                dgvGorevler.DefaultCellStyle.SelectionBackColor = Color.FromArgb(179, 157, 219);
            }
        }

        private void SutunAyarla()
        {
            try
            {
                if (dgvGorevler.Columns.Contains("AtananKullaniciID"))
                    dgvGorevler.Columns["AtananKullaniciID"].Visible = false;

                dgvGorevler.Columns["CagriID"].DisplayIndex = 0;
                dgvGorevler.Columns["Baslik"].DisplayIndex = 1;
                dgvGorevler.Columns["TalepEden"].DisplayIndex = 2;
                dgvGorevler.Columns["CagriKategori"].DisplayIndex = 3;
                dgvGorevler.Columns["Oncelik"].DisplayIndex = 4;
                dgvGorevler.Columns["Durum"].DisplayIndex = 5;
                dgvGorevler.Columns["OlusturmaTarihi"].DisplayIndex = 6;
                dgvGorevler.Columns["TeslimTarihi"].DisplayIndex = 7;
                dgvGorevler.Columns["HedefSure"].DisplayIndex = 8;

                if (!dgvGorevler.Columns.Contains("Detay"))
                {
                    DataGridViewButtonColumn detayButonu = new DataGridViewButtonColumn();
                    detayButonu.HeaderText = "Detay";
                    detayButonu.Text = "Detay";
                    detayButonu.UseColumnTextForButtonValue = true;
                    detayButonu.Name = "Detay";
                    detayButonu.DefaultCellStyle.BackColor = Color.FromArgb(179, 157, 219);
                    detayButonu.DefaultCellStyle.ForeColor = Color.White;
                    detayButonu.DefaultCellStyle.SelectionBackColor = Color.FromArgb(149, 127, 189);
                    detayButonu.DefaultCellStyle.SelectionForeColor = Color.White;
                    dgvGorevler.Columns.Add(detayButonu);
                }
            }
            catch (Exception ex)
            {
                LogEkle($"Hata: Sütunlar ayarlanırken hata oluştu - {ex.Message}", "Hata", "Cagri");
                MessageBox.Show("Sütunlar ayarlanırken hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            foreach (DataGridViewColumn column in dgvGorevler.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void dgvGorevler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    DataGridViewColumn column = dgvGorevler.Columns[e.ColumnIndex];

                    if (column is DataGridViewButtonColumn && column.Name == "Detay")
                    {
                        object cagriIDObj = dgvGorevler.Rows[e.RowIndex].Cells["CagriID"].Value;
                        object talepEdenObj = dgvGorevler.Rows[e.RowIndex].Cells["TalepEden"].Value;

                        if (cagriIDObj != null && cagriIDObj != DBNull.Value)
                        {
                            int cagriID = Convert.ToInt32(cagriIDObj);
                            LogEkle($"Çağrı detayı açıldı - Çağrı ID: {cagriID}", "Okuma", "Cagri");

                            int talepEdenID = 0;

                            // TalepEden adından ID'sini al
                            if (talepEdenObj != null && talepEdenObj != DBNull.Value)
                            {
                                string talepEdenAd = talepEdenObj.ToString();
                                talepEdenID = GetTalepEdenIDByName(talepEdenAd);
                            }

                            using (TaskDetail detayForm = new TaskDetail(cagriID, talepEdenID))
                            {
                                detayForm.ShowDialog();
                            }

                            CagrilariYukle();
                        }
                        else
                        {
                            MessageBox.Show("Çağrı ID'si bulunamadı.",
                                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Çağrı detayı açılırken hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvGorevler_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Sütun başlığına tıklandığında hiçbir şey yapma
            return;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            LogEkle("Kapat butonuna tıklandı", "Buton", "OfficerTaskspage");
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            LogEkle("Küçült butonuna tıklandı", "Buton", "OfficerTaskspage");
            WindowState = FormWindowState.Minimized;
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            LogEkle("Çıkış butonuna tıklandı", "Buton", "OfficerTaskspage");
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
            this.Close();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            LogEkle("Anasayfa butonuna tıklandı", "Buton", "OfficerTaskspage");
            OfficerHomepage homepage = new OfficerHomepage();
            homepage.ShowDialog();
            this.Close();
        }

        private void OfficerTaskspage_Load(object sender, EventArgs e)
        {
            txtArama.Text = "Ara...";
            txtArama.ForeColor = Color.Gray;

            dgvGorevler.RowTemplate.Height = 40;
            dgvGorevler.DefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            
            // Sütun başlıklarının sıralama özelliğini kapat
            dgvGorevler.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvGorevler.ColumnHeadersDefaultCellStyle.BackColor;
            dgvGorevler.EnableHeadersVisualStyles = false;
            dgvGorevler.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgvGorevler.ColumnHeadersDefaultCellStyle.ForeColor;
            dgvGorevler.AllowUserToOrderColumns = false;
            dgvGorevler.AllowUserToResizeColumns = true;
            dgvGorevler.MultiSelect = false;

            foreach (DataGridViewColumn column in dgvGorevler.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            CagrilariYukle();

            if (!IsEventHandlerAttached(txtArama, "TextChanged", "txtArama_TextChanged"))
            {
                txtArama.TextChanged += new EventHandler(txtArama_TextChanged);
            }

            cmbDurum.Items.AddRange(new string[] { "Tümü", "Atandı", "Beklemede", "Tamamlandı", "İptal Edildi", "Gecikti" });
            cmbOncelik.Items.AddRange(new string[] { "Tümü", "Düşük", "Orta", "Yüksek" });
            cmbKategori.Items.AddRange(new string[] { "Tümü", "Donanım", "Yazılım", "Ağ", "Erişim Talebi", "Mail Problemleri",
            "Veri Yedekleme", "Sistem Arızası", "Kullanıcı İşlemleri", "Genel Talep", "Raporlar" });
        }


        private bool IsEventHandlerAttached(Control control, string eventName, string handlerName)
        {
            var events = control.GetType().GetProperty("Events",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(control);

            if (events != null)
            {
                var eventField = events.GetType().GetField(eventName);
                if (eventField != null)
                {
                    var eventDelegate = eventField.GetValue(events) as Delegate;
                    if (eventDelegate != null)
                    {
                        foreach (var handler in eventDelegate.GetInvocationList())
                        {
                            if (handler.Method.Name == handlerName)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private void txtArama_TextChanged(object sender, EventArgs e)
        {
            if (txtArama.Text != "Ara..." && !string.IsNullOrWhiteSpace(txtArama.Text))
            {
                string aramaMetni = txtArama.Text.ToLower();

                if (dgvGorevler.DataSource is DataTable dt)
                {
                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("Baslik LIKE '%{0}%' OR TalepEden LIKE '%{0}%' OR CagriKategori LIKE '%{0}%'",
                                  aramaMetni.Replace("'", "''"));

                    SetRenkler();
                }
            }
            else if (dgvGorevler.DataSource is DataTable dt)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Empty;

                SetRenkler();
            }
        }

        private void txtArama_Enter_1(object sender, EventArgs e)
        {
            if (txtArama.Text == "Ara...")
            {
                txtArama.Text = ""; 
                txtArama.ForeColor = Color.Black; 
            }
        }

        private void txtArama_Leave_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArama.Text))
            {
                txtArama.Text = "Ara...";
                txtArama.ForeColor = Color.Gray;
            }
        }

        private void CagrilariFiltrele()
        {
            try
            {
                string durum = cmbDurum.SelectedItem?.ToString() ?? "Tümü";
                string oncelik = cmbOncelik.SelectedItem?.ToString() ?? "Tümü";
                string kategori = cmbKategori.SelectedItem?.ToString() ?? "Tümü";

                // Temel sorgu
                string query = @"SELECT c.CagriID, c.Baslik, te.TalepEden, c.Durum, c.OlusturmaTarihi, 
                       c.TeslimTarihi, c.AtananKullaniciID, c.CagriKategori, c.Oncelik, c.HedefSure 
                       FROM Cagri c
                       LEFT JOIN TalepEdenler te ON c.TalepEdenID = te.TalepEdenID
                       WHERE c.AtananKullaniciID = @KullaniciID";

                // Filtreleri koşullu olarak ekle
                if (durum != "Tümü")
                    query += " AND c.Durum = @Durum";

                if (oncelik != "Tümü")
                    query += " AND c.Oncelik = @Oncelik";

                if (kategori != "Tümü")
                    query += " AND c.CagriKategori = @Kategori";

                query += " ORDER BY c.OlusturmaTarihi DESC";

                baglantiNesnesi.BaglantiAc();

                using (SqlCommand cmd = new SqlCommand(query, baglantiNesnesi.conn))
                {
                    // Her zaman KullaniciID parametresini ekle
                    cmd.Parameters.AddWithValue("@KullaniciID", int.Parse(KullaniciBilgi.KullaniciID));

                    // Sadece gerekli parametreleri ekle
                    if (durum != "Tümü")
                        cmd.Parameters.AddWithValue("@Durum", durum);

                    if (oncelik != "Tümü")
                        cmd.Parameters.AddWithValue("@Oncelik", oncelik);

                    if (kategori != "Tümü")
                        cmd.Parameters.AddWithValue("@Kategori", kategori);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvGorevler.DataSource = dt;
                    SutunAyarla();
                    SetRenkler();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filtreleme sırasında hata oluştu: " + ex.Message,
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglantiNesnesi.BaglantiKapat();
            }
        }
        private int GetTalepEdenIDByName(string talepEdenAd)
        {
            int talepEdenID = -1;

            try
            {
                string sorgu = "SELECT TalepEdenID FROM TalepEdenler WHERE TalepEden = @TalepEden";
                SqlCommand cmd = new SqlCommand(sorgu, baglantiNesnesi.conn);
                cmd.Parameters.AddWithValue("@TalepEden", talepEdenAd);

                baglantiNesnesi.BaglantiAc();
                object sonuc = cmd.ExecuteScalar();

                if (sonuc != null && int.TryParse(sonuc.ToString(), out int id))
                {
                    talepEdenID = id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Talep Eden ID alınırken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglantiNesnesi.BaglantiKapat();
            }

            return talepEdenID;
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            LogEkle("Profil butonuna tıklandı", "Buton", "OfficerTaskspage");
            OfficerProfile profile = new OfficerProfile();
            profile.Show();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            LogEkle("Görevler butonuna tıklandı", "Buton", "OfficerTaskspage");
            this.Close();
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            LogEkle("Raporlar butonuna tıklandı", "Buton", "OfficerTaskspage");
            this.Close();
            OfficerReportsPage officerReportsPage = new OfficerReportsPage();
            officerReportsPage.Show();
        }

        private void cmbDurum_SelectedIndexChanged(object sender, EventArgs e)
        {
            CagrilariFiltrele();
        }

        private void cmbOncelik_SelectedIndexChanged(object sender, EventArgs e)
        {
            CagrilariFiltrele();
        }

        private void cmbKategori_SelectedIndexChanged(object sender, EventArgs e)
        {
            CagrilariFiltrele();
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            LogEkle("Temizle butonuna tıklandı", "Buton", "OfficerTaskspage");
            cmbDurum.SelectedIndex = -1;
            cmbOncelik.SelectedIndex = -1;
            cmbKategori.SelectedIndex = -1;
            txtArama.Text = "Ara...";
            txtArama.ForeColor = Color.Gray;
            CagrilariYukle();
        }
    }
}
