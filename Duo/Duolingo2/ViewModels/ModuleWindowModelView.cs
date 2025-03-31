using System;
using System.Diagnostics;

namespace Duo
{
    public class ModuleWindowModelView
    {
        private Module module;
        private string status;
        private ModuleService moduleService;

        private Stopwatch sessionTimer;
        private TimeSpan totalTimeFromService;
        private bool timeLogged;

        public ModuleWindowModelView(Module module, ModuleService moduleService, string currentStatus, int userId)
        {
            this.module = module ?? throw new ArgumentNullException(nameof(module));
            this.moduleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
            this.status = currentStatus ?? "Not Completed";
            this.timeLogged = false;

            // Get total time spent from service
            this.totalTimeFromService = moduleService.GetTotalModuleTime(userId, module.ModuleId);

            // Start new session timer
            sessionTimer = new Stopwatch();
            sessionTimer.Start();

            Console.WriteLine($"Module opened. Previous total time: {totalTimeFromService.TotalSeconds:F2} seconds.");
        }

        private void StopAndLogTime(int userId)
        {
            if (!timeLogged && sessionTimer.IsRunning)
            {
                sessionTimer.Stop();
                var updatedTotal = totalTimeFromService + sessionTimer.Elapsed;
                moduleService.UpdateModuleTime(userId, module.ModuleId, updatedTotal);
                Console.WriteLine($"New total time sent to service: {updatedTotal.TotalSeconds:F2} seconds.");
                timeLogged = true;
            }
        }

        public void ToggleCompletion(int userId)
        {
            if (status == "Completed")
            {
                Console.WriteLine("Module already completed.");
                return;
            }

            StopAndLogTime(userId);

            moduleService.ToggleModuleCompletion(userId, module.ModuleId);
            status = "Completed";
            Console.WriteLine($"Module {module.ModuleName} marked as completed.");
        }

        public void HandleBackToCourse(int userId)
        {
            Console.WriteLine("Leaving module and returning to course...");
            StopAndLogTime(userId);
        }

        public void HandleImageClick(int userId, int imageId)
        {
            if (moduleService.IsImageRewardable(module.ModuleId, imageId))
            {
                if (!moduleService.HasUserClaimedImageReward(userId, imageId))
                {
                    int reward = moduleService.GrantImageClickReward(userId, imageId);
                    Console.WriteLine($"Image clicked! User rewarded with {reward} coins.");
                }
                else
                {
                    Console.WriteLine("Reward already claimed for this image.");
                }
            }
            else
            {
                Console.WriteLine("This image does not offer a reward.");
            }
        }

        public string GetStatus()
        {
            return status;
        }

        public Module GetModule()
        {
            return module;
        }

        public TimeSpan GetCurrentSessionTime()
        {
            return sessionTimer.Elapsed;
        }

        public TimeSpan GetTotalTimeIncludingCurrent()
        {
            return totalTimeFromService + sessionTimer.Elapsed;
        }
    }
}
