using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseApp.Services
{
    public interface ICoinsService
    {
        int GetCoinBalance(int userId);
        bool TrySpendingCoins(int userId, int cost);
        void AddCoins(int userId, int amount);
        bool ApplyDailyLoginBonu(int userId = 0);
    }
}
