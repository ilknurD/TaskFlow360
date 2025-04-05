using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow360
{
    public class Cagri
    {
        public int CagriID { get; set; }
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