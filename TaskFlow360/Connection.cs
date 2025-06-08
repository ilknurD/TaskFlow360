using System;
using System.Data;
using System.Data.SqlClient;

namespace TaskFlow360
{
    public class Connection
    {
        private static string baglantiDizisi = @"Server=DESKTOP-57F0A7E\SQLEXPRESS;Database=TaskFlow360;Integrated Security=True";
        public SqlConnection conn = new SqlConnection(baglantiDizisi);

        public static bool MailKontrol(string Mail)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(baglantiDizisi))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Kullanici WHERE Email = @Email";
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
            try
            {
                if (conn == null)
                    conn = new SqlConnection(baglantiDizisi);

                if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                    conn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Veritabanı bağlantısı açılırken hata: " + ex.Message);
            }
        }

        public void BaglantiKapat()
        {
            try
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Veritabanı bağlantısı kapatılırken hata: " + ex.Message);
            }
        }

        public static SqlConnection BaglantiGetir()
        {
            SqlConnection conn = new SqlConnection(baglantiDizisi);
            if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                conn.Open();
            return conn;
        }
        public bool BaglantiTest()
        {
            try
            {
                BaglantiAc();
                return conn != null && conn.State == ConnectionState.Open;
            }
            catch
            {
                return false;
            }
            finally
            {
                BaglantiKapat();
            }
        }

    }
}
