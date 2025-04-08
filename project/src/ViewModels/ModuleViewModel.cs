using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;

namespace CourseApp.ViewModels
{
    public partial class ModuleViewModel : BaseViewModel
    {
        private readonly CourseService courseService;
        private readonly CoinsService coinsService;
        private readonly CourseViewModel courseViewModel;
        public Module CurrentModule { get; set; }
        public bool IsCompleted { get; set; }
        public ICommand CompleteModuleCommand { get; set; }

        public ICommand ModuleImageClickCommand { get; set; }

        public ModuleViewModel(Models.Module module, CourseViewModel courseVM,
            ICourseService? courseServiceOverride = null,
            ICoinsService? coinsServiceOverride = null)
        {
            // Corrected initialization: Use the proper concrete service classes
            courseService = courseService ?? new CourseService();
            coinsService = coinsService ?? new CoinsService();

            CurrentModule = module;
            IsCompleted = courseService.IsModuleCompleted(module.ModuleId);
            CompleteModuleCommand = new RelayCommand(ExecuteCompleteModule, CanCompleteModule);
            ModuleImageClickCommand = new RelayCommand(HandleModuleImageClick);
            courseViewModel = courseVM;

            courseService.OpenModule(module.ModuleId);

            courseViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ICourseViewModel.FormattedTimeRemaining))
                {
                    OnPropertyChanged(nameof(TimeSpent));
                }
            };

            courseService.OpenModule(module.ModuleId);
        }

        public void HandleModuleImageClick(object? obj)
        {
            var confirmStatus = courseService.ClickModuleImage(CurrentModule.ModuleId);
            if (confirmStatus)
            {
                OnPropertyChanged(nameof(CoinBalance));
            }
        }

        public string TimeSpent => courseViewModel.FormattedTimeRemaining;

        public int CoinBalance
        {
            get => coinsService.GetCoinBalance(0);
        }

        private bool CanCompleteModule(object parameter)
        {
            return !IsCompleted;
        }

        private void ExecuteCompleteModule(object parameter)
        {
            courseViewModel.MarkModuleAsCompletedAndCheckRewards(CurrentModule.ModuleId);
            IsCompleted = true;
            OnPropertyChanged(nameof(IsCompleted));
        }

        public void ExecuteModuleImageClick(object? obj)
        {
            if (courseService.ClickModuleImage(CurrentModule.ModuleId))
            {
                OnPropertyChanged(nameof(CoinBalance));
            }
        }
    }
}
