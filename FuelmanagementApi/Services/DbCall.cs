using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace FuelmanagementApi.Services
{
    public class DbCall
    {
        private readonly string _connectionString;

        public DbCall(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Dictionary<string, object>> CallStoredProcedure(
            string param1, 
            string param2, 
            string param3, 
            string param4,
            string search,
            string operation, 
            string screen)
        {
            if (string.IsNullOrWhiteSpace(screen))
                throw new ArgumentException("Stored procedure name (screen) cannot be null or empty.", nameof(screen));

            var resultList = new List<Dictionary<string, object>>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                //using (var cmdSetCollation = new MySqlCommand("SET collation_connection = utf8mb4_unicode_ci;", conn))
                //{
                //    cmdSetCollation.ExecuteNonQuery();
                //}

                using (MySqlCommand cmd = new MySqlCommand(screen, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@param1", param1);
                    cmd.Parameters.AddWithValue("@param2", param2);
                    cmd.Parameters.AddWithValue("@param3", param3);
                    cmd.Parameters.AddWithValue("@param4", param4);
                    cmd.Parameters.AddWithValue("@operation", operation);
                    cmd.Parameters.AddWithValue("@search", search);
                    //cmd.CommandText = "SET collation_connection = 'utf8mb4_general_ci'";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            resultList.Add(row);
                        }
                    }
                }

                return resultList;
            }
        }
    }
}
