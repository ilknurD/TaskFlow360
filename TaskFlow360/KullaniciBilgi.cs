using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow360
{
    // Kullanıcı bilgilerini saklamak için static sınıf
    public static class KullaniciBilgi
    {
        public static string KullaniciID { get; set; }
        public static string Rol { get; set; }
        public static string Ad { get; set; }
        public static string Soyad { get; set; }

        // Statik sınıfı genişletmek için kullanıcı bilgilerini temizleme metodu
        public static void BilgileriTemizle()
        {
            KullaniciID = null;
            Rol = null;
            Ad = null;
            Soyad = null;
        }

        // Yardımcı metot - Ad Soyad birleşik gösterimi
        public static string TamAd()
        {
            if (!string.IsNullOrEmpty(Ad) && !string.IsNullOrEmpty(Soyad))
                return $"{Ad} {Soyad}";
            else
                return string.Empty;
        }
    }
}
