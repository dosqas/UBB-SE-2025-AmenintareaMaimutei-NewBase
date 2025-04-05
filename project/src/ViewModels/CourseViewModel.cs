using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;
using Microsoft.UI.Xaml;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly

namespace CourseApp.ViewModels
{
    public partial class CourseViewModel : BaseViewModel
    {
        private const int SecondsInMinute = 60;
        private const int NotificationDisplayDurationSeconds = 3;
        private const int TimeTrackingAdjustmentDivisor = 2; // Prevents double-counting time

        private DispatcherTimer? courseProgressTimer;
        private readonly CourseService courseService;
        private readonly CoinsService coinsService;

        private int totalTimeSpentInSeconds;
        private int lastSavedTimeInSeconds;
        private bool isTimerRunning;
        private string? formattedTimeRemaining;
        private string notificationMessage = string.Empty;
        private bool isNotificationVisible = false;

        public Course CurrentCourse { get; private set; }
        public ObservableCollection<ModuleDisplayViewModel> ModuleRoadmap { get; private set; } = [];
        public ObservableCollection<Tag> Tags => new (courseService.GetCourseTags(CurrentCourse.CourseId));

        public ICommand? EnrollCommand { get; private set; }
        public bool IsEnrolled { get; private set; }
        public bool ShouldShowCoinBalance => CurrentCourse.IsPremium && !IsEnrolled;
        public int UserCoinBalance => coinsService.GetUserCoins(userId: 0);

        public int CompletedModuleCount { get; private set; }
        public int RequiredModuleCount { get; private set; }
        public bool IsCourseCompleted => CompletedModuleCount >= RequiredModuleCount;
        public int CourseTimeLimitInSeconds { get; private set; }
        public int RemainingTimeInSeconds => Math.Max(0, CourseTimeLimitInSeconds - totalTimeSpentInSeconds);
        public bool HasClaimedCompletionReward { get; private set; }
        public bool HasClaimedTimedReward { get; private set; }

        public string FormattedTimeRemaining
        {
            get => formattedTimeRemaining!;
            private set
            {
                formattedTimeRemaining = value;
                OnPropertyChanged(nameof(FormattedTimeRemaining));
            }
        }

        public string NotificationMessage
        {
            get => notificationMessage;
            private set
            {
                notificationMessage = value;
                OnPropertyChanged(nameof(NotificationMessage));
            }
        }

        public bool IsNotificationVisible
        {
            get => isNotificationVisible;
            private set
            {
                isNotificationVisible = value;
                OnPropertyChanged(nameof(IsNotificationVisible));
            }
        }

        public class ModuleDisplayViewModel
        {
            public Module? Module { get; set; }
            public bool IsUnlocked { get; set; }
            public bool IsCompleted { get; set; }
        }

        public CourseViewModel(Course course)
        {
            courseService = new CourseService();
            coinsService = new CoinsService();
            CurrentCourse = course;

            InitializeCourseState();
            SetupTimer();
            InitializeCommands();
        }

        private void InitializeCourseState()
        {
            IsEnrolled = courseService.IsUserEnrolled(CurrentCourse.CourseId);
            totalTimeSpentInSeconds = courseService.GetTimeSpent(CurrentCourse.CourseId);
            lastSavedTimeInSeconds = totalTimeSpentInSeconds;

            FormattedTimeRemaining = FormatTimeAsMinutesAndSeconds(RemainingTimeInSeconds);

            CompletedModuleCount = courseService.GetCompletedModulesCount(CurrentCourse.CourseId);
            RequiredModuleCount = courseService.GetRequiredModulesCount(CurrentCourse.CourseId);
            CourseTimeLimitInSeconds = courseService.GetCourseTimeLimit(CurrentCourse.CourseId);

            LoadModuleRoadmap();
        }

        private void SetupTimer()
        {
            courseProgressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            courseProgressTimer.Tick += (sender, eventArgs) => UpdateCourseProgressTime();
        }

        private void InitializeCommands()
        {
            EnrollCommand = new RelayCommand(
                execute: EnrollInCourse!,
                canExecute: CanEnrollInCourse!);
        }

        private void LoadModuleRoadmap()
        {
            var orderedModules = courseService.GetModules(CurrentCourse.CourseId)
                .OrderBy(module => module.Position)
                .ToList();

            foreach (var (module, index) in orderedModules.Select((m, i) => (m, i)))
            {
                bool isModuleCompleted = courseService.IsModuleCompleted(module.ModuleId);
                bool isModuleUnlocked = DetermineModuleUnlockStatus(module, index, orderedModules);

                ModuleRoadmap.Add(new ModuleDisplayViewModel
                {
                    Module = module,
                    IsUnlocked = isModuleUnlocked,
                    IsCompleted = isModuleCompleted
                });
            }

            OnPropertyChanged(nameof(ModuleRoadmap));
        }

        private bool DetermineModuleUnlockStatus(Module module, int index, System.Collections.Generic.List<Module> allModules)
        {
            if (!IsEnrolled)
            {
                return false;
            }
            if (module.IsBonus)
            {
                return courseService.IsModuleInProgress(module.ModuleId);
            }

            return index == 0 || courseService.IsModuleCompleted(allModules[index - 1].ModuleId);
        }

        private bool CanEnrollInCourse(object parameter)
        {
            return !IsEnrolled && UserCoinBalance >= CurrentCourse.Cost;
        }

        private void EnrollInCourse(object parameter)
        {
            if (!courseService.EnrollInCourse(CurrentCourse.CourseId))
            {
                return;
            }

            IsEnrolled = true;
            totalTimeSpentInSeconds = 0;
            FormattedTimeRemaining = FormatTimeAsMinutesAndSeconds(totalTimeSpentInSeconds);

            OnPropertyChanged(nameof(IsEnrolled));
            OnPropertyChanged(nameof(UserCoinBalance));

            StartProgressTimer();
            LoadModuleRoadmap();
        }

        public void StartProgressTimer()
        {
            if (!isTimerRunning && IsEnrolled)
            {
                isTimerRunning = true;
                courseProgressTimer!.Start();
            }
        }

        public void PauseProgressTimer()
        {
            if (isTimerRunning)
            {
                courseProgressTimer!.Stop();
                SaveProgressTime();
                isTimerRunning = false;
            }
        }

        private void SaveProgressTime()
        {
            int unsavedTimeInSeconds = (totalTimeSpentInSeconds - lastSavedTimeInSeconds) / TimeTrackingAdjustmentDivisor;

            if (unsavedTimeInSeconds > 0)
            {
                courseService.UpdateTimeSpent(CurrentCourse.CourseId, unsavedTimeInSeconds);
                lastSavedTimeInSeconds = totalTimeSpentInSeconds;
            }
        }

        private void UpdateCourseProgressTime()
        {
            totalTimeSpentInSeconds++;

            FormattedTimeRemaining = RemainingTimeInSeconds <= 0
                ? FormatTimeAsMinutesAndSeconds(0)
                : FormatTimeAsMinutesAndSeconds(RemainingTimeInSeconds);

            OnPropertyChanged(nameof(RemainingTimeInSeconds));
        }

        private static string FormatTimeAsMinutesAndSeconds(int totalSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
            int totalMinutes = timeSpan.Minutes + (timeSpan.Hours * SecondsInMinute);
            return $"{totalMinutes} min {timeSpan.Seconds} sec";
        }

        public void RefreshModuleRoadmap()
        {
            LoadModuleRoadmap();
        }

        public void CompleteModule(int moduleId)
        {
            courseService.CompleteModule(moduleId, CurrentCourse.CourseId);

            CompletedModuleCount = courseService.GetCompletedModulesCount(CurrentCourse.CourseId);
            OnPropertyChanged(nameof(CompletedModuleCount));
            OnPropertyChanged(nameof(IsCourseCompleted));

            if (IsCourseCompleted)
            {
                ClaimCourseCompletionRewards();
            }
        }

        private void ClaimCourseCompletionRewards()
        {
            bool wasCompletionRewardClaimed = courseService.ClaimCompletionReward(CurrentCourse.CourseId);
            if (wasCompletionRewardClaimed)
            {
                HasClaimedCompletionReward = true;
                NotifyUser("Congratulations! You have completed all required modules in this course. 50 coins have been added to your balance.");
            }

            if (RemainingTimeInSeconds > 0)
            {
                bool wasTimedRewardClaimed = courseService.ClaimTimedReward(CurrentCourse.CourseId, totalTimeSpentInSeconds);
                if (wasTimedRewardClaimed)
                {
                    HasClaimedTimedReward = true;
                    NotifyUser("Congratulations! You completed the course within the time limit. 300 coins have been added to your balance.");
                }
            }
        }

        public void PurchaseBonusModule(Module module)
        {
            if (courseService.IsModuleCompleted(module.ModuleId))
            {
                return;
            }

            bool purchaseSuccessful = courseService.BuyBonusModule(module.ModuleId, CurrentCourse.CourseId);

            if (purchaseSuccessful)
            {
                UpdatePurchasedModuleStatus(module);
                NotifyUser($"Congratulations! You have purchased bonus module {module.Title}, {module.Cost} coins have been deducted from your balance.");
                return;
            }

            NotifyUser("You do not have enough coins to buy this module.");
        }

        private void UpdatePurchasedModuleStatus(Module module)
        {
            var purchasedModule = ModuleRoadmap.FirstOrDefault(m => m.Module!.ModuleId == module.ModuleId);
            if (purchasedModule != null)
            {
                purchasedModule.IsUnlocked = true;
                purchasedModule.IsCompleted = false;
                courseService.OpenModule(module.ModuleId);
            }

            OnPropertyChanged(nameof(ModuleRoadmap));
            OnPropertyChanged(nameof(UserCoinBalance));
            RefreshModuleRoadmap();
        }

        private void NotifyUser(string message)
        {
            NotificationMessage = message;
            IsNotificationVisible = true;

            var notificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(NotificationDisplayDurationSeconds)
            };

            notificationTimer.Tick += (sender, eventArgs) =>
            {
                IsNotificationVisible = false;
                notificationTimer.Stop();
            };

            notificationTimer.Start();
        }
    }
}