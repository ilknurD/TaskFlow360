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

namespace TaskFlow360
{
    public partial class OfficerTaskspage : Form
    {
        public OfficerTaskspage()
        {
            InitializeComponent();
        }

        private Baglanti baglantiNesnesi = new Baglanti();

        private void CagrilariYukle()
        {
            try
            {
                baglantiNesnesi.BaglantiAc();

                string sorgu = @"SELECT CagriID, Baslik, TalepEden, Durum, OlusturmaTarihi, 
                               TeslimTarihi, AtananKullaniciID, CagriKategori, Oncelik, HedefSure 
                               FROM Cagri WHERE AtananKullaniciID = @KullaniciID
                               ORDER BY OlusturmaTarihi DESC";

                SqlCommand komut = new SqlCommand(sorgu, baglantiNesnesi.conn);
                komut.Parameters.AddWithValue("@KullaniciID", KullaniciBilgi.KullaniciID); 
                SqlDataAdapter adapter = new SqlDataAdapter(komut);
                DataTable veriTablosu = new DataTable();
                adapter.Fill(veriTablosu);

                dgvGorevler.Columns.Clear();
                dgvGorevler.ScrollBars = ScrollBars.Vertical;

                dgvGorevler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvGorevler.AutoGenerateColumns = false;
                dgvGorevler.BackgroundColor = Color.White;
                dgvGorevler.GridColor = Color.LightGray;    
                dgvGorevler.RowTemplate.Height = 40;
                dgvGorevler.DefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Regular);
                dgvGorevler.ColumnHeadersHeight = 70;
                dgvGorevler.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dgvGorevler.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 149, 237); // Mavi
                dgvGorevler.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvGorevler.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);

                dgvGorevler.Columns.Add("CagriID", "Çağrı No");
                dgvGorevler.Columns["CagriID"].DataPropertyName = "CagriID";
                dgvGorevler.Columns["CagriID"].Width = 60;
                dgvGorevler.Columns["CagriID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvGorevler.Columns["CagriID"].Resizable = DataGridViewTriState.False;
                dgvGorevler.Columns["CagriID"].MinimumWidth = 30;



                dgvGorevler.Columns.Add("Baslik", "Başlık");
                dgvGorevler.Columns["Baslik"].DataPropertyName = "Baslik";
                dgvGorevler.Columns["Baslik"].Width = 150;

                dgvGorevler.Columns.Add("TalepEden", "Talep Eden");
                dgvGorevler.Columns["TalepEden"].DataPropertyName = "TalepEden";
                dgvGorevler.Columns["TalepEden"].Width = 150;

                dgvGorevler.Columns.Add("CagriKategori", "Kategori");
                dgvGorevler.Columns["CagriKategori"].DataPropertyName = "CagriKategori";
                dgvGorevler.Columns["CagriKategori"].Width = 145;

                dgvGorevler.Columns.Add("Oncelik", "Öncelik");
                dgvGorevler.Columns["Oncelik"].DataPropertyName = "Oncelik";
                dgvGorevler.Columns["Oncelik"].Width = 110;

                dgvGorevler.Columns.Add("Durum", "Durum");
                dgvGorevler.Columns["Durum"].DataPropertyName = "Durum";
                dgvGorevler.Columns["Durum"].Width = 120;

                dgvGorevler.Columns.Add("OlusturmaTarihi", "Oluşturma Tarihi");
                dgvGorevler.Columns["OlusturmaTarihi"].DataPropertyName = "OlusturmaTarihi";
                dgvGorevler.Columns["OlusturmaTarihi"].Width = 130;
                dgvGorevler.Columns["OlusturmaTarihi"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

                dgvGorevler.Columns.Add("TeslimTarihi", "Teslim Tarihi");
                dgvGorevler.Columns["TeslimTarihi"].DataPropertyName = "TeslimTarihi";
                dgvGorevler.Columns["TeslimTarihi"].Width = 130;
                dgvGorevler.Columns["TeslimTarihi"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

                dgvGorevler.Columns.Add("HedefSure", "Hedef Süre");
                dgvGorevler.Columns["HedefSure"].DataPropertyName = "HedefSure";
                dgvGorevler.Columns["HedefSure"].Width = 100;

                DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn();
                btnColumn.Name = "Detay";
                btnColumn.HeaderText = "İşlem";
                btnColumn.Text = "Detay";
                btnColumn.UseColumnTextForButtonValue = true;
                dgvGorevler.Columns.Add(btnColumn);

                dgvGorevler.DataSource = veriTablosu;
                SetRenkler();
            }
            catch (Exception ex)
            {
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

        private void btnCikis_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
            this.Close();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
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

            CagrilariYukle();

            if (!IsEventHandlerAttached(txtArama, "TextChanged", "txtArama_TextChanged"))
            {
                txtArama.TextChanged += new EventHandler(txtArama_TextChanged);
            }

            cmbDurum.Items.AddRange(new string[] { "Tümü", "Atandı", "Beklemede", "Tamamlandı", "İptal Edildi", "Gecikti" });
            cmbOncelik.Items.AddRange(new string[] { "Tümü", "Düşük", "Orta", "Yüksek" });
            cmbKategori.Items.AddRange(new string[] { "Tümü", "Donanım", "Yazılım", "Ağ", "Erişim Talebi", "Mail Problemleri",
            "Veri Yedekleme", "Sistem Arızası", "Kullanıcı İşlemleri", "Genel Talep", "Raporlar"});

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

        private void dgvGorevler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvGorevler.Columns["Detay"].Index)
            {
                try
                {
                    int cagriID = Convert.ToInt32(dgvGorevler.Rows[e.RowIndex].Cells["CagriID"].Value);

                    // Çağrı detay formunu aç
                    //CagriDetay detayForm = new CagriDetay(cagriID);
                    //detayForm.ShowDialog();

                    CagrilariYukle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Çağrı detayı açılırken hata oluştu: " + ex.Message,
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            string durum = cmbDurum.SelectedItem?.ToString() ?? "Tümü";
            string oncelik = cmbOncelik.SelectedItem?.ToString() ?? "Tümü";
            string kategori = cmbKategori.SelectedItem?.ToString() ?? "Tümü";

            string query = "SELECT CagriID, Baslik, TalepEden, Durum, OlusturmaTarihi, " +
                           "TeslimTarihi, AtananKullaniciID, CagriKategori, Oncelik, HedefSure " +
                           "FROM Cagri WHERE AtananKullaniciID = @KullaniciID";

            if (durum != "Tümü")
                query += " AND Durum = @Durum";
            if (oncelik != "Tümü")
                query += " AND Oncelik = @Oncelik";
            if (kategori != "Tümü")
                query += " AND CagriKategori = @CagriKategori";

            query += " ORDER BY OlusturmaTarihi DESC";

            try
            {
                baglantiNesnesi.BaglantiAc();
                SqlCommand cmd = new SqlCommand(query, baglantiNesnesi.conn);

                cmd.Parameters.AddWithValue("@KullaniciID", KullaniciBilgi.KullaniciID);

                if (durum != "Tümü")
                    cmd.Parameters.AddWithValue("@Durum", durum);
                if (oncelik != "Tümü")
                    cmd.Parameters.AddWithValue("@Oncelik", oncelik);
                if (kategori != "Tümü")
                    cmd.Parameters.AddWithValue("@CagriKategori", kategori);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvGorevler.DataSource = dt;
                SetRenkler();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filtre uygulanırken hata oluştu: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglantiNesnesi.BaglantiKapat();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            OfficerProfile profile = new OfficerProfile();
            profile.Show();
        }

        private void btnGorevler_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerTaskspage officerTaskspage = new OfficerTaskspage();
            officerTaskspage.Show();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
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
            cmbDurum.SelectedIndex = -1;
            cmbOncelik.SelectedIndex = -1;
            cmbKategori.SelectedIndex = -1;
            txtArama.Text = "Ara...";
            txtArama.ForeColor = Color.Gray;
            CagrilariYukle(); 
        }
    }
}
