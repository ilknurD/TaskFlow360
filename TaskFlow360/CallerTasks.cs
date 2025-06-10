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
    public partial class CallerTasks : Form
    {
        Connection baglanti = new Connection();
        private readonly Logger _logger;

        public CallerTasks()
        {
            InitializeComponent();
            _logger = new Logger();
            dgvGorevler.CellClick += new DataGridViewCellEventHandler(dgvGorevler_CellClick);
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Sayfa Geçişi", "CallerTasks", "Anasayfaya geçiş yapıldı");
            CallerHomepage homepage = new CallerHomepage();
            homepage.Show();
            this.Close();
        }

        private void btnProfil_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Sayfa Geçişi", "CallerTasks", "Profil sayfasına geçiş yapıldı");
            CallerProfile asistantProfile = new CallerProfile();
            asistantProfile.Show();
            this.Close();
        }

        private void btnCagriOlustur_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Sayfa Geçişi", "CallerTasks", "Çağrı oluşturma sayfasına geçiş yapıldı");
            CallerTaskCreationPage taskCreationPage = new CallerTaskCreationPage();
            taskCreationPage.Show();
            this.Close();
        }

        private void btnCagriTakip_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Sayfa Geçişi", "CallerTasks", "Çağrı takip sayfasına geçiş yapıldı");
            CallerTasks assistantTasks = new CallerTasks();
            assistantTasks.Show();
            this.Close();
        }

        private void btnRaporlar_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Sayfa Geçişi", "CallerTasks", "Raporlar sayfasına geçiş yapıldı");
            CallerReports callerReports = new CallerReports();
            callerReports.Show();
            this.Close();
        }

        private void AssistantTasks_Load(object sender, EventArgs e)
        {
            try
            {
                txtArama.Text = "Ara...";
                txtArama.ForeColor = Color.Gray;

                dgvGorevler.RowTemplate.Height = 40;
                dgvGorevler.DefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Regular);
                GorevleriGetir();

                if (!IsEventHandlerAttached(txtArama, "TextChanged", "txtArama_TextChanged"))
                {
                    txtArama.TextChanged += new EventHandler(txtArama_TextChanged);
                }

                cmbDurum.Items.AddRange(new string[] { "Tümü", "Atandı", "Beklemede", "Tamamlandı", "İptal Edildi", "Gecikti" });
                cmbOncelik.Items.AddRange(new string[] { "Tümü", "Düşük", "Orta", "Yüksek" });
                cmbKategori.Items.AddRange(new string[] { "Tümü", "Donanım", "Yazılım", "Ağ", "Erişim Talebi", "Mail Problemleri",
                "Veri Yedekleme", "Sistem Arızası", "Kullanıcı İşlemleri", "Genel Talep", "Raporlar"});

                ConfiguredgvGorevler();
                // DataGridView stilleri
                dgvGorevler.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
                dgvGorevler.EnableHeadersVisualStyles = false;

                foreach (DataGridViewColumn column in dgvGorevler.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                dgvGorevler.CellContentClick += dgvGorevler_CellContentClick;
                dgvGorevler.DataBindingComplete += dgvGorevler_DataBindingComplete;
                dgvGorevler.CellFormatting += dgvGorevler_CellFormatting;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veriler yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    dv.RowFilter = string.Format("Baslik LIKE '%{0}%' OR CagriKategori LIKE '%{0}%'", aramaMetni.Replace("'", "''"));
                }
            }
            else if (dgvGorevler.DataSource is DataTable dt)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Empty;
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

        private void GorevleriGetir()
        {
            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();

                string kullaniciID = UserInformation.KullaniciID;

                string query = @"
    SELECT 
        '#' + CAST(CagriID AS NVARCHAR(10)) AS CagriNumarasi,
        Baslik,
        CagriKategori,
        Oncelik,
        Durum
    FROM [dbo].[Cagri]
    WHERE OlusturanKullaniciID = @kullaniciID
    ORDER BY 
        CASE Oncelik
            WHEN 'Yüksek' THEN 1
            WHEN 'Orta' THEN 2
            WHEN 'Düşük' THEN 3
            ELSE 4
        END";

                SqlDataAdapter da = new SqlDataAdapter(query, baglanti.conn);
                da.SelectCommand.Parameters.AddWithValue("@kullaniciID", kullaniciID);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvGorevler.DataSource = dt;

                dgvGorevler.Columns["CagriNumarasi"].HeaderText = "Çağrı No";
                dgvGorevler.Columns["Baslik"].HeaderText = "Başlık";
                dgvGorevler.Columns["CagriKategori"].HeaderText = "Kategori";
                dgvGorevler.Columns["Oncelik"].HeaderText = "Öncelik";
                dgvGorevler.Columns["Durum"].HeaderText = "Durum";

                dgvGorevler.Columns["CagriNumarasi"].DisplayIndex = 0;
                dgvGorevler.Columns["Baslik"].DisplayIndex = 1;
                dgvGorevler.Columns["CagriKategori"].DisplayIndex = 2;
                dgvGorevler.Columns["Oncelik"].DisplayIndex = 3;
                dgvGorevler.Columns["Durum"].DisplayIndex = 4;

                if (!dgvGorevler.Columns.Contains("islemButon"))
                {
                    DataGridViewButtonColumn islemButon = new DataGridViewButtonColumn();
                    islemButon.Name = "islemButon";
                    islemButon.HeaderText = "İşlemler";
                    islemButon.Text = "Düzenle";
                    islemButon.UseColumnTextForButtonValue = true;
                    islemButon.DisplayIndex = 5;
                    dgvGorevler.Columns.Add(islemButon);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                if (baglanti.conn.State == ConnectionState.Open)
                    baglanti.conn.Close();
            }
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Kapatma", "CallerTasks", "Kullanıcı uygulamayı kapattı");
            Application.Exit();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void ConfiguredgvGorevler()
        {
            dgvGorevler.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(126, 87, 194),
                ForeColor = Color.White,
                SelectionBackColor = Color.FromArgb(126, 87, 194),
                SelectionForeColor = Color.White,
                Font = new Font("Century Gothic", 11, FontStyle.Bold)
            };
            dgvGorevler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvGorevler.DefaultCellStyle = new DataGridViewCellStyle
            {
                SelectionBackColor = Color.FromArgb(226, 216, 243), // Soft mor
                SelectionForeColor = Color.Black,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            dgvGorevler.EnableHeadersVisualStyles = false;
            dgvGorevler.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(126, 87, 194),
                ForeColor = Color.White,
                Font = new Font("Century Gothic", 11, FontStyle.Bold)
            };

            if (dgvGorevler.Columns.Contains("islemButon"))
            {
                var buttonStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(126, 87, 194),
                    ForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(146, 107, 214),
                    SelectionForeColor = Color.White
                };
                dgvGorevler.Columns["islemButon"].DefaultCellStyle = buttonStyle;
            }
        }

        private void dgvGorevler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvGorevler.Columns["islemButon"].Index)
                {
                    dgvGorevler.Rows[e.RowIndex].Selected = true;

                    string cagriID = dgvGorevler.Rows[e.RowIndex].Cells["CagriNumarasi"].Value.ToString().Replace("#", "");
                    GorevleriGetir();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İşlem sırasında bir hata oluştu: {ex.Message}", "Hata",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvGorevler_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgvGorevler.Rows[e.RowIndex].Selected)
            {
                if (dgvGorevler.Columns[e.ColumnIndex].Name != "islemButon" &&
                    dgvGorevler.Columns[e.ColumnIndex].Name != "Oncelik" &&
                    dgvGorevler.Columns[e.ColumnIndex].Name != "Durum")
                {
                    e.CellStyle.SelectionBackColor = Color.FromArgb(226, 216, 243);
                    e.CellStyle.SelectionForeColor = Color.Black;
                }
            }

            e.CellStyle.Font = new Font("Century Gothic", 10);

            if (dgvGorevler.Columns[e.ColumnIndex].Name == "islemButon")
            {
                e.CellStyle.BackColor = Color.FromArgb(146, 107, 214);
                e.CellStyle.ForeColor = Color.White;
                return;
            }

            if (dgvGorevler.Columns[e.ColumnIndex].Name == "Oncelik" && e.Value != null)
            {
                string oncelik = e.Value.ToString();
                if (oncelik == "Yüksek")
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 205, 210); // Soft kırmızı
                }
                else if (oncelik == "Orta")
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 224, 178); // Soft turuncu
                }
                else if (oncelik == "Düşük")
                {
                    e.CellStyle.BackColor = Color.FromArgb(200, 230, 201); // Soft yeşil
                }
                e.CellStyle.ForeColor = Color.FromArgb(64, 64, 64);
                return;
            }

            if (dgvGorevler.Columns[e.ColumnIndex].Name == "Durum" && e.Value != null)
            {
                string durum = e.Value.ToString();
                if (durum == "Beklemede")
                {
                    e.CellStyle.BackColor = Color.FromArgb(207, 216, 220); // Soft gri
                }
                else if (durum == "Gecikti")
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 204, 188); // Soft kırmızı-turuncu
                }
                else if (durum == "Tamamlandı")
                {
                    e.CellStyle.BackColor = Color.FromArgb(197, 225, 165); // Soft yeşil
                }
                e.CellStyle.ForeColor = Color.FromArgb(64, 64, 64);
                return;
            }
        }

        private void dgvGorevler_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvGorevler.Rows)
            {
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(226, 216, 243);
                row.DefaultCellStyle.SelectionForeColor = Color.Black;
            }
            dgvGorevler.ClearSelection();
        }
        private int GetTalepEdenIDByCagriID(int cagriID)
        {
            int talepEdenID = 0;

            try
            {
                if (baglanti.conn.State != ConnectionState.Open)
                    baglanti.conn.Open();

                string query = "SELECT TalepEdenID FROM [dbo].[Cagri] WHERE CagriID = @CagriID";

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@CagriID", cagriID);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        talepEdenID = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"TalepEden ID alınırken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (baglanti.conn.State == ConnectionState.Open)
                    baglanti.conn.Close();
            }

            return talepEdenID;
        }

        private void dgvGorevler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == dgvGorevler.Columns["islemButon"].Index)
                {
                    dgvGorevler.Rows[e.RowIndex].Selected = true;
                    string cagriID = dgvGorevler.Rows[e.RowIndex].Cells["CagriNumarasi"].Value.ToString().Replace("#", "");

                    if (int.TryParse(cagriID, out int parsedCagriID))
                    {
                        int talepEdenID = GetTalepEdenIDByCagriID(parsedCagriID);
                        _logger.LogEkle("Çağrı Detayı", "CallerTasks", $"Çağrı detayı görüntülendi - Çağrı ID: {parsedCagriID}");
                        TaskDetail taskDetailForm = new TaskDetail(parsedCagriID, talepEdenID);
                        taskDetailForm.ShowDialog();
                        GorevleriGetir();
                    }
                    else
                    {
                        MessageBox.Show("Geçersiz çağrı ID'si.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogEkle("Hata", "CallerTasks", $"Hata oluştu: {ex.Message}");
                MessageBox.Show($"İşlem sırasında bir hata oluştu: {ex.Message}", "Hata",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string query = @"SELECT 
                    '#' + CAST(c.CagriID AS NVARCHAR(10)) AS CagriNumarasi,
                    c.Baslik,
                    c.CagriKategori,
                    c.Oncelik,
                    c.Durum
                FROM Cagri c
                WHERE c.OlusturanKullaniciID = @KullaniciID";

                if (durum != "Tümü")
                    query += " AND c.Durum = @Durum";

                if (oncelik != "Tümü")
                    query += " AND c.Oncelik = @Oncelik";

                if (kategori != "Tümü")
                    query += " AND c.CagriKategori = @Kategori";

                query += " ORDER BY c.OlusturmaTarihi DESC";

                baglanti.BaglantiAc();

                using (SqlCommand cmd = new SqlCommand(query, baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@KullaniciID", int.Parse(UserInformation.KullaniciID));

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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filtreleme sırasında hata oluştu: " + ex.Message,
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
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
            _logger.LogEkle("Filtre Temizleme", "CallerTasks", "Tüm filtreler temizlendi");
            cmbDurum.SelectedIndex = -1;
            cmbOncelik.SelectedIndex = -1;
            cmbKategori.SelectedIndex = -1;
            txtArama.Text = "Ara...";
            txtArama.ForeColor = Color.Gray;
            GorevleriGetir();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Çıkış", "CallerTasks", "Kullanıcı çıkış yaptı");
            UserInformation.BilgileriTemizle();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}
