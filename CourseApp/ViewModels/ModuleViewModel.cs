using CourseApp.Models;
using CourseApp.Services;
using System;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using Microsoft.UI.Xaml.Controls;
using CourseApp.Views;

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

        public ICommand OnModuleImageClick { get; set; }


        public ModuleViewModel(Models.Module module , CourseViewModel courseVM)
        {
            courseService = new CourseService();
            coinsService = new CoinsService();
            coinsService.GetUserCoins(0);
            CurrentModule = module;
            IsCompleted = courseService.IsModuleCompleted(module.ModuleId);
            CompleteModuleCommand = new RelayCommand(ExecuteCompleteModule, CanCompleteModule);
            OnModuleImageClick = new RelayCommand(ExecuteModuleImageClick);
            courseViewModel = courseVM;
            courseService.OpenModule(module.ModuleId);
            courseViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(courseViewModel.TimeSpent))
                {
                    OnPropertyChanged(nameof(TimeSpent));
                }
            };
            courseService.OpenModule(module.ModuleId);
        }

        private async void ExecuteModuleImageClick(object? obj)
        {

            bool confirmStatus = courseService.ClickModuleImage(CurrentModule.ModuleId);
            if(confirmStatus)
                OnPropertyChanged(nameof(CoinBalance));
        }
        public string TimeSpent => courseViewModel.TimeSpent;


        public int CoinBalance
        {
            get => coinsService.GetUserCoins(0);
        }

        private bool CanCompleteModule(object parameter)
        {
            return !IsCompleted;
        }

        private void ExecuteCompleteModule(object parameter)
        {
            // Mark module as complete in the database.
            this.courseViewModel.UpdateModuleCompletion(CurrentModule.ModuleId);
            IsCompleted = true;
            OnPropertyChanged(nameof(IsCompleted));
            courseViewModel.ReloadModules(); // Refresh roadmap to unlock the next module
        }
    }
}
