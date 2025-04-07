using System;
using CourseApp.Repository;

namespace CourseApp.Services
{
    public class CoinsService : ICoinsService
    {
        private const int UserId = 0;

        private readonly CoinsRepository coinsRepository = new CoinsRepository();

        public CoinsService()
        {
        }

        public int GetUserCoins(int userId)
        {
            return coinsRepository.GetUserCoinBalance(userId);
        }

        public bool SpendCoins(int userId, int cost)
        {
            return coinsRepository.TryDeductCoinsFromUserWallet(userId, cost);
        }

        public void EarnCoins(int userId, int amount)
        {
            coinsRepository.AddCoinsToUserWallet(userId, amount);
        }

        public bool CheckUserDailyLogin(int userId = 0)
        {
            DateTime lastLogin = coinsRepository.GetUserLastLoginTime(userId);
            DateTime today = DateTime.Now;
            if (lastLogin.Date < today.Date)
            {
                coinsRepository.AddCoinsToUserWallet(userId, 100);
                coinsRepository.UpdateUserLastLoginTimeToNow(userId);
                return true;
            }
            return false;
        }
    }
}