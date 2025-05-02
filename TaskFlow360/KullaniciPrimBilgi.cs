using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow360
{
    public class KullaniciPrimBilgi
    {
        public int KullaniciID { get; set; }
        public string AdSoyad { get; set; }
        public decimal TemelMaas { get; set; }
        public int ToplamGorev { get; set; }
        public int YuksekOncelikGorev { get; set; }
        public int OrtaOncelikGorev { get; set; }
        public int DusukOncelikGorev { get; set; }
        public decimal YuksekPrim { get; set; }
        public decimal OrtaPrim { get; set; }
        public decimal DusukPrim { get; set; }
        public decimal ToplamPrim { get; set; }
        public decimal ToplamMaas { get; set; }
    }

}
