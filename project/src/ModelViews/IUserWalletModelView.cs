using System;

namespace CourseApp.Repository
{
    public interface IUserWalletModelView
    {
        void AddCoinsToUserWallet(int userId, int amountToAdd);
        int GetUserCoinBalance(int userId);
        DateTime GetUserLastLoginTime(int userId);
        void InitializeUserWalletIfNotExists(int userId, int initialCoinBalance = 0);
        void SetUserCoinBalance(int userId, int updatedCoinBalance);
        bool TryDeductCoinsFromUserWallet(int userId, int deductionAmount);
        void UpdateUserLastLoginTimeToNow(int userId);
    }
}