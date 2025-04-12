using System;
using CourseApp.Repository;
using CourseApp.ModelViews;

namespace CourseApp.Services
{
    public class CoinsService : ICoinsService
    {
        private const int UserId = 0;

        private readonly ICoinsRepository coinsRepository;

        public CoinsService(ICoinsRepository coinsRepository = null)
        {
            this.coinsRepository = coinsRepository ?? new CoinsRepository(new UserWalletModelView());
        }

        public int GetCoinBalance(int userId)
        {
            return coinsRepository.GetUserCoinBalance(userId);
        }

        public bool TrySpendingCoins(int userId, int cost)
        {
            return coinsRepository.TryDeductCoinsFromUserWallet(userId, cost);
        }

        public void AddCoins(int userId, int amount)
        {
            coinsRepository.AddCoinsToUserWallet(userId, amount);
        }

        public bool ApplyDailyLoginBonus(int userId = 0)
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
