using CourseApp.Models;

namespace CourseApp.Services
{
    public interface INotificationService
    {
        void ShowCourseCompletionReward();
        void ShowTimedCompletionReward();
        void ShowModulePurchaseNotification(Module module);
        void ShowPurchaseFailedNotification();
    }
}