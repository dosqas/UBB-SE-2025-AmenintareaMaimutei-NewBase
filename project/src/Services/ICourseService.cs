using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseApp.Models;

namespace CourseApp.Services
{
    public interface ICourseService
    {
        List<Course> GetCourses();
        List<Module> GetModules(int courseId);
        List<Module> GetNormalModules(int courseId);
        List<Tag> GetTags();
        List<Tag> GetCourseTags(int courseId);

        bool IsUserEnrolled(int courseId);
        bool EnrollInCourse(int courseId);
        bool IsModuleCompleted(int moduleId);
        void CompleteModule(int moduleId, int courseId);
        void OpenModule(int moduleId);

        bool BuyBonusModule(int moduleId, int courseId);

        List<Course> GetFilteredCourses(string searchText, bool filterPremium, bool filterFree, bool filterEnrolled, bool filterNotEnrolled, List<int> selectedTagIds);

        void UpdateTimeSpent(int courseId, int seconds);
        int GetTimeSpent(int courseId);

        bool ClickModuleImage(int moduleId);
        bool IsModuleInProgress(int moduleId);
        bool IsModuleAvailable(int moduleId);
        bool IsCourseCompleted(int courseId);

        int GetCompletedModulesCount(int courseId);
        int GetRequiredModulesCount(int courseId);

        bool ClaimCompletionReward(int courseId);
        bool ClaimTimedReward(int courseId, int timeSpent);
        int GetCourseTimeLimit(int courseId);
    }
}
