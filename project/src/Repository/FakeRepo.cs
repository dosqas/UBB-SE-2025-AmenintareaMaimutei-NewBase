using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseApp.Repository
{
    public class FakeCoinsRepository : ICoinsRepository
    {
        private int userCoinBalance = 100;
        private DateTime lastLogin = DateTime.Now.AddDays(-2);

        public int GetUserCoinBalance(int userId) => userCoinBalance;

        public bool TryDeductCoinsFromUserWallet(int userId, int cost)
        {
            if (userCoinBalance >= cost)
            {
                userCoinBalance -= cost;
                return true;
            }
            return false;
        }

        public void AddCoinsToUserWallet(int userId, int amount) =>
            userCoinBalance += amount;

        public DateTime GetUserLastLoginTime(int userId) => lastLogin;

        public void UpdateUserLastLoginTimeToNow(int userId) =>
            lastLogin = DateTime.Now;

        public void InitializeUserWalletIfNotExists(int userId, int initialCoinBalance = 0)
        {
            userCoinBalance += initialCoinBalance;
        }

        public void SetUserCoinBalance(int userId, int updatedCoinBalance) =>
            userCoinBalance = updatedCoinBalance;
    }
}
