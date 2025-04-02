using System.Collections.ObjectModel;
using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using System;


namespace CourseApp.ViewModels

{
    public class CourseViewModel : BaseViewModel
    {
        private DispatcherTimer? timer;
        private DateTime sessionStartTime;
        private int totalTimeSpent;
        private readonly CourseService courseService;
        public Course CurrentCourse { get; set; }
        public ObservableCollection<Models.Module> Modules { get; set; }
        public ICommand EnrollCommand { get; set; }
        public bool IsEnrolled { get; set; }
        public int CoinBalance
        {
            //TODO Implement the CoinBalance property
            get => 20;
        }

        private string timeSpent;
        private bool timerStarted;
        private int lastSavedTime = 0;
        /// <summary>
        /// Formatted string showing the current tracked time spent on the course.
        /// Updated every second while the timer is running.
        /// </summary>
        public string TimeSpent
        {
            get => timeSpent;
            set
            {
                timeSpent = value;
                OnPropertyChanged(nameof(TimeSpent));
            }
        }
        public CourseViewModel(Course course)
        {
            courseService = new CourseService();
            CurrentCourse = course;
            Modules = new ObservableCollection<Models.Module>(courseService.GetModules(course.CourseId));
            IsEnrolled = courseService.IsUserEnrolled(course.CourseId);
            EnrollCommand = new RelayCommand(ExecuteEnroll, CanEnroll);

            totalTimeSpent = courseService.GetTimeSpent(course.CourseId);// Tracks the total time spent on the course in seconds
            lastSavedTime = totalTimeSpent; // Stores the last saved time to calculate delta on save

            TimeSpent = FormatTime(totalTimeSpent);

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) =>
            {
                totalTimeSpent++;
                TimeSpent = FormatTime(totalTimeSpent);
            };

        }


        private bool CanEnroll(object parameter)
        {
            return !IsEnrolled;
        }

        /// <summary>
        /// Handles course enrollment logic. Sets IsEnrolled to true, resets the timer to 0,
        /// and starts timing from scratch for a new enrollment.
        /// </summary>
        private void ExecuteEnroll(object parameter)
        {
            courseService.EnrollInCourse(CurrentCourse.CourseId);
            IsEnrolled = true;
            totalTimeSpent = 0;
            TimeSpent = FormatTime(totalTimeSpent);
            OnPropertyChanged(nameof(IsEnrolled));
            StartTimer();
        }

        /// <summary>
        /// Starts the timer for tracking time spent on the course.
        /// This will only run if the course is enrolled and the timer is not already running.
        /// </summary>
        public void StartTimer()
        {
            if (!timerStarted && IsEnrolled)
            {
                timerStarted = true;
                sessionStartTime = DateTime.Now;
                timer?.Start();
            }
        }

        /// <summary>
        /// Stops the timer and saves the newly accumulated time to the database.
        /// Called when the user navigates away from the course or module.
        /// </summary>
        public void PauseTimer()
        {
            if (timerStarted)
            {
                timer?.Stop();
                SaveTimeSpent();
                timerStarted = false;
            }
        }
        
        /// <summary>
        /// Saves only the new time spent since the last save to the database.
        /// This prevents double-counting time when the app restarts or navigates back.
        /// </summary>
        private void SaveTimeSpent()
        {
            int delta = totalTimeSpent - lastSavedTime;

            if (delta > 0)
            {
                courseService.UpdateTimeSpent(CurrentCourse.CourseId, delta);
                lastSavedTime = totalTimeSpent;
            }
        }

        /// <summary>
        /// Converts a time value in seconds into a formatted string like "X min Y sec".
        /// </summary>
        /// <param name="seconds">The number of seconds to format.</param>
        /// <returns>Formatted string representing the time.</returns>
        private string FormatTime(int seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return $"{ts.Minutes + ts.Hours * 60} min {ts.Seconds} sec";
        }
    }
}
