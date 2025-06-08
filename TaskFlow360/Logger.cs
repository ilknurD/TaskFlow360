using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;

namespace TaskFlow360
{
    public class Logger
    {
        private readonly Baglanti _baglanti;

        public Logger()
        {
            _baglanti = new Baglanti();
        }

        private string GetLocalIPAddress()
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.ToString();
                }
            }
            catch
            {
                return "IP Alınamadı";
            }
        }

        public void LogEkle(string islemTipi, string tabloAdi, string islemDetaylari)
        {
            try
            {
                _baglanti.BaglantiAc();

                string query = @"INSERT INTO Log (IslemTarihi, KullaniciID, IslemTipi, TabloAdi, IslemDetaylari, IPAdresi) 
                               VALUES (@IslemTarihi, @KullaniciID, @IslemTipi, @TabloAdi, @IslemDetaylari, @IPAdresi)";

                using (SqlCommand cmd = new SqlCommand(query, _baglanti.conn))
                {
                    cmd.Parameters.AddWithValue("@IslemTarihi", DateTime.Now);
                    cmd.Parameters.AddWithValue("@KullaniciID", KullaniciBilgi.KullaniciID);
                    cmd.Parameters.AddWithValue("@IslemTipi", islemTipi);
                    cmd.Parameters.AddWithValue("@TabloAdi", tabloAdi);
                    cmd.Parameters.AddWithValue("@IslemDetaylari", islemDetaylari);
                    cmd.Parameters.AddWithValue("@IPAdresi", GetLocalIPAddress());

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Loglama hatası durumunda konsola yazdır
                Console.WriteLine($"Loglama hatası: {ex.Message}");
            }
            finally
            {
                _baglanti.BaglantiKapat();
            }
        }
    }
} 