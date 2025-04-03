using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CourseApp.Models;
using CourseApp.ViewModels;
using Microsoft.UI.Xaml.Navigation;

namespace CourseApp.Views
{
    public sealed partial class CoursePage : Page
    {
        private CourseViewModel viewModel;
        public CoursePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.Parameter is CourseViewModel vm)
            {
                viewModel = vm;
                this.DataContext = viewModel;
                ModulesListView.ItemClick += ModulesListView_ItemClick;
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Simply go back in the navigation stack.
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }


        private void ModulesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is CourseViewModel.ModuleDisplayModel moduleDisplay && viewModel.IsEnrolled)
            {
                if (!moduleDisplay.IsUnlocked)
                {
                    // Optional: Show message if locked
                    var dialog = new ContentDialog
                    {
                        Title = "Locked Module",
                        Content = "Please complete the previous module to unlock this one.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    _ = dialog.ShowAsync();
                    return;
                }

                this.Frame.Navigate(typeof(ModulePage), (moduleDisplay.Module, viewModel));
            }
        }


    }
}
