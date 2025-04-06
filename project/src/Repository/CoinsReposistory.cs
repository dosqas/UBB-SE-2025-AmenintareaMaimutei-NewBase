using System;
using Microsoft.Data.SqlClient;
using CourseApp.Data;

namespace CourseApp.Repository
{
    public class CoinsRepository
    {
        private readonly SqlConnection connection;

        private const int DefaultInitialCoinBalance = 0;

        public CoinsRepository()
        {
            connection = DataLink.GetConnection();
        }

        public void InitializeUserWalletIfNotExists(int userId, int initialCoinBalance = DefaultInitialCoinBalance)
        {
            using (SqlConnection databaseConnection = DataLink.GetConnection())
            {
                databaseConnection.Open();

                string insertWalletIfNotExistsQuery = @"
                    IF NOT EXISTS (SELECT 1 FROM UserWallet WHERE UserId = @userId)
                    BEGIN
                        INSERT INTO UserWallet (UserId, coinBalance, lastLogin)
                        VALUES (@userId, @initialCoinBalance, GETDATE())
                    END";

                using (SqlCommand sqlCommand = new SqlCommand(insertWalletIfNotExistsQuery, databaseConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@userId", userId);
                    sqlCommand.Parameters.AddWithValue("@initialCoinBalance", initialCoinBalance);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public int GetUserCoinBalance(int userId)
        {
            using (SqlConnection databaseConnection = DataLink.GetConnection())
            {
                databaseConnection.Open();
                string selectCoinBalanceQuery = "SELECT coinBalance FROM UserWallet WHERE UserId = @userId";
                using (SqlCommand sqlCommand = new SqlCommand(selectCoinBalanceQuery, databaseConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@userId", userId);
                    return (int?)sqlCommand.ExecuteScalar() ?? DefaultInitialCoinBalance;
                }
            }
        }
        public void SetUserCoinBalance(int userId, int updatedCoinBalance)
        {
            using (SqlConnection databaseConnection = DataLink.GetConnection())
            {
                databaseConnection.Open();

                string upsertWalletQuery = @"
                    IF EXISTS (SELECT 1 FROM UserWallet WHERE UserId = @userId)
                        BEGIN
                            UPDATE UserWallet SET coinBalance = @updatedCoinBalance WHERE UserId = @userId
                        END
                    ELSE
                        BEGIN
                            INSERT INTO UserWallet (UserId, coinBalance, lastLogin)
                            VALUES (@userId, @updatedCoinBalance, GETDATE())
                        END";

                using (SqlCommand sqlCommand = new SqlCommand(upsertWalletQuery, databaseConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@userId", userId);
                    sqlCommand.Parameters.AddWithValue("@updatedCoinBalance", updatedCoinBalance);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public DateTime GetUserLastLoginTime(int userId)
        {
            using (SqlConnection databaseConnection = DataLink.GetConnection())
            {
                databaseConnection.Open();
                string selectLastLoginQuery = "SELECT lastLogin FROM UserWallet WHERE UserId = @userId";
                using (SqlCommand sqlCommand = new SqlCommand(selectLastLoginQuery, databaseConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@userId", userId);
                    return (DateTime?)sqlCommand.ExecuteScalar() ?? DateTime.MinValue;
                }
            }
        }

        public void UpdateUserLastLoginTimeToNow(int userId)
        {
            using (SqlConnection databaseConnection = DataLink.GetConnection())
            {
                databaseConnection.Open();
                string upsertLastLoginQuery = @"
                    IF EXISTS (SELECT 1 FROM UserWallet WHERE UserId = @userId)
                        BEGIN
                            UPDATE UserWallet SET lastLogin = GETDATE() WHERE UserId = @userId
                        END
                    ELSE
                        BEGIN
                            INSERT INTO UserWallet (UserId, coinBalance, lastLogin)
                            VALUES (@userId, @defaultBalance, GETDATE())
                        END";

                using (SqlCommand sqlCommand = new SqlCommand(upsertLastLoginQuery, databaseConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@userId", userId);
                    sqlCommand.Parameters.AddWithValue("@defaultBalance", DefaultInitialCoinBalance);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void AddCoinsToUserWallet(int userId, int amountToAdd)
        {
            int currentCoinBalance = GetUserCoinBalance(userId);
            SetUserCoinBalance(userId, currentCoinBalance + amountToAdd);
        }

        public bool TryDeductCoinsFromUserWallet(int userId, int deductionAmount)
        {
            int currentCoinBalance = GetUserCoinBalance(userId);

            if (currentCoinBalance >= deductionAmount)
            {
                SetUserCoinBalance(userId, currentCoinBalance - deductionAmount);
                return true;
            }

            return false;
        }
    }
}