using CourseApp.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseApp.Repository
{
    class CoinsRepository
    {
        private readonly SqlConnection _connection;
        public CoinsRepository()
        {
            _connection = DataLink.GetConnection();
        }
        public void InitUserWallet(int userId, int initialBalance = 0)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();

                string query = @"
            IF NOT EXISTS (SELECT 1 FROM UserWallet WHERE UserId = @userId)
            BEGIN
                INSERT INTO UserWallet (UserId, coinBalance, lastLogin)
                VALUES (@userId, @initialBalance, GETDATE())
            END";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@initialBalance", initialBalance);
                    command.ExecuteNonQuery();
                }
            }
        }

        public int GetUserCoins(int userId)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT coinBalance FROM UserWallet WHERE UserId = @userId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    return (int?)command.ExecuteScalar() ?? 0;
                }
            }
        }
        public void UpdateUserCoins(int userId, int newBalance)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "UPDATE UserWallet SET coinBalance = @newBalance WHERE UserId = @userId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@newBalance", newBalance);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddCoins(int userId, int amount)
        {
            int currentCoins = GetUserCoins(userId);
            UpdateUserCoins(userId, currentCoins + amount);
        }


        public bool DeductCoins(int userId, int cost)
        {
            int currentCoins = GetUserCoins(userId);
            if (currentCoins >= cost)
            {
                UpdateUserCoins(userId, currentCoins - cost);
                return true; // Purchase successful
            }
            return false; // Not enough coins
        }


    }
}