using System.Collections.ObjectModel;
using System.Windows.Input;
using CourseApp.Models;
using CourseApp.Services;

namespace CourseApp.ViewModels
{
    public class CourseViewModel : BaseViewModel
    {
        private readonly CourseService courseService;
        public Course CurrentCourse { get; set; }
        public ObservableCollection<Models.Module> Modules { get; set; }
        public ICommand EnrollCommand { get; set; }
        public bool IsEnrolled { get; set; }

        public CourseViewModel(Course course)
        {
            courseService = new CourseService();
            CurrentCourse = course;
            Modules = new ObservableCollection<Models.Module>(courseService.GetModules(course.CourseId));
            IsEnrolled = courseService.IsUserEnrolled(course.CourseId);
            EnrollCommand = new RelayCommand(ExecuteEnroll, CanEnroll);
        }

        private bool CanEnroll(object parameter)
        {
            return !IsEnrolled;
        }

        private void ExecuteEnroll(object parameter)
        {
            // Empty implementation for enrollment functionality.
            courseService.EnrollInCourse(CurrentCourse.CourseId);
            IsEnrolled = true;
            OnPropertyChanged(nameof(IsEnrolled));
        }
    }
}
