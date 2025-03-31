using Microsoft.Data.SqlClient;
using System;

namespace Duo.Data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(_connectionString);
            }
            catch (SqlException ex)
            {
                throw new Exception("An error occurred while creating the SQL connection.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating the SQL connection.", ex);
            }
        }
    }
}