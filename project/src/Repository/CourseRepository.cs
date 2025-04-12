using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CourseApp.Models;
using CourseApp.ModelViews;

namespace CourseApp.Repository
{
    [ExcludeFromCodeCoverage]
    public class CourseRepository : ICourseRepository
    {
        private readonly CourseModelView courseModelView;
        private readonly ModuleModelView moduleModelView;
        private readonly EnrollmentModelView enrollmentModelView;
        private readonly TagModelView tagModelView;
        private readonly ProgressModelView progressModelView;
        private readonly RewardModelView rewardModelView;

        public CourseRepository()
        {
            courseModelView = new CourseModelView();
            moduleModelView = new ModuleModelView();
            enrollmentModelView = new EnrollmentModelView();
            tagModelView = new TagModelView();
            progressModelView = new ProgressModelView();
            rewardModelView = new RewardModelView();
        }

        // Course operations
        public Course? GetCourse(int courseId) => CourseModelView.GetCourse(courseId);
        public List<Course> GetAllCourses() => CourseModelView.GetAllCourses();

        // Module operations
        public Module? GetModule(int moduleId) => ModuleModelView.GetModule(moduleId);
        public List<Module> GetModulesByCourseId(int courseId)
        {
            return ModuleModelView.GetModulesByCourseId(courseId);
        }

        public bool IsModuleAvailable(int userId, int moduleId) => ModuleModelView.IsModuleAvailable(userId, moduleId);

        public bool IsModuleOpen(int userId, int moduleId) =>
            ModuleModelView.IsModuleOpen(userId, moduleId);

        public bool IsModuleCompleted(int userId, int moduleId) =>
            ModuleModelView.IsModuleCompleted(userId, moduleId);

        public bool IsModuleInProgress(int userId, int moduleId) =>
            ModuleModelView.IsModuleInProgress(userId, moduleId);

        public bool IsModuleImageClicked(int userId, int moduleId) =>
            ModuleModelView.IsModuleImageClicked(userId, moduleId);

        public void OpenModule(int userId, int moduleId) =>
            ModuleModelView.OpenModule(userId, moduleId);

        public void CompleteModule(int userId, int moduleId) =>
            ModuleModelView.CompleteModule(userId, moduleId);

        public void ClickModuleImage(int userId, int moduleId) =>
            ModuleModelView.ClickModuleImage(userId, moduleId);

        // Enrollment operations
        public bool IsUserEnrolled(int userId, int courseId) =>
            EnrollmentModelView.IsUserEnrolled(userId, courseId);

        public void EnrollUser(int userId, int courseId) =>
            EnrollmentModelView.EnrollUser(userId, courseId);

        // Progress tracking
        public void UpdateTimeSpent(int userId, int courseId, int seconds) =>
            ProgressModelView.UpdateTimeSpent(userId, courseId, seconds);

        public int GetTimeSpent(int userId, int courseId) =>
            ProgressModelView.GetTimeSpent(userId, courseId);

        public int GetRequiredModulesCount(int courseId) =>
            ProgressModelView.GetRequiredModulesCount(courseId);

        public int GetCompletedModulesCount(int userId, int courseId) =>
            ProgressModelView.GetCompletedModulesCount(userId, courseId);

        public bool IsCourseCompleted(int userId, int courseId) =>
            ProgressModelView.IsCourseCompleted(userId, courseId);

        public void MarkCourseAsCompleted(int userId, int courseId) =>
            ProgressModelView.MarkCourseAsCompleted(userId, courseId);

        // Reward operations
        public bool ClaimCompletionReward(int userId, int courseId) =>
            RewardModelView.ClaimCompletionReward(userId, courseId);

        public bool ClaimTimedReward(int userId, int courseId, int timeSpent, int timeLimit) =>
            RewardModelView.ClaimTimedReward(userId, courseId, timeSpent, timeLimit);

        public int GetCourseTimeLimit(int courseId) =>
            RewardModelView.GetCourseTimeLimit(courseId);

        // Tag operations
        public List<Tag> GetAllTags() => TagModelView.GetAllTags();
        public List<Tag> GetTagsForCourse(int courseId) => TagModelView.GetTagsForCourse(courseId);
    }
}