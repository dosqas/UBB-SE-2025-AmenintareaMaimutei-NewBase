using Duo.Models;
using Duo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duo.Services
{
    public class ModuleService
    {
        private readonly ModuleRepository moduleRepository;
        private readonly CoinRepository coinRepository;
        private readonly int currentUserId;

        public ModuleService(ModuleRepository moduleRepository, CoinRepository coinRepository)
        {
            this.moduleRepository = moduleRepository;
            this.coinRepository = coinRepository;
            this.currentUserId = 0;
        }

        public void MarkModuleAsCompleted(int courseId, int moduleId)
        {
            moduleRepository.MarkModuleAsCompletedAsync(courseId, moduleId, currentUserId).Wait();
        }

        public bool isModuleCompleted(int courseId, int moduleId)
        {
            return moduleRepository.IsModuleCompletedAsync(courseId, moduleId, currentUserId).Result;
        }

        public void UnlockBonusModule(int CourseId, int moduleId)
        {
            Module module = moduleRepository.GetModuleById(CourseId, moduleId);

            int userCoins = coinRepository.GetCoinsByUserIdAsync(currentUserId).Result;

            if(userCoins >= module.UnlockCost)
            {
                coinRepository.SetUserCoinBalanceAsync(currentUserId, userCoins - module.UnlockCost);
                moduleRepository.UnlockBonusModule(moduleId, currentUserId);
            }

            else
            {
                throw new Exception("Not enough coins to unlock module");
            }

        }
    }

}
