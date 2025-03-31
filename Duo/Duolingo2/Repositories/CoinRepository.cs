using Duo.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duo.Repositories
{
    public class CoinRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public CoinRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> GetCoinsByUserIdAsync(int userId = 0)
        {
            try
            {
                int coins = 0;
                using (var connection = _dbConnection.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = "SELECT CoinBalance FROM UserCoins WHERE UserId = @UserId";
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                coins = reader.GetInt32(0);
                            }
                        }
                    }
                }
                return coins;
            }
            catch (SqlException ex)
            {
                throw new Exception("An error occurred while retrieving the coin balance.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public async void SetUserCoinBalanceAsync(int userId, int coins)
        {
            try
            {
                using (var connection = _dbConnection.GetConnection())
                {
                    await connection.OpenAsync();
                    var query = "UPDATE UserCoins SET CoinBalance = @Coins WHERE UserId = @UserId";
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.AddWithValue("@Coins", coins);
                        command.Parameters.AddWithValue("@UserId", userId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("An error occurred while updating the coin balance.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }
    }
}

