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
                vm.StartTimer();
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                viewModel.PauseTimer();
                this.Frame.GoBack(); 
            }
        }


        private void ModulesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is CourseViewModel.ModuleDisplayModelView moduleDisplay && viewModel.IsEnrolled)
            {
                if (moduleDisplay.IsUnlocked)
                {
                    this.Frame.Navigate(typeof(ModulePage), (moduleDisplay.Module, viewModel));
                    return;
                }
                if (moduleDisplay.Module.IsBonus)
                {
                    viewModel.TryBuyBonusModule(moduleDisplay.Module);

                }
                var dialog = new ContentDialog
                {
                    Title = "Module Locked",
                    Content = "You need to complete the previous modules to unlock this one.",
                    CloseButtonText = "OK"
                };
            }
        }


    }
}
