using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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

        // Diğer tablolarla ilişkiler için
        public string TalepEden { get; set; }
        public Kullanici AtananKullanici { get; set; }
        public Kullanici OlusturanKullanici { get; set; }
        public Kullanici KullaniciID { get; set; }

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

        public static void CagriAciklamaGuncelle(int cagriId, string aciklama, SqlConnection conn)
        {
            string query = "UPDATE Cagri SET CagriAciklama = @aciklama WHERE CagriID = @cagriId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@aciklama", (object)aciklama ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cagriId", cagriId);

                int affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows == 0)
                    throw new Exception("Güncellenecek çağrı bulunamadı.");
            }
        }
        //YAPILAN HER DEĞİŞİKLİK İÇİN CAGRİDURUMGUNCELLEME TABLOSUNA DA EKLEME YAPMAK GEREKİYOR
        public static void CagriDurumGuncelle(int cagriId, string durum, string aciklama, int yoneticiId, SqlConnection conn, int KullaniciId)
        {
            string query = @"
           INSERT INTO CagriDurumGuncelleme (CagriID, GuncellemeTarihi, Durum, Aciklama, DegistirenKullaniciID)
           VALUES (@cagriId, @guncellemeTarihi, @durum, @aciklama, @KullaniciId)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@cagriId", cagriId);
                cmd.Parameters.AddWithValue("@guncellemeTarihi", DateTime.Now);
                cmd.Parameters.AddWithValue("@durum", string.IsNullOrWhiteSpace(durum) ? "Güncellendi" : durum); // Varsayılan durum
                cmd.Parameters.AddWithValue("@aciklama", (object)aciklama ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@KullaniciId", KullaniciId);

                cmd.ExecuteNonQuery();
            }
        }

    }
}