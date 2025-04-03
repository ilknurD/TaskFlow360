using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow360
{
     public class Baglanti
    {
        private SqlConnection connection;
        public static string baglantiDizisi = "server=DESKTOP-57F0A7E\\SQLEXPRESS;database=Yurt360;integrated security=True";
       // Logger Logger = new Logger();
        public static bool MailKontrol(string Mail)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(baglantiDizisi))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Kullanici WHERE mail = @Email";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Email", Mail);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Veritabanı bağlantısı hatası: " + ex.Message);
            }
        }

        public void BaglantiAc()
        {
            connection = new SqlConnection("connection_string_buraya");
            connection.Open();
        }

        public void BaglantiKapat()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

    }
}
