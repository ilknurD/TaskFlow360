﻿using System;
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
    public partial class AssistantTaskCreationPage : Form
    {
        public AssistantTaskCreationPage()
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

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            AssistantHomepage assistantHomepage = new AssistantHomepage();
            assistantHomepage.Show();
            this.Close();
        }

        private void btnCagriTakip_Click(object sender, EventArgs e)
        {
            AssistantTasks assistantTasks = new AssistantTasks();
            assistantTasks.Show();
            this.Close();
        }

        private void AssistantTaskCreationPage_Load(object sender, EventArgs e)
        {
            EkipYoneticileriniGetir();
            DepartmanlariGetir();
            StatikVerileriYukle();
        }
        private bool GirdilerGecerliMi()
        {
            if (string.IsNullOrWhiteSpace(txtBaslik.Text) ||
                string.IsNullOrWhiteSpace(richTextAciklama.Text) ||
                string.IsNullOrWhiteSpace(txtTalepAdSoyad.Text) ||
                string.IsNullOrWhiteSpace(txtTalepAdres.Text) ||
                string.IsNullOrWhiteSpace(txtTalepTelefon.Text) ||
                string.IsNullOrWhiteSpace(txtTalepMail.Text) ||
                cmbKategori.SelectedIndex == -1 ||
                cmbOncelik.SelectedIndex == -1 ||
                cmbDurum.SelectedIndex == -1 ||
                cmbEkipYoneticisi.SelectedIndex == -1 ||
                cmbDepartman.SelectedIndex == -1 ||
                cmbHedefSure.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurun.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void EkipYoneticileriniGetir()
        {
            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT KullaniciID, Ad + ' ' + Soyad AS AdSoyad FROM Kullanici WHERE Rol = 'Ekip Yöneticisi'", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbEkipYoneticisi.DataSource = dt;
                cmbEkipYoneticisi.DisplayMember = "AdSoyad";
                cmbEkipYoneticisi.ValueMember = "KullaniciID";
            }
        }
        private void DepartmanlariGetir()
        {
            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT DepartmanID, DepartmanAdi FROM Departman", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbDepartman.DataSource = dt;
                cmbDepartman.DisplayMember = "DepartmanAdi";
                cmbDepartman.ValueMember = "DepartmanID";
            }
        }
        private void StatikVerileriYukle()
        {
            cmbKategori.Items.AddRange(new string[] { "Yazılım", "Donanım", "Ağ", "Rapor","Sistem","Diğer" });
            cmbOncelik.Items.AddRange(new string[] { "Düşük", "Orta", "Yüksek" });
            cmbDurum.Items.AddRange(new string[] { "Yeni", "Atandı", "Devam Ediyor", "Beklemede" });
            cmbHedefSure.Items.AddRange(new object[] { 1, 2, 4, 8, 24, 48 });
        }
        private void FormuTemizle()
        {
            txtBaslik.Text = "";
            richTextAciklama.Text = "";
            txtTalepAdSoyad.Text = "";
            txtTalepAdres.Text = "";
            txtTalepTelefon.Text = "";
            txtTalepMail.Text = "";

            cmbKategori.SelectedIndex = -1;
            cmbOncelik.SelectedIndex = -1;
            cmbDurum.SelectedIndex = -1;
            cmbDepartman.SelectedIndex = -1;
            cmbEkipYoneticisi.SelectedIndex = -1;
            cmbHedefSure.SelectedIndex = -1;

            dtpBitisTarihi.Value = DateTime.Now;
        }

        private void btnOlustur_Click(object sender, EventArgs e)
        {
            if (!GirdilerGecerliMi())
                return;

            string kullaniciID = KullaniciBilgi.KullaniciID;

            // Talep Eden bilgileri
            string talepAdSoyad = txtTalepAdSoyad.Text.Trim();
            string talepAdres = txtTalepAdres.Text.Trim();
            string talepTelefon = txtTalepTelefon.Text.Trim();
            string talepEmail = txtTalepMail.Text.Trim();

            // Çağrı bilgileri
            string baslik = txtBaslik.Text.Trim();
            string aciklama = richTextAciklama.Text.Trim();
            string kategori = cmbKategori.SelectedItem?.ToString();
            string oncelik = cmbOncelik.SelectedItem?.ToString();
            string durum = cmbDurum.SelectedItem?.ToString();
            DateTime teslimTarihi = dtpBitisTarihi.Value;
            int hedefSure = Convert.ToInt32(cmbHedefSure.SelectedItem);
            string atananID = cmbEkipYoneticisi.SelectedValue?.ToString();
            int departmanID = Convert.ToInt32(cmbDepartman.SelectedValue);

            using (SqlConnection conn = Baglanti.BaglantiGetir())
            {
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // 1. Talep Eden'i ekle
                    SqlCommand cmdTalep = new SqlCommand(@"
                INSERT INTO TalepEdenler (TalepEden, TalepEdenAdres, TalepEdenTelefon, TalepEdenEmail)
                OUTPUT INSERTED.TalepEdenID
                VALUES (@AdSoyad, @Adres, @Telefon, @Email)", conn, trans);

                    cmdTalep.Parameters.AddWithValue("@AdSoyad", talepAdSoyad);
                    cmdTalep.Parameters.AddWithValue("@Adres", talepAdres);
                    cmdTalep.Parameters.AddWithValue("@Telefon", talepTelefon);
                    cmdTalep.Parameters.AddWithValue("@Email", talepEmail);

                    int talepEdenID = (int)cmdTalep.ExecuteScalar();

                    // 2. Çağrıyı ekle
                    SqlCommand cmdCagri = new SqlCommand(@"
                INSERT INTO Cagri 
                (Baslik, CagriAciklama, CagriKategori, Oncelik, Durum, OlusturmaTarihi, TeslimTarihi, 
                 AtananKullaniciID, OlusturanKullaniciID, HedefSure, TalepEdenID)
                VALUES 
                (@Baslik, @Aciklama, @Kategori, @Oncelik, @Durum, @OlusturmaTarihi, @TeslimTarihi, 
                 @AtananID, @OlusturanID, @HedefSure, @TalepEdenID)", conn, trans);

                    cmdCagri.Parameters.AddWithValue("@Baslik", baslik);
                    cmdCagri.Parameters.AddWithValue("@Aciklama", aciklama);
                    cmdCagri.Parameters.AddWithValue("@Kategori", kategori);
                    cmdCagri.Parameters.AddWithValue("@Oncelik", oncelik);
                    cmdCagri.Parameters.AddWithValue("@Durum", durum);
                    cmdCagri.Parameters.AddWithValue("@OlusturmaTarihi", DateTime.Now);
                    cmdCagri.Parameters.AddWithValue("@TeslimTarihi", teslimTarihi);
                    cmdCagri.Parameters.AddWithValue("@AtananID", atananID);
                    cmdCagri.Parameters.AddWithValue("@OlusturanID", kullaniciID);
                    cmdCagri.Parameters.AddWithValue("@HedefSure", hedefSure);
                    cmdCagri.Parameters.AddWithValue("@TalepEdenID", talepEdenID);

                    cmdCagri.ExecuteNonQuery();

                    trans.Commit();

                    // Şimdi mesaj için isimleri sorgula
                    string olusturanAdSoyad = "";
                    string talepEdenAdSoyad = "";
                    string atananAdSoyad = "";

                    using (SqlCommand cmd = new SqlCommand(@"
                SELECT Ad + ' ' + Soyad FROM Kullanici WHERE KullaniciID = @OlusturanID;
                SELECT TalepEden FROM TalepEdenler WHERE TalepEdenID = @TalepEdenID;
                SELECT Ad + ' ' + Soyad FROM Kullanici WHERE KullaniciID = @AtananID;
            ", conn))
                    {
                        cmd.Parameters.AddWithValue("@OlusturanID", kullaniciID);
                        cmd.Parameters.AddWithValue("@TalepEdenID", talepEdenID);
                        cmd.Parameters.AddWithValue("@AtananID", atananID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                olusturanAdSoyad = reader.GetString(0);

                            if (reader.NextResult() && reader.Read())
                                talepEdenAdSoyad = reader.GetString(0);

                            if (reader.NextResult() && reader.Read())
                                atananAdSoyad = reader.GetString(0);
                        }
                    }

                    FormuTemizle();

                    string mesaj = $"{olusturanAdSoyad} tarafından\n" +
                    $"{talepEdenAdSoyad} adlı kişi adına\n" +
                    $"{atananAdSoyad} e yeni görev atanmıştır.";
                    MessageBox.Show(mesaj, "Görev Atama Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                }
            }
        }

    }
}
