using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;
using Windows.UI.Popups;

namespace CourseApp.ViewModels
{
    public class ModuleViewModel : BaseViewModel
    {
        private readonly CourseService courseService;
        private readonly CoinsService coinsService;
        private readonly CourseViewModel courseViewModel;
        public Module CurrentModule { get; set; }
        public bool IsCompleted { get; set; }
        public ICommand CompleteModuleCommand { get; set; }
        public ICommand ModuleImageClick { get; }

        public ModuleViewModel(Models.Module module , CourseViewModel courseVM)
        {
            courseService = new CourseService();
            coinsService = new CoinsService();
            coinsService.GetUserCoins(0);
            CurrentModule = module;
            IsCompleted = courseService.IsModuleCompleted(module.ModuleId);
            CompleteModuleCommand = new RelayCommand(ExecuteCompleteModule, CanCompleteModule);
            ModuleImageClick = new RelayCommand(OnModuleImageClick);
            courseViewModel = courseVM;
            courseViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(courseViewModel.TimeSpent))
                {
                    OnPropertyChanged(nameof(TimeSpent));
                }
            };
        }

        public string TimeSpent => courseViewModel.TimeSpent;


        public int CoinBalance
        {
            get => coinsService.GetUserCoins(0);
        }

        private async void OnModuleImageClick(object? parameter)
        {
            // Pop up with a text
            MessageDialog dialog = new MessageDialog("Image clicked");
            dialog.ShowAsync();
        }

        private bool CanCompleteModule(object parameter)
        {
            return !IsCompleted;
        }

        private void ExecuteCompleteModule(object parameter)
        {
            // Mark module as complete in the database.
            courseService.CompleteModule(CurrentModule.ModuleId);
            IsCompleted = true;
            OnPropertyChanged(nameof(IsCompleted));
        }
    }
}
