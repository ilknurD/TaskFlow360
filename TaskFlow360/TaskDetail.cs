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
    public partial class TaskDetail : Form
    {
        Connection bgl = new Connection();
        private readonly Logger _logger;
        private int _cagriID;
        private int _talepEdenID;
        public TaskDetail(int cagriID, int talepEdenID)
        {
            InitializeComponent();
            _logger = new Logger();
            _cagriID = cagriID;
            _talepEdenID = talepEdenID;
            btnGeriBildirimKaydet.Click += BtnGeriBildirimKaydet_Click;
            GeriBildirimKontrolleriniOlustur();
        }

        private void GeriBildirimKontrolleriniOlustur()
        {
            if (cmbGeriBildirim.Items.Count == 0)
            {
                cmbGeriBildirim.Items.AddRange(new string[] { "Seçiniz", "Olumlu", "Olumsuz" });
            }
            
            cmbGeriBildirim.DropDownStyle = ComboBoxStyle.DropDownList;

            if (UserInformation.Rol == "Çağrı Merkezi")
            {
                MusteriBildirimi.Visible = true;
                
                bool isTamamlandi = cmbDurum.Text == "Tamamlandı";
                
                foreach (Control control in MusteriBildirimi.Controls)
                {
                    if (control is ComboBox || control is Button)
                    {
                        control.Enabled = isTamamlandi;
                    }
                }

                cmbGeriBildirim.Enabled = isTamamlandi;
                btnGeriBildirimKaydet.Enabled = isTamamlandi;
            }
            else
            {
                MusteriBildirimi.Visible = false;
            }
        }

        private void BtnGeriBildirimKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbGeriBildirim.SelectedItem == null || cmbGeriBildirim.SelectedItem.ToString() == "Seçiniz")
                {
                    MessageBox.Show("Lütfen bir geri bildirim seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bgl.BaglantiAc();

                string kontrolSorgu = "SELECT COUNT(*) FROM CagriGeriBildirim WHERE CagriID = @CagriID";
                SqlCommand kontrolCmd = new SqlCommand(kontrolSorgu, bgl.conn);
                kontrolCmd.Parameters.AddWithValue("@CagriID", _cagriID);
                int kayitSayisi = (int)kontrolCmd.ExecuteScalar();

                string sorgu;
                if (kayitSayisi > 0)
                {
                    _logger.LogEkle("Güncelleme", "ÇağrıGeriBildirim", $"Çağrı ID: {_cagriID} için geri bildirim güncellendi - Yeni Değer: {cmbGeriBildirim.SelectedItem}");
                    sorgu = @"UPDATE CagriGeriBildirim 
                             SET GeriBildirimTipi = @GeriBildirimTipi,
                                 OlusturmaTarihi = GETDATE(),
                                 OlusturanKullaniciID = @KullaniciID
                             WHERE CagriID = @CagriID";
                }
                else
                {
                    _logger.LogEkle("Ekleme", "ÇağrıGeriBildirim", $"Çağrı ID: {_cagriID} için yeni geri bildirim eklendi - Değer: {cmbGeriBildirim.SelectedItem}");
                    sorgu = @"INSERT INTO CagriGeriBildirim (CagriID, GeriBildirimTipi, OlusturanKullaniciID) 
                             VALUES (@CagriID, @GeriBildirimTipi, @KullaniciID)";
                }

                SqlCommand cmd = new SqlCommand(sorgu, bgl.conn);
                cmd.Parameters.AddWithValue("@CagriID", _cagriID);
                cmd.Parameters.AddWithValue("@GeriBildirimTipi", cmbGeriBildirim.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@KullaniciID", Convert.ToInt32(UserInformation.KullaniciID));

                cmd.ExecuteNonQuery();

                MessageBox.Show("Geri bildirim başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                string seciliDeger = cmbGeriBildirim.SelectedItem.ToString();
                YukleCagriBilgileri();
                cmbGeriBildirim.SelectedItem = seciliDeger;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Geri bildirim kaydedilirken hata oluştu: " + ex.Message,
                               "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                bgl.BaglantiKapat();
            }
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

            BtndegisiklikleriKaydet.Visible = UserInformation.Rol == "Çağrı Merkezi";

            bool talepEdenDuzenlenebilir = UserInformation.Rol == "Çağrı Merkezi";
            txtTalepEdenTelefon.ReadOnly = !talepEdenDuzenlenebilir;
            txtTalepEdenEmail.ReadOnly = !talepEdenDuzenlenebilir;
            txtTalepEdenAdres.ReadOnly = !talepEdenDuzenlenebilir;
            txtTalepEdenID.Enabled = false;

            if (!talepEdenDuzenlenebilir)
            {
                AlanıEtkileşimsizYap(txtTalepEdenTelefon);
                AlanıEtkileşimsizYap(txtTalepEdenEmail);
                AlanıEtkileşimsizYap(txtTalepEdenAdres);
            }
        }
        private void AlanıEtkileşimsizYap(TextBox textBox)
        {
            textBox.ReadOnly = true;
            textBox.TabStop = false; 
            textBox.Cursor = Cursors.Default;  
            textBox.GotFocus += TextBox_GotFocus;  

            textBox.BackColor = Color.FromArgb(245, 245, 245);
        }
        private void TextBox_GotFocus(object sender, EventArgs e)
        {
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
                       k.Ad AS AtananKullaniciAd, k.Soyad AS AtananKullaniciSoyad,
                       (SELECT TOP 1 GeriBildirimTipi FROM CagriGeriBildirim WHERE CagriID = c.CagriID ORDER BY OlusturmaTarihi DESC) AS SonGeriBildirim
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

                        if (dr["SonGeriBildirim"] != DBNull.Value)
                        {
                            string geriBildirim = dr["SonGeriBildirim"].ToString();
                            cmbGeriBildirim.SelectedItem = geriBildirim;
                        }
                        else
                        {
                            cmbGeriBildirim.SelectedIndex = 0; 
                        }
                    }
                    else
                    {
                        MessageBox.Show("Çağrı bilgisi bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                GeriBildirimKontrolleriniOlustur();
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
            Label lblDate = new Label();
            lblDate.Text = dateTime;
            lblDate.Location = new Point(20, y);
            lblDate.AutoSize = true;
            lblDate.ForeColor = Color.Gray;
            lblDate.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            pnlgecmisCagrilar.Controls.Add(lblDate);

            y += 20;

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

            Label lblDesc = new Label();
            lblDesc.Text = description;
            lblDesc.Location = new Point(20, y);
            lblDesc.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblDesc.Size = new Size(pnlgecmisCagrilar.Width - 40, 40);
            lblDesc.ForeColor = Color.Black;
            lblDesc.AutoEllipsis = true;
            lblDesc.MaximumSize = new Size(pnlgecmisCagrilar.Width - 40, 0); 
            lblDesc.AutoSize = true;
            pnlgecmisCagrilar.Controls.Add(lblDesc);

            y += lblDesc.Height + 15;

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
                    _logger.LogEkle("Güncelleme", "Çağrı", $"Çağrı ID: {_cagriID} durumu güncellendi - Eski Durum: {mevcutDurum}, Yeni Durum: {yeniDurum}");
                }

                if (aciklama != mevcutAciklama)
                {
                    degisiklikVar = true;
                    _logger.LogEkle("Güncelleme", "Çağrı", $"Çağrı ID: {_cagriID} açıklaması güncellendi");
                }

                if (!degisiklikVar)
                {
                    MessageBox.Show("Herhangi bir değişiklik yapmadınız.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    bgl.BaglantiKapat();
                    return;
                }

                StringBuilder updateSql = new StringBuilder("UPDATE Cagri SET ");
                List<SqlParameter> parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(yeniDurum) && yeniDurum != mevcutDurum)
                {
                    updateSql.Append("Durum = @Durum");
                    parameters.Add(new SqlParameter("@Durum", yeniDurum));

                    // Eğer durum Tamamlandı ise TeslimTarihi ve CevapTarihi'ni güncelleme işlemi
                    if (yeniDurum == "Tamamlandı")
                    {
                        updateSql.Append(", TeslimTarihi = @TeslimTarihi, CevapTarihi = @CevapTarihi");
                        parameters.Add(new SqlParameter("@TeslimTarihi", DateTime.Now));
                        parameters.Add(new SqlParameter("@CevapTarihi", DateTime.Now));
                    }
                }

                if (aciklama != mevcutAciklama)
                {
                    if (parameters.Count > 0) updateSql.Append(", ");
                    updateSql.Append("CagriAciklama = @Aciklama");
                    parameters.Add(new SqlParameter("@Aciklama", aciklama));
                }

                updateSql.Append(" WHERE CagriID = @CagriID");
                parameters.Add(new SqlParameter("@CagriID", _cagriID));

                SqlCommand cmdUpdate = new SqlCommand(updateSql.ToString(), bgl.conn);
                cmdUpdate.Parameters.AddRange(parameters.ToArray());
                cmdUpdate.ExecuteNonQuery();

                if (!string.IsNullOrEmpty(yeniDurum) && yeniDurum != mevcutDurum)
                {
                    string sorgu2 = @"INSERT INTO CagriDurumGuncelleme (CagriID, GuncellemeTarihi, Durum, Aciklama, DegistirenKullaniciID) 
                VALUES (@CagriID, GETDATE(), @Durum, @Aciklama, @KullaniciID)";
                    SqlCommand cmd2 = new SqlCommand(sorgu2, bgl.conn);
                    cmd2.Parameters.AddWithValue("@CagriID", _cagriID);
                    cmd2.Parameters.AddWithValue("@Durum", yeniDurum);
                    cmd2.Parameters.AddWithValue("@Aciklama", aciklama);
                    cmd2.Parameters.AddWithValue("@KullaniciID", Convert.ToInt32(UserInformation.KullaniciID));
                    cmd2.ExecuteNonQuery();
                }

                MessageBox.Show("Çağrı başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                YukleCagriBilgileri();
                GeriBildirimKontrolleriniOlustur();

                if (cmbDurum.Text == "Tamamlandı")
                {
                    YukleCagriBilgileri();
                    GeriBildirimKontrolleriniOlustur();
                }
                else
                {
                    this.Close();
                }
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

        private void BtndegisiklikleriKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                if (UserInformation.Rol != "Çağrı Merkezi")
                {
                    MessageBox.Show("Bu işlem için yetkiniz bulunmamaktadır.", "Yetki Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string mevcutTelefon = "";
                string mevcutEmail = "";
                string mevcutAdres = "";

                bgl.BaglantiAc();

                string sorgula = "SELECT TalepEdenTelefon, TalepEdenEmail, TalepEdenAdres FROM TalepEdenler WHERE TalepEdenID = @TalepEdenID";
                SqlCommand cmdSorgula = new SqlCommand(sorgula, bgl.conn);
                cmdSorgula.Parameters.AddWithValue("@TalepEdenID", _talepEdenID);

                using (SqlDataReader dr = cmdSorgula.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        mevcutTelefon = dr["TalepEdenTelefon"].ToString();
                        mevcutEmail = dr["TalepEdenEmail"].ToString();
                        mevcutAdres = dr["TalepEdenAdres"].ToString();
                    }
                }

                bool degisiklikVar = false;
                StringBuilder logDetaylari = new StringBuilder();

                if (txtTalepEdenTelefon.Text != mevcutTelefon)
                {
                    degisiklikVar = true;
                    logDetaylari.AppendLine($"Telefon: {mevcutTelefon} -> {txtTalepEdenTelefon.Text}");
                }
                if (txtTalepEdenEmail.Text != mevcutEmail)
                {
                    degisiklikVar = true;
                    logDetaylari.AppendLine($"E-posta: {mevcutEmail} -> {txtTalepEdenEmail.Text}");
                }
                if (txtTalepEdenAdres.Text != mevcutAdres)
                {
                    degisiklikVar = true;
                    logDetaylari.AppendLine($"Adres: {mevcutAdres} -> {txtTalepEdenAdres.Text}");
                }

                if (!degisiklikVar)
                {
                    MessageBox.Show("Herhangi bir değişiklik yapmadınız.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string guncelleSorgu = @"UPDATE TalepEdenler 
                                       SET TalepEdenTelefon = @Telefon,
                                           TalepEdenEmail = @Email,
                                           TalepEdenAdres = @Adres
                                       WHERE TalepEdenID = @TalepEdenID";

                SqlCommand cmdGuncelle = new SqlCommand(guncelleSorgu, bgl.conn);
                cmdGuncelle.Parameters.AddWithValue("@Telefon", txtTalepEdenTelefon.Text);
                cmdGuncelle.Parameters.AddWithValue("@Email", txtTalepEdenEmail.Text);
                cmdGuncelle.Parameters.AddWithValue("@Adres", txtTalepEdenAdres.Text);
                cmdGuncelle.Parameters.AddWithValue("@TalepEdenID", _talepEdenID);

                cmdGuncelle.ExecuteNonQuery();

                _logger.LogEkle("Güncelleme", "TalepEdenler", $"Talep Eden ID: {_talepEdenID} için bilgiler güncellendi.\n{logDetaylari}");

                MessageBox.Show("Talep eden bilgileri başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                YukleCagriBilgileri();
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
    }
}
