using System;
using System.Data.SqlClient;

namespace SarasaviLibrarySystem
{
    public class DatabaseHelper
    {
        private static string connectionString = "Server=umindu-acer\\SQLEXPRESS;Database=SarasaviLibraryDB;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}