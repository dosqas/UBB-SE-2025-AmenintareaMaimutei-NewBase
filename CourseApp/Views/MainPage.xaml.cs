using Microsoft.UI.Xaml.Controls;
using CourseApp.Models;
using CourseApp.ViewModels;
using Microsoft.UI.Xaml;


namespace CourseApp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
            CoursesListView.ItemClick += CoursesListView_ItemClick;
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
