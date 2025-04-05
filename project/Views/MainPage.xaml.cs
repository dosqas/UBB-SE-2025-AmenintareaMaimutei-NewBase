using Microsoft.UI.Xaml.Controls;
using CourseApp.Models;
using CourseApp.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using System;


namespace CourseApp.Views
{
    public sealed partial class MainPage : Page
    {
        //keep this static so that the dialog is only shown once. The page is recreated every time it is navigated to.
        private static bool _dialogShown = false;

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
            CoursesListView.ItemClick += CoursesListView_ItemClick;
        }

        private async void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure the dialog is only shown once. Just in case.
            if (!_dialogShown)
            {
                _dialogShown = true;

                bool dailyLoginRewardEligible = (this.DataContext as MainViewModel)!.CheckUserDailyLogin();

                if (dailyLoginRewardEligible)
                {
                    ContentDialog welcomeDialog = new ContentDialog
                    {
                        Title = "Welcome!",
                        Content = "You have been granted the daily login reward! 100 coins Just for you <3",
                        CloseButtonText = "Cheers!",
                        XamlRoot = RootGrid.XamlRoot
                    };
                    await welcomeDialog.ShowAsync();
                }
            }
        }

        private void CoursesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Course selectedCourse)
            {
                var mainWindow = MainWindow.Instance;
                var courseVM = mainWindow.GetOrCreateCourseViewModel(selectedCourse);
                this.Frame.Navigate(typeof(CoursePage), courseVM);
            }
        }

    }
}
