using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow360
{
    public class Gorev
    {
        public string GorevAdi { get; set; }
        public string IlgiliKisi { get; set; }
        public DateTime TeslimTarihi { get; set; }
        public string Oncelik { get; set; }
        public string Durum { get; set; } // Örneğin: "Tamamlandı", "Devam Ediyor", "Gecikti"
    }

}
