using CourseApp.Models;
using CourseApp.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace CourseApp.Views
{
    public sealed partial class MainWindow : Window
    {
        private readonly Dictionary<int, CourseViewModel> courseVMCache = new();
        private CourseViewModel? currentCourseVM;
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();
            Instance = this;

            MainFrame.Navigated += OnMainFrameNavigated;
            MainFrame.Navigate(typeof(MainPage));
        }

        /// <summary>
        /// Called every time the MainFrame navigates to a new page.
        /// Controls which course timer should start or stop depending on navigation:
        /// - Pauses the previous course's timer if switching
        /// - Starts the current course timer only
        /// - Resumes timer from module to course and vice versa
        /// </summary>
        private void OnMainFrameNavigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            // Navigated to a course page
            if (e.Parameter is Course course)
            {
                if (!courseVMCache.TryGetValue(course.CourseId, out var newVM))
                {
                    newVM = new CourseViewModel(course);
                    courseVMCache[course.CourseId] = newVM;
                }

                // Only pause previous timer if switching courses
                if (currentCourseVM != null && currentCourseVM != newVM)
                {
                    currentCourseVM.PauseTimer();
                }

                currentCourseVM = newVM;
                currentCourseVM.StartTimer();
            }

            // Navigated to a module page
            else if (e.Parameter is ValueTuple<Module, CourseViewModel> tuple)
            {
                var courseVM = tuple.Item2;

                if (currentCourseVM != null && currentCourseVM != courseVM)
                {
                    currentCourseVM.PauseTimer();
                }

                currentCourseVM = courseVM;
                currentCourseVM.StartTimer();
            }

            // Navigated to something else (like MainPage)
            else
            {
                // Only pause if leaving course/module
                if (e.SourcePageType != typeof(CoursePage) && e.SourcePageType != typeof(ModulePage))
                {
                    currentCourseVM?.PauseTimer();
                }
            }
        }





        public CourseViewModel GetOrCreateCourseViewModel(Course course)
        {
            if (!courseVMCache.TryGetValue(course.CourseId, out var vm))
            {
                vm = new CourseViewModel(course);
                courseVMCache[course.CourseId] = vm;
            }
            return vm;
        }

    }
}
