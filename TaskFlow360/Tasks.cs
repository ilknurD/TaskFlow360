using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TaskFlow360
{
    public partial class Tasks : Form
    {
        private string uyeID;         // Atama yapılacak kişi
        private string managerID;     // Mevcut oturumdaki yönetici

        public Tasks(string uyeID, string managerID)
        {
            InitializeComponent();
            this.uyeID = uyeID;
            this.managerID = managerID;
            dgvGorevler.CellContentClick += dgvGorevler_CellContentClick;
        }

        private void Tasks_Load(object sender, EventArgs e)
        {
            GorevleriGetir();
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
        }

        private void GorevleriGetir()
        {
            try
            {
                string query = @"
                SELECT 
                    CagriID, 
                    Baslik, 
                    Oncelik, 
                    Durum, 
                    TeslimTarihi
                FROM Cagri
                WHERE Durum <> 'Tamamlandı'
                  AND AtananKullaniciID IN (
                      SELECT KullaniciID FROM Kullanici
                      WHERE YoneticiID = @ManagerID OR KullaniciID = @ManagerID
                  )";

                using (SqlConnection conn = Baglanti.BaglantiGetir())
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
                using (SqlConnection conn = Baglanti.BaglantiGetir())
                {
                    foreach (DataGridViewRow row in dgvGorevler.Rows)
                    {
                        if (row.IsNewRow) continue;

                        bool secildi = Convert.ToBoolean(row.Cells["Secildi"].Value ?? false);

                        if (secildi)
                        {
                            int cagriID = Convert.ToInt32(row.Cells["CagriID"].Value);

                            string updateQuery = "UPDATE Cagri SET AtananKullaniciID = @UyeID WHERE CagriID = @CagriID";
                            SqlCommand cmd = new SqlCommand(updateQuery, conn);
                            cmd.Parameters.AddWithValue("@UyeID", uyeID);
                            cmd.Parameters.AddWithValue("@CagriID", cagriID);

                            cmd.ExecuteNonQuery();
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
            this.Text = $"Görev Atama - {uyeAdi}";
        }

        public void SetMemberName(string uyeAdi)
        {
            SetSelectedMemberInfo(uyeAdi);
        }
    }
}
