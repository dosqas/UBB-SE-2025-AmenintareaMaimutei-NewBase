using System.Collections.ObjectModel;
using System.Windows.Input;
using CourseApp.Models;

public interface IMainViewModel : IBaseViewModel
{
    ObservableCollection<Course> DisplayedCourses { get; }
    ObservableCollection<Tag> AvailableTags { get; }
    int UserCoinBalance { get; }
    string SearchQuery { get; set; }
    bool FilterByPremium { get; set; }
    bool FilterByFree { get; set; }
    bool FilterByEnrolled { get; set; }
    bool FilterByNotEnrolled { get; set; }
    ICommand ResetAllFiltersCommand { get; }

    bool TryDailyLoginReward();
}