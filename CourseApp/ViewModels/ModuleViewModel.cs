using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;

namespace CourseApp.ViewModels
{
    public class ModuleViewModel : BaseViewModel
    {
        private readonly CourseService courseService;
        public Module CurrentModule { get; set; }
        public bool IsCompleted { get; set; }
        public ICommand CompleteModuleCommand { get; set; }

        public ModuleViewModel(Models.Module module)
        {
            courseService = new CourseService();
            CurrentModule = module;
            IsCompleted = courseService.IsModuleCompleted(module.ModuleId);
            CompleteModuleCommand = new RelayCommand(ExecuteCompleteModule, CanCompleteModule);
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
