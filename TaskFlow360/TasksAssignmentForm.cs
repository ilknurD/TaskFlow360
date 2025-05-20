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
    public partial class TasksAssignmentForm : Form
    {
        private int cagriId;
        private string baslik;
        private string yoneticiId;
        Baglanti baglanti = new Baglanti();

        public TasksAssignmentForm(int cagriId, string baslik, string yoneticiId)
        {
            InitializeComponent();
            this.cagriId = cagriId;
            this.baslik = baslik;
            this.yoneticiId = yoneticiId;
        }

        private void TasksAssignmentForm_Load(object sender, EventArgs e)
        {
            try
            {
                baglanti.BaglantiAc();

                // Cagri sınıfından bilgileri çek
                Cagri seciliCagri = Cagri.CagriGetir(cagriId, baglanti.conn);

                if (seciliCagri != null)
                {
                    txtCagriID.Text = seciliCagri.CagriID.ToString();
                    txtBaslik.Text = seciliCagri.Baslik;
                    txtAciklama.Text = ""; // Boş bırakılır, kullanıcı yeni açıklama girsin

                    txtCagriID.ReadOnly = true;
                    txtBaslik.ReadOnly = true;
                }
                else
                {
                    MessageBox.Show("Çağrı bilgisi bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Çağrı bilgisi çekilirken hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                string aciklama = txtAciklama.Text.Trim();

                if (string.IsNullOrWhiteSpace(aciklama))
                {
                    MessageBox.Show("Lütfen açıklama giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                baglanti.BaglantiAc();

                // Çağrının güncellemesini yap
                Cagri.CagriDurumGuncelle(
                    cagriId,
                    "Görev Atandı",                      // Sabit durum metni
                    aciklama,                            // Kullanıcının girdiği açıklama
                    yoneticiId,
                    baglanti.conn
                );

                MessageBox.Show("Durum güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.BaglantiKapat();
            }
        }

    }
}
