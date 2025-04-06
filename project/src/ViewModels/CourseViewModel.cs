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
        #region Constants
        private const int NotificationDisplayDurationInSeconds = 3;
        private const int CourseCompletionRewardCoins = 50;
        private const int TimedCompletionRewardCoins = 300;
        private const int TimeTrackingDatabaseAdjustmentDivisor = 2;
        private const int MinutesInAnHour = 60;
        private const int SecondsInOneSecond = 1;
        #endregion

        #region Fields
        private DispatcherTimer courseProgressTimer;
        private int totalSecondsSpentOnCourse;
        private int courseCompletionTimeLimitInSeconds;
        private string formattedTimeRemaining;
        private bool isCourseTimerRunning;
        private int lastSavedTimeInSeconds = 0;

        private readonly CourseService courseService;
        private readonly CoinsService coinsService;

        private string notificationMessageText = string.Empty;
        private bool shouldShowNotification = false;
        #endregion

        #region Properties
        public Course CurrentCourse { get; }
        public ObservableCollection<ModuleProgressStatus> ModuleRoadmap { get; } = new ObservableCollection<ModuleProgressStatus>();

        public ICommand EnrollCommand { get; private set; }
        public bool IsEnrolled { get; private set; }

        public bool CoinVisibility => CurrentCourse.IsPremium && !IsEnrolled;
        public int CoinBalance => coinsService.GetUserCoins(0);

        public ObservableCollection<Tag> Tags => new ObservableCollection<Tag>(courseService.GetCourseTags(CurrentCourse.CourseId));

        public string FormattedTimeRemaining
        {
            get => formattedTimeRemaining;
            private set
            {
                formattedTimeRemaining = value;
                OnPropertyChanged();
            }
        }

        public string NotificationMessage
        {
            get => notificationMessageText;
            private set
            {
                notificationMessageText = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNotification
        {
            get => shouldShowNotification;
            private set
            {
                shouldShowNotification = value;
                OnPropertyChanged();
            }
        }

        public int CompletedModules { get; private set; }
        public int RequiredModules { get; private set; }
        public bool IsCourseCompleted => CompletedModules >= RequiredModules;
        public int TimeLimit { get; set; }
        public int TimeRemaining => Math.Max(0, TimeLimit - totalSecondsSpentOnCourse);
        public bool CompletionRewardClaimed { get; private set; }
        public bool TimedRewardClaimed { get; private set; }
        #endregion

        #region Nested Classes
        public class ModuleProgressStatus
        {
            public Module Module { get; set; }
            public bool IsUnlocked { get; set; }
            public bool IsCompleted { get; set; }
        }

        private class NotificationHelper
        {
            private readonly CourseViewModel parent;

            public NotificationHelper(CourseViewModel parentViewModel)
            {
                parent = parentViewModel;
            }

            public void ShowTemporaryNotification(string message)
            {
                parent.NotificationMessage = message;
                parent.ShowNotification = true;

                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(NotificationDisplayDurationInSeconds)
                };
                timer.Tick += OnNotificationTimerTick;
                timer.Start();
            }

            private void OnNotificationTimerTick(object sender, object e)
            {
                if (sender is DispatcherTimer timer)
                {
                    parent.ShowNotification = false;
                    timer.Tick -= OnNotificationTimerTick;
                    timer.Stop();
                }
            }
        }
        #endregion

        #region Constructor
        public CourseViewModel(Course course)
        {
            CurrentCourse = course ?? throw new ArgumentNullException(nameof(course));
            courseService = new CourseService();
            coinsService = new CoinsService();
            notificationHelper = new NotificationHelper(this);

            InitializeProperties();
            SetupCourseTimer();
            LoadInitialData();
        }
        private void InitializeProperties()
        {
            IsEnrolled = courseService.IsUserEnrolled(CurrentCourse.CourseId);
            EnrollCommand = new RelayCommand(EnrollUserInCourse, CanUserEnrollInCourse);
        }

        private void SetupCourseTimer()
        {
            courseProgressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(SecondsInOneSecond)
            };
            courseProgressTimer.Tick += OnCourseTimerTick;
        }

        private void LoadInitialData()
        {
            totalSecondsSpentOnCourse = courseService.GetTimeSpent(CurrentCourse.CourseId);
            lastSavedTimeInSeconds = totalSecondsSpentOnCourse;
            courseCompletionTimeLimitInSeconds = CurrentCourse.TimeToComplete - totalSecondsSpentOnCourse;
            FormattedTimeRemaining = FormatTimeRemainingDisplay(courseCompletionTimeLimitInSeconds - totalSecondsSpentOnCourse);

            CompletedModules = courseService.GetCompletedModulesCount(CurrentCourse.CourseId);
            RequiredModules = courseService.GetRequiredModulesCount(CurrentCourse.CourseId);
            TimeLimit = courseService.GetCourseTimeLimit(CurrentCourse.CourseId);

            LoadAndOrganizeCourseModules();
        }
        #endregion

        #region Timer Methods
        private void OnCourseTimerTick(object sender, object e)
        {
            totalSecondsSpentOnCourse++;
            UpdateTimeDisplay();
            OnPropertyChanged(nameof(TimeRemaining));
        }

        private void UpdateTimeDisplay()
        {
            int remainingSeconds = courseCompletionTimeLimitInSeconds - totalSecondsSpentOnCourse;
            FormattedTimeRemaining = FormatTimeRemainingDisplay(Math.Max(0, remainingSeconds));
        }
        #endregion

        #region Module Management
        private void LoadAndOrganizeCourseModules()
        {
            var modules = courseService.GetModules(CurrentCourse.CourseId)
                           .OrderBy(module => module.Position)
                           .ToList();

            ModuleRoadmap.Clear();

            for (int index = 0; index < modules.Count; index++)
            {
                var module = modules[index];
                bool isCompleted = courseService.IsModuleCompleted(module.ModuleId);
                bool isUnlocked = GetModuleUnlockStatus(module, index);

                ModuleRoadmap.Add(new ModuleProgressStatus
                {
                    Module = module,
                    IsUnlocked = isUnlocked,
                    IsCompleted = isCompleted
                });
            }

            OnPropertyChanged(nameof(ModuleRoadmap));
        }

        private bool GetModuleUnlockStatus(Module module, int moduleIndex)
        {
            if (!module.IsBonus)
            {
                return IsEnrolled &&
                      (moduleIndex == 0 ||
                       courseService.IsModuleCompleted(ModuleRoadmap[moduleIndex - 1].Module.ModuleId));
            }
            return courseService.IsModuleInProgress(module.ModuleId);
        }
        private bool CanUserEnrollInCourse(object? parameter)
        {
            return !IsEnrolled && CoinBalance >= CurrentCourse.Cost;
        }

        private void EnrollUserInCourse(object? parameter)
        {
            if (!courseService.EnrollInCourse(CurrentCourse.CourseId))
            {
                return;
            }

            IsEnrolled = true;
            ResetCourseProgressTracking();
            OnPropertyChanged(nameof(IsEnrolled));
            OnPropertyChanged(nameof(CoinBalance));
            StartCourseProgressTimer();
            LoadAndOrganizeCourseModules();
        }

        private void ResetCourseProgressTracking()
        {
            totalSecondsSpentOnCourse = 0;
            FormattedTimeRemaining = FormatTimeRemainingDisplay(totalSecondsSpentOnCourse);
        }
        #endregion

        #region Timer Control Methods
        public void StartCourseProgressTimer()
        {
            if (!isCourseTimerRunning && IsEnrolled)
            {
                isCourseTimerRunning = true;
                courseProgressTimer.Start();
            }
        }

        public void PauseCourseProgressTimer()
        {
            if (isCourseTimerRunning)
            {
                courseProgressTimer.Stop();
                SaveCourseProgressTime();
                isCourseTimerRunning = false;
            }
        }

        private void SaveCourseProgressTime()
        {
            int secondsToSave = (totalSecondsSpentOnCourse - lastSavedTimeInSeconds) / TimeTrackingDatabaseAdjustmentDivisor;

            if (secondsToSave > 0)
            {
                courseService.UpdateTimeSpent(CurrentCourse.CourseId, secondsToSave);
                lastSavedTimeInSeconds = totalSecondsSpentOnCourse;
            }
        }
        #endregion

        #region Utility Methods
        private static string FormatTimeRemainingDisplay(int totalSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
            int totalMinutes = timeSpan.Minutes + (timeSpan.Hours * MinutesInAnHour);
            return $"{totalMinutes} min {timeSpan.Seconds} sec";
        }

        public void RefreshCourseModulesDisplay()
        {
            LoadAndOrganizeCourseModules();
        }
        #endregion

        #region Reward Handling
        public void MarkModuleAsCompletedAndCheckRewards(int targetModuleId)
        {
            courseService.CompleteModule(targetModuleId, CurrentCourse.CourseId);
            UpdateCompletionStatus();

            if (IsCourseCompleted)
            {
                CheckForCompletionReward();
                CheckForTimedReward();
            }
        }

        private void UpdateCompletionStatus()
        {
            CompletedModules = courseService.GetCompletedModulesCount(CurrentCourse.CourseId);
            OnPropertyChanged(nameof(CompletedModules));
            OnPropertyChanged(nameof(IsCourseCompleted));
        }

        private void CheckForCompletionReward()
        {
            bool rewardClaimed = courseService.ClaimCompletionReward(CurrentCourse.CourseId);
            if (rewardClaimed)
            {
                CompletionRewardClaimed = true;
                OnPropertyChanged(nameof(CompletionRewardClaimed));
                OnPropertyChanged(nameof(CoinBalance));
                ShowCourseCompletionRewardNotification();
            }
        }

        private void CheckForTimedReward()
        {
            if (TimeRemaining > 0)
            {
                bool rewardClaimed = courseService.ClaimTimedReward(CurrentCourse.CourseId, totalSecondsSpentOnCourse);
                if (rewardClaimed)
                {
                    TimedRewardClaimed = true;
                    OnPropertyChanged(nameof(TimedRewardClaimed));
                    OnPropertyChanged(nameof(CoinBalance));
                    ShowTimedCompletionRewardNotification();
                }
            }
        }
        #endregion

        #region Notification Methods
        private readonly NotificationHelper notificationHelper;

        private void ShowCourseCompletionRewardNotification()
        {
            string message = $"Congratulations! You have completed all required modules in this course. {CourseCompletionRewardCoins} coins have been added to your balance.";
            notificationHelper.ShowTemporaryNotification(message);
        }

        private void ShowTimedCompletionRewardNotification()
        {
            string message = $"Congratulations! You completed the course within the time limit. {TimedCompletionRewardCoins} coins have been added to your balance.";
            notificationHelper.ShowTemporaryNotification(message);
        }

        private void ShowModulePurchaseNotification(Module module)
        {
            string message = $"Congratulations! You have purchased bonus module {module.Title}, {module.Cost} coins have been deducted from your balance.";
            notificationHelper.ShowTemporaryNotification(message);
            RefreshCourseModulesDisplay();
        }
        #endregion

        #region Module Purchase
        public void AttemptBonusModulePurchase(Module module)
        {
            if (courseService.IsModuleCompleted(module.ModuleId))
            {
                return;
            }

            bool purchaseSuccessful = courseService.BuyBonusModule(module.ModuleId, CurrentCourse.CourseId);

            if (purchaseSuccessful)
            {
                UpdatePurchasedModuleStatus(module);
                ShowModulePurchaseNotification(module);
                OnPropertyChanged(nameof(ModuleRoadmap));
                OnPropertyChanged(nameof(CoinBalance));
            }
            else
            {
                ShowPurchaseFailedNotification();
            }
        }

        private void UpdatePurchasedModuleStatus(Module module)
        {
            var moduleToUpdate = ModuleRoadmap.FirstOrDefault(m => m.Module.ModuleId == module.ModuleId);
            if (moduleToUpdate != null)
            {
                moduleToUpdate.IsUnlocked = true;
                moduleToUpdate.IsCompleted = false;
                courseService.OpenModule(module.ModuleId);
            }
        }

        private void ShowPurchaseFailedNotification()
        {
            notificationHelper.ShowTemporaryNotification("You do not have enough coins to buy this module.");
        }
        #endregion
    }
}