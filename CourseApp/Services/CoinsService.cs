using CourseApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseApp.Services
{
    public class CoinsService
    {
        private const int UserId = 0;


        private readonly CoinsRepository coinsRepository = new CoinsRepository();
        public CoinsService()
        {

        }

        public int GetUserCoins(int userId)
        {
            return coinsRepository.GetUserCoins(userId);
        }

        public bool SpendCoins(int userId, int cost)
        {
            return coinsRepository.DeductCoins(userId, cost);
        }

        public void EarnCoins(int userId, int amount)
        {
            coinsRepository.AddCoins(userId, amount);
        }

        public bool checkUserDailyLogin(int userId = 0)
        {
            DateTime lastLogin = coinsRepository.GetUserLastLogin(userId);
            DateTime today = DateTime.Now;
            if (lastLogin.Date < today.Date)
            {
                coinsRepository.AddCoins(userId, 100);
                coinsRepository.UpdateLastLogin(userId);
                return true;
            }
            return false;
        }
    }

}