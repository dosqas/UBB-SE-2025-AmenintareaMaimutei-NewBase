using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseApp.Repository
{
    public interface ICoinsRepository
    {
        void InitializeUserWalletIfNotExists(int userId, int initialCoinBalance = 0);
        int GetUserCoinBalance(int userId);
        void SetUserCoinBalance(int userId, int updatedCoinBalance);
        DateTime GetUserLastLoginTime(int userId);
        void UpdateUserLastLoginTimeToNow(int userId);
        void AddCoinsToUserWallet(int userId, int amountToAdd);
        bool TryDeductCoinsFromUserWallet(int userId, int deductionAmount);
    }
}
