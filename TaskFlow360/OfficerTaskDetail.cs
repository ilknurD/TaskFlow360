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
        private int _cagriID;
        private int _talepEdenID;
        public OfficerTaskDetail(int cagriID, int talepEdenID)
        {
            InitializeComponent();
            _cagriID = cagriID;
            _talepEdenID = talepEdenID;
        }

        private void OfficerTaskDetail_Load(object sender, EventArgs e)
        {
            YukleCagriBilgileri();
            YukleTalepEdeninGecmisCagrilari();
            Duzenlenebilirler();
        }

        private void Duzenlenebilirler()
        {
            txtCagriID.ReadOnly = true;
            AlanıEtkileşimsizYap(txtCagriID);
            txtBaslik.ReadOnly = true;
            AlanıEtkileşimsizYap(txtBaslik);
            txtTalepEden.ReadOnly = true;
            AlanıEtkileşimsizYap(txtTalepEden);
            txtOlusturmaTarihi.ReadOnly = true;
            AlanıEtkileşimsizYap(txtOlusturmaTarihi);
            cmbOncelik.Enabled = false;
            txtAciklama.ReadOnly = false;
            cmbDurum.Enabled = true;
            cmbKategori.Enabled = false;
            txtAtanan.ReadOnly = true;
            AlanıEtkileşimsizYap(txtAtanan);
            txtTeslimTarihi.ReadOnly = true;
            AlanıEtkileşimsizYap(txtTeslimTarihi);
            txtHedefSure.ReadOnly = true;
            AlanıEtkileşimsizYap(txtHedefSure);
        }
        private void AlanıEtkileşimsizYap(TextBox textBox)
        {
            textBox.ReadOnly = true;
            textBox.TabStop = false;  // Tab ile üzerine gelinmesini engeller
            textBox.Cursor = Cursors.Default;  // Metin imleci yerine normal fare imleci gösterir
            textBox.GotFocus += TextBox_GotFocus;  // Odak kazandığında odağı kaybetmesini sağlar

            textBox.BackColor = Color.FromArgb(245, 245, 245);
        }
        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            // Odağı başka bir kontrole ver (örneğin form)
            this.ActiveControl = null;
        }

        private void YukleCagriBilgileri()
        {
            try
            {
                bgl.BaglantiAc();

                string sorgu = @"SELECT c.CagriID, c.Baslik, c.CagriAciklama, c.Durum, c.OlusturmaTarihi, 
                       c.TeslimTarihi, c.CagriKategori, c.Oncelik, c.HedefSure, 
                       te.TalepEdenID, te.TalepEden, te.TalepEdenAdres, te.TalepEdenTelefon, te.TalepEdenEmail,
                       k.Ad AS AtananKullaniciAd, k.Soyad AS AtananKullaniciSoyad
                       FROM Cagri c
                       LEFT JOIN TalepEdenler te ON c.TalepEdenID = te.TalepEdenID
                       LEFT JOIN Kullanici k ON c.AtananKullaniciID = k.KullaniciID
                       WHERE c.CagriID = @CagriID";

                SqlCommand cmd = new SqlCommand(sorgu, bgl.conn);
                cmd.Parameters.AddWithValue("@CagriID", _cagriID);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        txtCagriID.Text = dr["CagriID"].ToString();
                        txtBaslik.Text = dr["Baslik"].ToString();
                        txtAciklama.Text = dr["CagriAciklama"].ToString();
                        cmbDurum.Text = dr["Durum"].ToString();
                        txtOlusturmaTarihi.Text = Convert.ToDateTime(dr["OlusturmaTarihi"]).ToString("dd.MM.yyyy HH:mm");
                        txtTalepEden.Text = dr["TalepEden"].ToString();
                        if (dr["AtananKullaniciAd"] != DBNull.Value && dr["AtananKullaniciSoyad"] != DBNull.Value)
                            txtAtanan.Text = dr["AtananKullaniciAd"].ToString() + " " + dr["AtananKullaniciSoyad"].ToString();
                        else
                            txtAtanan.Text = "Atanmadı";


                        if (dr["TeslimTarihi"] != DBNull.Value)
                            txtTeslimTarihi.Text = Convert.ToDateTime(dr["TeslimTarihi"]).ToString("dd.MM.yyyy HH:mm");
                        else
                            txtTeslimTarihi.Text = "Henüz teslim edilmedi";

                        cmbKategori.Text = dr["CagriKategori"].ToString();
                        cmbOncelik.Text = dr["Oncelik"].ToString();
                        txtHedefSure.Text = dr["HedefSure"].ToString();

                        // TalepEdenID değerini sınıf değişkenine kaydet
                        if (dr["TalepEdenID"] != DBNull.Value)
                        {
                            _talepEdenID = Convert.ToInt32(dr["TalepEdenID"]);
                        }
                        else
                        {
                            _talepEdenID = 0;
                        }

                        lblTalepEdenID.Text = dr["TalepEdenID"].ToString();
                        lblTalepEden.Text = dr["TalepEden"].ToString();
                        lblTalepEdenTelefon.Text = dr["TalepEdenTelefon"].ToString();
                        lblTalepEdenEmail.Text = dr["TalepEdenEmail"].ToString();

                        txtTalepEdenID.Text = dr["TalepEdenID"].ToString();
                        txtTalepEdenTelefon.Text = dr["TalepEdenTelefon"].ToString();
                        txtTalepEdenEmail.Text = dr["TalepEdenEmail"].ToString();
                        txtTalepEdenAdres.Text = dr["TalepEdenAdres"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Çağrı bilgisi bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Çağrı bilgileri yüklenirken hata oluştu: " + ex.Message,
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                bgl.BaglantiKapat();
            }
        }


        private void YukleTalepEdeninGecmisCagrilari()
        {
            try
            {
                if (_talepEdenID <= 0)
                {
                    MessageBox.Show("Talep eden bilgisi bulunamadı. Geçmiş çağrılar yüklenemeyecek.",
                                   "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                pnlgecmisCagrilar.Controls.Clear();

                Label lblLoading = new Label();
                lblLoading.Text = "Çağrılar yükleniyor...";
                lblLoading.Location = new Point(pnlgecmisCagrilar.Width / 2 - 60, pnlgecmisCagrilar.Height / 2);
                lblLoading.AutoSize = true;
                pnlgecmisCagrilar.Controls.Add(lblLoading);

                bgl.BaglantiAc();

                int y = 20;

                string sorgu = @"SELECT CagriID, Baslik, Durum, OlusturmaTarihi, TeslimTarihi, CagriAciklama
                   FROM Cagri 
                   WHERE TalepEdenID = @TalepEdenID AND CagriID != @CagriID
                   ORDER BY OlusturmaTarihi DESC";

                SqlCommand cmd = new SqlCommand(sorgu, bgl.conn);
                cmd.Parameters.AddWithValue("@TalepEdenID", _talepEdenID);
                cmd.Parameters.AddWithValue("@CagriID", _cagriID);

                SqlDataReader dr = cmd.ExecuteReader();
                bool hasRecords = false;

                while (dr.Read())
                {
                    hasRecords = true;

                    string tarihSaat = Convert.ToDateTime(dr["OlusturmaTarihi"]).ToString("dd.MM.yyyy HH:mm");
                    string baslik = dr["Baslik"].ToString();
                    string aciklama = dr["CagriAciklama"].ToString();
                    string durum = dr["Durum"].ToString();

                    AddGecmisCagriItem(tarihSaat, baslik, aciklama, durum, ref y);
                }

                dr.Close();

                pnlgecmisCagrilar.Controls.Remove(lblLoading);

                if (!hasRecords)
                {
                    Label lblNoRecords = new Label();
                    lblNoRecords.Text = "Bu talep eden için başka çağrı bulunmamaktadır.";
                    lblNoRecords.Location = new Point(20, 20);
                    lblNoRecords.AutoSize = true;
                    lblNoRecords.ForeColor = Color.Gray;
                    lblNoRecords.Font = new Font("Segoe UI", 9, FontStyle.Italic);
                    pnlgecmisCagrilar.Controls.Add(lblNoRecords);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Geçmiş çağrılar yüklenirken hata oluştu: " + ex.Message,
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                bgl.BaglantiKapat();
            }
        }

        private void AddGecmisCagriItem(string dateTime, string title, string description, string status, ref int y)
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

            // Başlık ve Durum
            Label lblTitle = new Label();
            lblTitle.Text = title + " - " + status;
            lblTitle.Location = new Point(20, y);
            lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // Durum rengini ayarla
            Color statusColor;
            switch (status)
            {
                case "Atandı":
                    statusColor = Color.MediumSlateBlue;
                    break;
                case "Beklemede":
                    statusColor = Color.OrangeRed;
                    break;
                case "Tamamlandı":
                    statusColor = Color.SeaGreen;
                    break;
                case "İptal Edildi":
                    statusColor = Color.IndianRed;
                    break;
                case "Gecikti":
                    statusColor = Color.Crimson;
                    break;
                default:
                    statusColor = Color.Black;
                    break;
            }

            lblTitle.ForeColor = statusColor;
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

            // Ayırıcı çizgi
            Panel separator = new Panel();
            separator.BackColor = Color.LightGray;
            separator.Height = 1;
            separator.Width = pnlgecmisCagrilar.Width - 40;
            separator.Location = new Point(20, y);
            pnlgecmisCagrilar.Controls.Add(separator);

            y += 10;
        }

        private void btnDurumGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                string yeniDurum = cmbDurum.SelectedItem?.ToString();
                string aciklama = txtAciklama.Text.Trim();
                bool degisiklikVar = false;

                string mevcutDurum = "";
                string mevcutAciklama = "";

                bgl.BaglantiAc();

                // Mevcut değerleri oku
                string sorgula = "SELECT Durum, CagriAciklama FROM Cagri WHERE CagriID = @CagriID";
                SqlCommand cmdSorgula = new SqlCommand(sorgula, bgl.conn);
                cmdSorgula.Parameters.AddWithValue("@CagriID", _cagriID);

                using (SqlDataReader dr = cmdSorgula.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        mevcutDurum = dr["Durum"].ToString();
                        mevcutAciklama = dr["CagriAciklama"].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(yeniDurum) && yeniDurum != mevcutDurum)
                {
                    degisiklikVar = true;
                }

                if (aciklama != mevcutAciklama)
                {
                    degisiklikVar = true;
                }

                if (!degisiklikVar)
                {
                    MessageBox.Show("Herhangi bir değişiklik yapmadınız.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    bgl.BaglantiKapat();
                    return;
                }

                // SQL komutunu oluştur
                StringBuilder updateSql = new StringBuilder("UPDATE Cagri SET ");
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(yeniDurum) && yeniDurum != mevcutDurum)
                {
                    updateSql.Append("Durum = @Durum");
                    parameters.Add(new SqlParameter("@Durum", yeniDurum));
                }

                if (aciklama != mevcutAciklama)
                {
                    if (parameters.Count > 0) updateSql.Append(", ");
                    updateSql.Append("CagriAciklama = @Aciklama");
                    parameters.Add(new SqlParameter("@Aciklama", aciklama));
                }

                updateSql.Append(" WHERE CagriID = @CagriID");
                parameters.Add(new SqlParameter("@CagriID", _cagriID));

                // Güncelleme sorgusunu çalıştır
                SqlCommand cmdUpdate = new SqlCommand(updateSql.ToString(), bgl.conn);
                cmdUpdate.Parameters.AddRange(parameters.ToArray());
                cmdUpdate.ExecuteNonQuery();

                // Durumda değişiklik varsa, durum güncellemesini kaydet
                if (!string.IsNullOrEmpty(yeniDurum) && yeniDurum != mevcutDurum)
                {
                    string sorgu2 = @"INSERT INTO CagriDurumGuncelleme (CagriID, GuncellemeTarihi, Durum, Aciklama, DegistirenKullaniciID) 
                VALUES (@CagriID, GETDATE(), @Durum, @Aciklama, @KullaniciID)";
                    SqlCommand cmd2 = new SqlCommand(sorgu2, bgl.conn);
                    cmd2.Parameters.AddWithValue("@CagriID", _cagriID);
                    cmd2.Parameters.AddWithValue("@Durum", yeniDurum);
                    cmd2.Parameters.AddWithValue("@Aciklama", aciklama);
                    cmd2.Parameters.AddWithValue("@KullaniciID", Convert.ToInt32(KullaniciBilgi.KullaniciID));
                    cmd2.ExecuteNonQuery();
                }

                MessageBox.Show("Çağrı başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                YukleCagriBilgileri();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme sırasında hata oluştu: " + ex.Message,
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                bgl.BaglantiKapat();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btnVagzec_Click(object sender, EventArgs e)
        {  
            DialogResult sonuc = MessageBox.Show("Değişiklikleri iptal etmek istiyor musunuz?","Vazgeç", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (sonuc == DialogResult.Yes)
            {
                YukleCagriBilgileri();
                this.Close();
            }
        }
    }
}
