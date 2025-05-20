using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow360
{
    public class Cagri
    {
        Baglanti baglanti = new Baglanti();
        public int CagriID { get; set; }
        public int yoneticiId { get; set; }
        public string CagriAciklama { get; set; }
        public string CagriKategori { get; set; }
        public string Oncelik { get; set; }
        public string Durum { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? TeslimTarihi { get; set; }
        public DateTime? CevapTarihi { get; set; }
        public int TalepEdenID { get; set; }
        public int AtananKullaniciID { get; set; }
        public int OlusturanKullaniciID { get; set; }
        public int? HedefSure { get; set; }
        public string Baslik { get; set; }

        // Diğer tablolarla ilişkiler için property'ler
        public string TalepEden { get; set; }
        public Kullanici AtananKullanici { get; set; }
        public Kullanici OlusturanKullanici { get; set; }
        public List<CagriDurumGuncelleme> DurumGuncellemeleri { get; set; }

        public static Cagri CagriGetir(int cagriId, SqlConnection conn)
        {
            Cagri cagri = null;

            string sorgu = "SELECT CagriID, Baslik, CagriAciklama FROM Cagri WHERE CagriID = @id";
            SqlCommand komut = new SqlCommand(sorgu, conn);
            komut.Parameters.AddWithValue("@id", cagriId);

            SqlDataReader dr = komut.ExecuteReader();
            if (dr.Read())
            {
                cagri = new Cagri()
                {
                    CagriID = Convert.ToInt32(dr["CagriID"]),
                    Baslik = dr["Baslik"].ToString(),
                    CagriAciklama = dr["CagriAciklama"].ToString()
                };
            }
            dr.Close();

            return cagri;
        }

        public static void CagriDurumGuncelle(int cagriId, string yeniDurum, string aciklama, int degistirenKullaniciId, SqlConnection conn)
        {
            string sorgu = @"
        INSERT INTO CagriDurumGuncelleme (CagriID, GuncellemeTarihi, Durum, Aciklama, DegistirenKullaniciID)
        VALUES (@cagriId, @tarih, @durum, @aciklama, @kullaniciId)";

            using (SqlCommand komut = new SqlCommand(sorgu, conn))
            {
                komut.Parameters.AddWithValue("@cagriId", cagriId);
                komut.Parameters.AddWithValue("@tarih", DateTime.Now);
                komut.Parameters.AddWithValue("@durum", yeniDurum);
                komut.Parameters.AddWithValue("@aciklama", aciklama);
                komut.Parameters.AddWithValue("@kullaniciId", degistirenKullaniciId);

                komut.ExecuteNonQuery();
            }
        }

    }

    public class CagriDurumGuncelleme
    {
        public int GuncellemeID { get; set; }
        public string Cagri { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
        public string Durum { get; set; }
        public string Aciklama { get; set; }
        public int DegistirenKullaniciID { get; set; }

        // İlişkili nesne
        public Kullanici DegistirenKullanici { get; set; }

    }

}