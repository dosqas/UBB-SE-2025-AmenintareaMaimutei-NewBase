using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CourseApp.Models;
using CourseApp.ViewModels;

namespace CourseApp.Views
{
    public sealed partial class ModulePage : Page
    {
        private ModuleViewModel viewModel;
        public ModulePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var module = e.Parameter as Models.Module;
            viewModel = new ModuleViewModel(module);
            this.DataContext = viewModel;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Instead of navigating to a new CoursePage, simply go back.
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }
    }
}
