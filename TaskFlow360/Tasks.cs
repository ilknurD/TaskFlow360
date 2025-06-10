using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class Tasks : Form
    {
        private string uyeID;    
        private string managerID;  
        private readonly Logger _logger;

        public Tasks(string uyeID, string managerID)
        {
            InitializeComponent();
            this.uyeID = uyeID;
            this.managerID = managerID;
            _logger = new Logger();
            dgvGorevler.CellContentClick += dgvGorevler_CellContentClick;
        }

        private void Tasks_Load(object sender, EventArgs e)
        {
            GorevleriGetir();
            foreach (DataGridViewColumn column in dgvGorevler.Columns) 
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void Sutunlar()
        {
            if (dgvGorevler.Columns.Count > 0)
                return;

            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn
            {
                Name = "Secildi",
                HeaderText = "",
                Width = 30
            };
            dgvGorevler.Columns.Add(chk);

            dgvGorevler.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CagriID",
                HeaderText = "Çağrı ID",
                DataPropertyName = "CagriID",
                ReadOnly = true
            });

            dgvGorevler.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Baslik",
                HeaderText = "Başlık",
                DataPropertyName = "Baslik"
            });
            
            dgvGorevler.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AtananKisi",
                HeaderText = "Atanan Kişi",
                DataPropertyName = "AtananKisi"
            });

            dgvGorevler.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Oncelik",
                HeaderText = "Öncelik",
                DataPropertyName = "Oncelik"
            });

            dgvGorevler.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Durum",
                HeaderText = "Durum",
                DataPropertyName = "Durum"
            });

            dgvGorevler.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TeslimTarihi",
                HeaderText = "Teslim Tarihi",
                DataPropertyName = "TeslimTarihi"
            });

            dgvGorevler.AutoGenerateColumns = false;
            dgvGorevler.Font = new Font("Century Gothic", 10);
            dgvGorevler.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 11, FontStyle.Bold);
            dgvGorevler.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(126, 87, 194); // Mor
            dgvGorevler.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGorevler.EnableHeadersVisualStyles = false;

            dgvGorevler.DefaultCellStyle.BackColor = Color.White;
            dgvGorevler.DefaultCellStyle.ForeColor = Color.Black;
            dgvGorevler.DefaultCellStyle.SelectionBackColor = Color.FromArgb(179, 157, 219); // Açık mor
            dgvGorevler.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvGorevler.RowHeadersVisible = false;
            dgvGorevler.BorderStyle = BorderStyle.None;
            dgvGorevler.GridColor = Color.FromArgb(230, 230, 250); // Morumsu grid çizgisi

            dgvGorevler.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 240, 255); // Hafif mor ton

            foreach (DataGridViewColumn col in dgvGorevler.Columns)
            {
                col.ReadOnly = col.Name != "Secildi"; 
            }
        }

        private void GorevleriGetir()
        {
            dgvGorevler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGorevler.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvGorevler.ScrollBars = ScrollBars.Both;
            try
            {
                string query = @"
                SELECT 
                    C.CagriID,
                    C.Baslik,
                    C.Oncelik,
                    C.Durum,
                    C.TeslimTarihi,
                    K.Ad + ' ' + K.Soyad AS AtananKisi
                FROM Cagri C
                INNER JOIN Kullanici K ON C.AtananKullaniciID = K.KullaniciID
                WHERE C.Durum <> 'Tamamlandı'
                  AND C.AtananKullaniciID IN (
                      SELECT KullaniciID FROM Kullanici
                      WHERE YoneticiID = @ManagerID OR KullaniciID = @ManagerID
                  )";
    

                using (SqlConnection conn = Connection.BaglantiGetir())
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ManagerID", managerID);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    Sutunlar();
                    dgvGorevler.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Görevler getirilirken hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvGorevler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvGorevler.Columns[e.ColumnIndex].Name == "Secildi")
            {
                foreach (DataGridViewRow row in dgvGorevler.Rows)
                {
                    if (row.Index != e.RowIndex)
                    {
                        row.Cells["Secildi"].Value = false;
                    }
                }

                bool currentValue = Convert.ToBoolean(dgvGorevler.Rows[e.RowIndex].Cells["Secildi"].Value ?? false);
                dgvGorevler.Rows[e.RowIndex].Cells["Secildi"].Value = !currentValue;
            }
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            string aranan = txtAra.Text.Trim().ToLower();

            foreach (DataGridViewRow row in dgvGorevler.Rows)
            {
                if (row.IsNewRow) continue;

                string baslik = row.Cells["Baslik"].Value?.ToString().ToLower();
                row.Visible = string.IsNullOrEmpty(aranan) || (baslik != null && baslik.Contains(aranan));
            }
        }

        private void btnSecilenleriAta_Click(object sender, EventArgs e)
        {
            int sayac = 0;

            try
            {
                using (SqlConnection conn = Connection.BaglantiGetir())
                {
                    string uyeAdiSoyadi = "";
                    string uyeQuery = "SELECT Ad + ' ' + Soyad FROM Kullanici WHERE KullaniciID = @UyeID";
                    using (SqlCommand cmdUye = new SqlCommand(uyeQuery, conn))
                    {
                        cmdUye.Parameters.AddWithValue("@UyeID", uyeID);
                        object result = cmdUye.ExecuteScalar();
                        uyeAdiSoyadi = result != null ? result.ToString() : "Bilinmeyen Kullanıcı";
                    }

                    foreach (DataGridViewRow row in dgvGorevler.Rows)
                    {
                        if (row.IsNewRow) continue;

                        bool secildi = Convert.ToBoolean(row.Cells["Secildi"].Value ?? false);

                        if (secildi)
                        {
                            int cagriID = Convert.ToInt32(row.Cells["CagriID"].Value);

                            string updateQuery = "UPDATE Cagri SET AtananKullaniciID = @UyeID WHERE CagriID = @CagriID";
                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@UyeID", uyeID);
                                cmd.Parameters.AddWithValue("@CagriID", cagriID);
                                cmd.ExecuteNonQuery();
                            }

                            _logger.LogEkle("Güncelleme", "Cagri", $"Görev atandı - ÇağrıID: {cagriID}, Atanan: {uyeAdiSoyadi}, Yöneticisi: {managerID}");

                            Call.CagriDurumGuncelle(
                                cagriID,
                                "Görev Atandı",
                                $"Görev {uyeAdiSoyadi} kullanıcısına atandı.",
                                Convert.ToInt32(managerID), 
                                conn,
                                Convert.ToInt32(managerID)
                            );

                            sayac++;
                        }
                    }
                }

                if (sayac > 0)
                {
                    MessageBox.Show($"{sayac} görev başarıyla atandı.", "Bilgi",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Lütfen atanacak bir görev seçin.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Görev atama sırasında hata oluştu: " + ex.Message, "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();

        }


        private void btnIptal_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            _logger.LogEkle("Çıkış", "Form", "Görevler formundan çıkış yapıldı.");
            this.Hide();
            ManagerTasks tasks = new ManagerTasks();
            tasks.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void SetSelectedMemberInfo(string uyeAdi)
        {
            _logger.LogEkle("Kullanıcı Bilgisi", "Form", $"Seçilen kullanıcı: {uyeAdi}");
            lblSelectedMember.Text = $"Seçili Kişi: {uyeAdi}";
        }

        public void SetMemberName(string uyeAdi)
        {
            SetSelectedMemberInfo(uyeAdi);
        }
    }
}
