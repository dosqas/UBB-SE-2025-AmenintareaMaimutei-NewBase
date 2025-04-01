using Microsoft.UI.Xaml.Controls;
using CourseApp.Models;
using CourseApp.ViewModels;

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
                this.Frame.Navigate(typeof(CoursePage), selectedCourse);
            }
        }
    }
}
