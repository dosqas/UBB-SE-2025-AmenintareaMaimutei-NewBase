using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;
using Microsoft.UI.Xaml;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly

namespace CourseApp.ViewModels
{
    public partial class CourseViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private const int SecondsInOneMinute = 60;
        private const int NotificationVisibleDurationInSeconds = 3;
        private const int TimeTrackingDivider = 2;

        protected DispatcherTimer? courseProgressDispatcherTimer;
        protected readonly CourseService courseService;
        protected readonly CoinsService coinsService;

        protected int totalSecondsSpentOnCourse;
        protected int lastSavedTimeInSeconds;
        protected bool isProgressTimerActive;
        protected string timeRemainingFormatted = string.Empty;
        protected string userNotificationMessage = string.Empty;
        protected bool shouldShowNotification;

        public Course CurrentCourse { get; }

        public ObservableCollection<ModuleDisplayViewModel> DisplayedModules { get; } = [];

        public ObservableCollection<Tag> CourseTags => new (courseService.GetCourseTags(CurrentCourse.CourseId));
        public ICommand EnrollInCourseCommand { get; private set; } = null!;

        public bool IsUserEnrolled { get; private set; }
        public bool ShouldDisplayCoinBalance => CurrentCourse.IsPremium && !IsUserEnrolled;
        public int UserCoinBalance => coinsService.GetUserCoins(userId: 0);
        public int CountOfCompletedModules { get; private set; }
        public int CountOfRequiredModules { get; private set; }
        public bool HasUserCompletedCourse => CountOfCompletedModules >= CountOfRequiredModules;
        public int CourseTotalTimeLimitInSeconds { get; private set; }
        public int CourseRemainingTimeInSeconds => Math.Max(0, CourseTotalTimeLimitInSeconds - totalSecondsSpentOnCourse);
        public bool HasUserClaimedCompletionReward { get; private set; }
        public bool HasUserClaimedTimeReward { get; private set; }

        public string TimeRemainingFormatted
        {
            get => timeRemainingFormatted;
            private set => SetProperty(ref timeRemainingFormatted, value);
        }

        public string UserNotificationMessage
        {
            get => userNotificationMessage;
            private set => SetProperty(ref userNotificationMessage, value);
        }

        public bool ShouldShowNotification
        {
            get => shouldShowNotification;
            private set => SetProperty(ref shouldShowNotification, value);
        }

        public class ModuleDisplayViewModel
        {
            public Module? Module { get; set; }
            public bool IsUnlocked { get; set; }
            public bool IsCompleted { get; set; }
        }

        public CourseViewModel(Course course, CourseService? injectedCourseService = null, CoinsService? injectedCoinsService = null)
        {
            courseService = injectedCourseService ?? new CourseService();
            coinsService = injectedCoinsService ?? new CoinsService();
            CurrentCourse = course;

            InitializeCourseData();
            ConfigureProgressTrackingTimer();
            SetupCommands();
        }

        protected void InitializeCourseData()
        {
            IsUserEnrolled = courseService.IsUserEnrolled(CurrentCourse.CourseId);
            totalSecondsSpentOnCourse = courseService.GetTimeSpent(CurrentCourse.CourseId);
            lastSavedTimeInSeconds = totalSecondsSpentOnCourse;

            TimeRemainingFormatted = ConvertSecondsToReadableFormat(CourseRemainingTimeInSeconds);

            CountOfCompletedModules = courseService.GetCompletedModulesCount(CurrentCourse.CourseId);
            CountOfRequiredModules = courseService.GetRequiredModulesCount(CurrentCourse.CourseId);
            CourseTotalTimeLimitInSeconds = courseService.GetCourseTimeLimit(CurrentCourse.CourseId);

            LoadDisplayedModules();
        }

        protected void ConfigureProgressTrackingTimer()
        {
            courseProgressDispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            courseProgressDispatcherTimer.Tick += (sender, args) => IncrementCourseProgressTime();
        }

        protected void SetupCommands()
        {
            EnrollInCourseCommand = new RelayCommand(ExecuteEnrollInCourse, CanUserEnrollInCourse);
        }

        protected void LoadDisplayedModules()
        {
            DisplayedModules.Clear();

            var orderedModules = courseService.GetModules(CurrentCourse.CourseId)
                .OrderBy(module => module.Position)
                .ToList();

            foreach (var (module, position) in orderedModules.Select((module, index) => (module, index)))
            {
                var displayModel = new ModuleDisplayViewModel
                {
                    Module = module,
                    IsCompleted = courseService.IsModuleCompleted(module.ModuleId),
                    IsUnlocked = DetermineIfModuleIsUnlocked(module, position, orderedModules)
                };

                DisplayedModules.Add(displayModel);
            }

            OnPropertyChanged(nameof(DisplayedModules));
        }

        protected bool DetermineIfModuleIsUnlocked(Module module, int index, System.Collections.Generic.List<Module> moduleList)
        {
            if (!IsUserEnrolled)
            {
                return false;
            }

            if (module.IsBonus)
            {
                return courseService.IsModuleInProgress(module.ModuleId);
            }

            return index == 0 || courseService.IsModuleCompleted(moduleList[index - 1].ModuleId);
        }

        internal bool CanUserEnrollInCourse(object? parameter) =>
            !IsUserEnrolled && UserCoinBalance >= CurrentCourse.Cost;

        protected void ExecuteEnrollInCourse(object? parameter)
        {
            if (!courseService.EnrollInCourse(CurrentCourse.CourseId))
            {
                return;
            }

            IsUserEnrolled = true;
            totalSecondsSpentOnCourse = 0;
            TimeRemainingFormatted = ConvertSecondsToReadableFormat(0);

            OnPropertyChanged(nameof(IsUserEnrolled));
            OnPropertyChanged(nameof(UserCoinBalance));

            StartProgressTracking();
            LoadDisplayedModules();
        }

        public void StartProgressTracking()
        {
            if (!isProgressTimerActive && IsUserEnrolled)
            {
                isProgressTimerActive = true;
                courseProgressDispatcherTimer?.Start();
            }
        }

        public void PauseProgressTracking()
        {
            if (isProgressTimerActive)
            {
                courseProgressDispatcherTimer?.Stop();
                SaveCourseProgressTime();
                isProgressTimerActive = false;
            }
        }

        protected void SaveCourseProgressTime()
        {
            int unsavedElapsedTime = (totalSecondsSpentOnCourse - lastSavedTimeInSeconds) / TimeTrackingDivider;

            if (unsavedElapsedTime > 0)
            {
                courseService.UpdateTimeSpent(CurrentCourse.CourseId, unsavedElapsedTime);
                lastSavedTimeInSeconds = totalSecondsSpentOnCourse;
            }
        }

        protected void IncrementCourseProgressTime()
        {
            totalSecondsSpentOnCourse++;
            TimeRemainingFormatted = ConvertSecondsToReadableFormat(CourseRemainingTimeInSeconds);
            OnPropertyChanged(nameof(CourseRemainingTimeInSeconds));
        }

        protected static string ConvertSecondsToReadableFormat(int totalSeconds)
        {
            var time = TimeSpan.FromSeconds(totalSeconds);
            int minutes = time.Minutes + (time.Hours * SecondsInOneMinute);
            return $"{minutes} min {time.Seconds} sec";
        }

        public void RefreshDisplayedModules() => LoadDisplayedModules();

        public void MarkModuleAsCompleted(int moduleId)
        {
            courseService.CompleteModule(moduleId, CurrentCourse.CourseId);

            CountOfCompletedModules = courseService.GetCompletedModulesCount(CurrentCourse.CourseId);
            OnPropertyChanged(nameof(CountOfCompletedModules));
            OnPropertyChanged(nameof(HasUserCompletedCourse));

            if (HasUserCompletedCourse)
            {
                GrantCourseCompletionRewards();
            }
        }

        protected void GrantCourseCompletionRewards()
        {
            if (courseService.ClaimCompletionReward(CurrentCourse.CourseId))
            {
                HasUserClaimedCompletionReward = true;
                ShowTemporaryNotification("Congratulations! You completed all required modules. 50 coins added.");
            }

            bool finishedWithinTime = CourseRemainingTimeInSeconds > 0;

            if (finishedWithinTime &&
                courseService.ClaimTimedReward(CurrentCourse.CourseId, totalSecondsSpentOnCourse))
            {
                HasUserClaimedTimeReward = true;
                ShowTemporaryNotification("Finished in time! 300 coins added.");
            }
        }

        public void AttemptBonusModulePurchase(Module module)
        {
            if (courseService.IsModuleCompleted(module.ModuleId))
            {
                return;
            }

            if (courseService.BuyBonusModule(module.ModuleId, CurrentCourse.CourseId))
            {
                UnlockBonusModule(module);
                ShowTemporaryNotification($"Purchased bonus module '{module.Title}', {module.Cost} coins deducted.");
            }
            else
            {
                ShowTemporaryNotification("Not enough coins to buy this module.");
            }
        }

        protected void UnlockBonusModule(Module module)
        {
            var matchingModule = DisplayedModules.FirstOrDefault(m => m.Module?.ModuleId == module.ModuleId);

            if (matchingModule != null)
            {
                matchingModule.IsUnlocked = true;
                matchingModule.IsCompleted = false;
                courseService.OpenModule(module.ModuleId);
            }

            OnPropertyChanged(nameof(DisplayedModules));
            OnPropertyChanged(nameof(UserCoinBalance));
            RefreshDisplayedModules();
        }

        protected void ShowTemporaryNotification(string message)
        {
            UserNotificationMessage = message;
            ShouldShowNotification = true;

            var hideNotificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(NotificationVisibleDurationInSeconds)
            };

            hideNotificationTimer.Tick += (sender, args) =>
            {
                ShouldShowNotification = false;
                hideNotificationTimer.Stop();
            };

            hideNotificationTimer.Start();
        }
    }
}