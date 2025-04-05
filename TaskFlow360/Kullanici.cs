using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow360
{
    public class Kullanici
    {
        public int KullaniciID { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        public string Rol { get; set; }
        public string Maas { get; set; }
        public int? DepartmanID { get; set; }
        public int? BolumID { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public DateTime? DogumTar { get; set; }
        public DateTime? IseBaslamaTar { get; set; }
        public string KimlikNo { get; set; }

        // İlişkili nesneler
        public Departman Departman { get; set; }
        public Bolum Bolum { get; set; }
        public PrimMaasHesaplama PrimMaasHesaplama { get; set; }
        public List<Log> Loglar { get; set; }
        public List<Cagri> TalepEttigiCagrilar { get; set; }
        public List<Cagri> AtananCagrilar { get; set; }
        public List<Cagri> OlusturduguCagrilar { get; set; }
        public List<CagriDurumGuncelleme> DegistirdigiCagrilar { get; set; }
        public List<PerformansRaporu> PerformansRaporlari { get; set; }
    }

    public class Departman
    {
        public int DepartmanID { get; set; }
        public string DepartmanAdi { get; set; }

        // İlişkili nesneler
        public List<Kullanici> Kullanicilar { get; set; }
        public List<Bolum> Bolumler { get; set; }
    }

    public class Bolum
    {
        public int BolumID { get; set; }
        public string BolumAdi { get; set; }
        public int? BagliDepartmanID { get; set; }

        // İlişkili nesneler
        public Departman BagliDepartman { get; set; }
        public List<Kullanici> Kullanicilar { get; set; }
    }

    public class PrimMaasHesaplama
    {
        public int HesaplamaID { get; set; }
        public int KullaniciID { get; set; }
        public DateTime HesaplamaTarihi { get; set; }
        public decimal ToplamPrim { get; set; }
        public decimal ToplamMaas { get; set; }

        // İlişkili nesne
        public Kullanici Kullanici { get; set; }
    }

    public class Log
    {
        public int LogID { get; set; }
        public DateTime IslemTarihi { get; set; }
        public int KullaniciID { get; set; }
        public string IslemTipi { get; set; }
        public string TabloAdi { get; set; }
        public string IslemDetaylari { get; set; }

        // İlişkili nesne
        public Kullanici Kullanici { get; set; }
    }

    public class PerformansRaporu
    {
        public int RaporID { get; set; }
        public int KullaniciID { get; set; }
        public DateTime RaporTarihi { get; set; }
        public int ToplamCagriSayisi { get; set; }
        public TimeSpan OrtalamaÇozumSuresi { get; set; }
        public decimal Prim { get; set; }

        // İlişkili nesne
        public Kullanici Kullanici { get; set; }
    }
}