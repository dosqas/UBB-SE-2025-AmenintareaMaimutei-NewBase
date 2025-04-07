using System;

namespace CourseApp.Services
{
    /// <summary>
    /// Interface for coin-related operations
    /// </summary>
    public interface ICoinsService
    {
        /// <summary>
        /// Gets the current coin balance for a user
        /// </summary>
        int GetUserCoins(int userId);

        /// <summary>
        /// Attempts to spend coins from a user's wallet
        /// </summary>
        /// <returns>True if successful, false if insufficient funds</returns>
        bool SpendCoins(int userId, int cost);

        /// <summary>
        /// Adds coins to a user's wallet
        /// </summary>
        void EarnCoins(int userId, int amount);

        /// <summary>
        /// Checks if user has logged in today and grants daily reward if not
        /// </summary>
        /// <returns>True if daily reward was granted, false if already logged in today</returns>
        bool CheckUserDailyLogin(int userId = 0);
    }
}