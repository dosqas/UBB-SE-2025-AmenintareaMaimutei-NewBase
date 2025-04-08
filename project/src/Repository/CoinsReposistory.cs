using System;

namespace CourseApp.Repository
{
    public class CoinsRepository : ICoinsRepository
    {
        private readonly UserWalletModelView walletModelView;

        public CoinsRepository()
        {
            walletModelView = new UserWalletModelView();
        }

        public void InitializeUserWalletIfNotExists(int userId, int initialCoinBalance = 0) =>
            walletModelView.InitializeUserWalletIfNotExists(userId, initialCoinBalance);

        public int GetUserCoinBalance(int userId) =>
            walletModelView.GetUserCoinBalance(userId);

        public void SetUserCoinBalance(int userId, int updatedCoinBalance) =>
            walletModelView.SetUserCoinBalance(userId, updatedCoinBalance);

        public DateTime GetUserLastLoginTime(int userId) =>
            walletModelView.GetUserLastLoginTime(userId);

        public void UpdateUserLastLoginTimeToNow(int userId) =>
            walletModelView.UpdateUserLastLoginTimeToNow(userId);

        public void AddCoinsToUserWallet(int userId, int amountToAdd) =>
            walletModelView.AddCoinsToUserWallet(userId, amountToAdd);

        public bool TryDeductCoinsFromUserWallet(int userId, int deductionAmount) =>
            walletModelView.TryDeductCoinsFromUserWallet(userId, deductionAmount);
    }
}