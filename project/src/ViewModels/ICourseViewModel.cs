using System.Collections.ObjectModel;
using System.Windows.Input;
using CourseApp.Models;
using static CourseApp.ViewModels.CourseViewModel;

public interface ICourseViewModel : IBaseViewModel
{
    Course CurrentCourse { get; }
    ObservableCollection<ModuleProgressStatus> ModuleRoadmap { get; }
    ICommand? EnrollCommand { get; }
    bool IsEnrolled { get; }
    bool CoinVisibility { get; }
    int CoinBalance { get; }
    ObservableCollection<Tag> Tags { get; }
    string FormattedTimeRemaining { get; }
    string NotificationMessage { get; }
    bool ShowNotification { get; }
    int CompletedModules { get; }
    int RequiredModules { get; }
    bool IsCourseCompleted { get; }
    int TimeLimit { get; }
    int TimeRemaining { get; }
    bool CompletionRewardClaimed { get; }
    bool TimedRewardClaimed { get; }

    void StartCourseProgressTimer();
    void PauseCourseProgressTimer();
    void RefreshCourseModulesDisplay();
    void MarkModuleAsCompletedAndCheckRewards(int targetModuleId);
    void AttemptBonusModulePurchase(Module module);
    void LoadAndOrganizeCourseModules();
}