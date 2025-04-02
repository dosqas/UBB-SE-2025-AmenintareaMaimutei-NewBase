using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;
using Windows.UI.Popups;

namespace CourseApp.ViewModels
{
    public class ModuleViewModel : BaseViewModel
    {
        private readonly CourseService courseService;
        public Module CurrentModule { get; set; }
        public bool IsCompleted { get; set; }
        public ICommand CompleteModuleCommand { get; set; }
        public ICommand ModuleImageClick { get; }

        public ModuleViewModel(Models.Module module)
        {
            courseService = new CourseService();
            CurrentModule = module;
            IsCompleted = courseService.IsModuleCompleted(module.ModuleId);
            CompleteModuleCommand = new RelayCommand(ExecuteCompleteModule, CanCompleteModule);
            ModuleImageClick = new RelayCommand(OnModuleImageClick);
        }

        public string TimeSpent
        {
            get => "24 minutes";
            set
            {
                //TODO Implement the TimeSpent property
            }
        }

        public int CoinBalance
        {
            get => 100;
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
