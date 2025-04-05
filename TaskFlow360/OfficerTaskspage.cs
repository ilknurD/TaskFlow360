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

        // Diğer kod parçaları...

        private void CagrilariYukle()
        {
            try
            {
                baglantiNesnesi.BaglantiAc();

                string sorgu = @"SELECT CagriID, Baslik, TalepEden, Durum, OlusturmaTarihi, 
                               TeslimTarihi, AtananKullaniciID, CagriKategori, Oncelik 
                               FROM Cagri ORDER BY OlusturmaTarihi DESC";

                SqlCommand komut = new SqlCommand(sorgu, baglantiNesnesi.conn);
                SqlDataAdapter adapter = new SqlDataAdapter(komut);
                DataTable veriTablosu = new DataTable();
                adapter.Fill(veriTablosu);

                // Mevcut sütunları temizle
                dgvGorevler.Columns.Clear();

                // DataGridView görünüm ayarları
                dgvGorevler.AutoGenerateColumns = false;
                dgvGorevler.BackgroundColor = Color.White;
                dgvGorevler.GridColor = Color.LightGray;
                dgvGorevler.RowTemplate.Height = 40;
                dgvGorevler.DefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Regular);

                // Sütun başlıkları stil
                dgvGorevler.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(100, 149, 237); // Mavi
                dgvGorevler.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvGorevler.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);

                // Sütunlar ekle
                dgvGorevler.Columns.Add("CagriID", "Çağrı No");
                dgvGorevler.Columns["CagriID"].DataPropertyName = "CagriID";
                dgvGorevler.Columns["CagriID"].Width = 80;

                dgvGorevler.Columns.Add("Baslik", "Başlık");
                dgvGorevler.Columns["Baslik"].DataPropertyName = "Baslik";
                dgvGorevler.Columns["Baslik"].Width = 250;

                dgvGorevler.Columns.Add("TalepEden", "Talep Eden");
                dgvGorevler.Columns["TalepEden"].DataPropertyName = "TalepEden";
                dgvGorevler.Columns["TalepEden"].Width = 150;

                dgvGorevler.Columns.Add("CagriKategori", "Kategori");
                dgvGorevler.Columns["CagriKategori"].DataPropertyName = "CagriKategori";
                dgvGorevler.Columns["CagriKategori"].Width = 120;

                dgvGorevler.Columns.Add("Oncelik", "Öncelik");
                dgvGorevler.Columns["Oncelik"].DataPropertyName = "Oncelik";
                dgvGorevler.Columns["Oncelik"].Width = 100;

                dgvGorevler.Columns.Add("Durum", "Durum");
                dgvGorevler.Columns["Durum"].DataPropertyName = "Durum";
                dgvGorevler.Columns["Durum"].Width = 120;

                dgvGorevler.Columns.Add("OlusturmaTarihi", "Oluşturma Tarihi");
                dgvGorevler.Columns["OlusturmaTarihi"].DataPropertyName = "OlusturmaTarihi";
                dgvGorevler.Columns["OlusturmaTarihi"].Width = 150;
                dgvGorevler.Columns["OlusturmaTarihi"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

                dgvGorevler.Columns.Add("TeslimTarihi", "Teslim Tarihi");
                dgvGorevler.Columns["TeslimTarihi"].DataPropertyName = "TeslimTarihi";
                dgvGorevler.Columns["TeslimTarihi"].Width = 150;
                dgvGorevler.Columns["TeslimTarihi"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";

                // Detay butonu ekle
                DataGridViewButtonColumn btnColumn = new DataGridViewButtonColumn();
                btnColumn.Name = "Detay";
                btnColumn.HeaderText = "İşlem";
                btnColumn.Text = "Detay";
                btnColumn.UseColumnTextForButtonValue = true;
                dgvGorevler.Columns.Add(btnColumn);

                // Veri kaynağını ayarla
                dgvGorevler.DataSource = veriTablosu;

                // Durum renklerini ayarla
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
                    Color backColor;

                    switch (durum)
                    {
                        case "Atandı":
                            backColor = Color.FromArgb(230, 230, 250); // Açık mor  //Durumlar burdaki gibi bütün proje içerisinde düzeltilecek
                            break;
                        case "Beklemede":
                            backColor = Color.FromArgb(255, 239, 213); // Açık turuncu
                            break;
                        case "Çözüm Bekliyor":
                            backColor = Color.FromArgb(173, 216, 230); // Açık mavi
                            break;
                        case "Tamamlandı":
                            backColor = Color.FromArgb(144, 238, 144); // Açık yeşil
                            break;
                        case "İptal Edildi":
                            backColor = Color.FromArgb(211, 211, 211); // Açık gri
                            break;
                        default:
                            backColor = Color.White; // Varsayılan renk
                            break;
                    }

                    row.DefaultCellStyle.BackColor = backColor;
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(
                        Math.Max(backColor.R - 30, 0),
                        Math.Max(backColor.G - 30, 0),
                        Math.Max(backColor.B - 30, 0));
                    row.DefaultCellStyle.SelectionForeColor = Color.Black;
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
            this.Close();
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            this.Close();
            OfficerHomepage homepage = new OfficerHomepage();
            homepage.ShowDialog();
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

                    // Renkleri tekrar ayarla
                    SetRenkler();
                }
            }
            else if (dgvGorevler.DataSource is DataTable dt)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Empty;

                // Renkleri tekrar ayarla
                SetRenkler();
            }
        }

        private void dgvGorevler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvGorevler.Columns["Detay"].Index)
            {
                try
                {
                    // Seçilen çağrının ID'sini al
                    int cagriID = Convert.ToInt32(dgvGorevler.Rows[e.RowIndex].Cells["CagriID"].Value);

                    // Çağrı detay formunu aç
                    //CagriDetay detayForm = new CagriDetay(cagriID);
                    //detayForm.ShowDialog();

                    // Detay formunu kapattıktan sonra verileri yenile
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
    }
}
