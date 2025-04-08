using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;

namespace CourseApp.ViewModels
{
    public partial class ModuleViewModel : BaseViewModel
    {
        private readonly IModuleCompletionService moduleCompletionService;
        private readonly ICoinsService coinsService;
        private readonly ICourseViewModel courseViewModel;

        public Module CurrentModule { get; }
        public bool IsCompleted { get; private set; }
        public ICommand CompleteModuleCommand { get; }
        public ICommand OnModuleImageClick { get; }

        public ModuleViewModel(Module module, ICourseViewModel courseVM,
                             IModuleCompletionService moduleCompletionService,
                             ICoinsService coinsService)
        {
            CurrentModule = module;
            courseViewModel = courseVM;
            this.moduleCompletionService = moduleCompletionService;
            this.coinsService = coinsService;

            IsCompleted = this.moduleCompletionService.IsModuleCompleted(module.ModuleId);

            CompleteModuleCommand = new RelayCommand(ExecuteCompleteModule, CanCompleteModule);
            OnModuleImageClick = new RelayCommand(ExecuteModuleImageClick);

            this.moduleCompletionService.OpenModule(module.ModuleId);

            courseViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ICourseViewModel.FormattedTimeRemaining))
                {
                    OnPropertyChanged(nameof(TimeSpent));
                }
            };
        }

        public string TimeSpent => courseViewModel.FormattedTimeRemaining;
        public int CoinBalance => coinsService.GetUserCoins(0);

        private bool CanCompleteModule(object parameter) => !IsCompleted;

        private void ExecuteCompleteModule(object parameter)
        {
            courseViewModel.MarkModuleAsCompletedAndCheckRewards(CurrentModule.ModuleId);
            IsCompleted = true;
            OnPropertyChanged(nameof(IsCompleted));
        }

        public void ExecuteModuleImageClick(object? obj)
        {
            if (moduleCompletionService.ClickModuleImage(CurrentModule.ModuleId))
            {
                OnPropertyChanged(nameof(CoinBalance));
            }
        }
    }
}