using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow360;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WindowsFormsApp
{
    public class KullaniciGiris
    {
        private Baglanti baglanti;

        public KullaniciGiris(Baglanti baglanti)
        {
            this.baglanti = baglanti;
        }

        public bool GirisDogrula(string kullaniciAdi, string sifre)
        {
            bool dogruMu = false;

            baglanti.BaglantiAc();
            string sorgu = "SELECT COUNT(1) FROM Kullanici WHERE KullaniciAdi = @Email AND Sifre = @Sifre";

            using (SqlCommand komut = new SqlCommand(sorgu))
            {
                komut.Parameters.AddWithValue("@Email", kullaniciAdi);
                komut.Parameters.AddWithValue("@Sifre", sifre);

                int sonuc = Convert.ToInt32(komut.ExecuteScalar());

                if (sonuc == 1)
                {
                    dogruMu = true;
                }
            }

            baglanti.BaglantiKapat();
            return dogruMu;
        }
    }
}

