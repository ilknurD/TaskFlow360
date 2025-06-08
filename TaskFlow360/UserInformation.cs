using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow360
{
    public static class UserInformation
    {
        public static string KullaniciID { get; set; }
        public static string Rol { get; set; }
        public static string Ad { get; set; }
        public static string Soyad { get; set; }

        public static void BilgileriTemizle()
        {
            KullaniciID = null;
            Rol = null;
            Ad = null;
            Soyad = null;
        }

        public static string TamAd()
        {
            if (!string.IsNullOrEmpty(Ad) && !string.IsNullOrEmpty(Soyad))
                return $"{Ad} {Soyad}";
            else
                return string.Empty;
        }
    }
}
