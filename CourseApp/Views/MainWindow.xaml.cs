using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CourseApp.Views
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            // Navigate to MainPage on startup.
            MainFrame.Navigate(typeof(MainPage));
        }
    }
}
