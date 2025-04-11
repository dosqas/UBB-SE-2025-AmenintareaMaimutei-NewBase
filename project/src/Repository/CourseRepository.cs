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
        public Course? GetCourse(int courseId) => courseModelView.GetCourse(courseId);
        public List<Course> GetAllCourses() => courseModelView.GetAllCourses();

        // Module operations
        public Module? GetModule(int moduleId) => moduleModelView.GetModule(moduleId);
        public List<Module> GetModulesByCourseId(int courseId) => moduleModelView.GetModulesByCourseId(courseId);

        public bool IsModuleAvailable(int userId, int moduleId) => moduleModelView.IsModuleAvailable(userId, moduleId);

        public bool IsModuleOpen(int userId, int moduleId) =>
            moduleModelView.IsModuleOpen(userId, moduleId);

        public bool IsModuleCompleted(int userId, int moduleId) =>
            moduleModelView.IsModuleCompleted(userId, moduleId);

        public bool IsModuleInProgress(int userId, int moduleId) =>
            moduleModelView.IsModuleInProgress(userId, moduleId);

        public bool IsModuleImageClicked(int userId, int moduleId) =>
            moduleModelView.IsModuleImageClicked(userId, moduleId);

        public void OpenModule(int userId, int moduleId) =>
            moduleModelView.OpenModule(userId, moduleId);

        public void CompleteModule(int userId, int moduleId) =>
            moduleModelView.CompleteModule(userId, moduleId);

        public void ClickModuleImage(int userId, int moduleId) =>
            moduleModelView.ClickModuleImage(userId, moduleId);

        // Enrollment operations
        public bool IsUserEnrolled(int userId, int courseId) =>
            enrollmentModelView.IsUserEnrolled(userId, courseId);

        public void EnrollUser(int userId, int courseId) =>
            enrollmentModelView.EnrollUser(userId, courseId);

        // Progress tracking
        public void UpdateTimeSpent(int userId, int courseId, int seconds) =>
            progressModelView.UpdateTimeSpent(userId, courseId, seconds);

        public int GetTimeSpent(int userId, int courseId) =>
            progressModelView.GetTimeSpent(userId, courseId);

        public int GetRequiredModulesCount(int courseId) =>
            progressModelView.GetRequiredModulesCount(courseId);

        public int GetCompletedModulesCount(int userId, int courseId) =>
            progressModelView.GetCompletedModulesCount(userId, courseId);

        public bool IsCourseCompleted(int userId, int courseId) =>
            progressModelView.IsCourseCompleted(userId, courseId);

        public void MarkCourseAsCompleted(int userId, int courseId) =>
            progressModelView.MarkCourseAsCompleted(userId, courseId);

        // Reward operations
        public bool ClaimCompletionReward(int userId, int courseId) =>
            rewardModelView.ClaimCompletionReward(userId, courseId);

        public bool ClaimTimedReward(int userId, int courseId, int timeSpent, int timeLimit) =>
            rewardModelView.ClaimTimedReward(userId, courseId, timeSpent, timeLimit);

        public int GetCourseTimeLimit(int courseId) =>
            rewardModelView.GetCourseTimeLimit(courseId);

        // Tag operations
        public List<Tag> GetAllTags() => tagModelView.GetAllTags();
        public List<Tag> GetTagsForCourse(int courseId) => tagModelView.GetTagsForCourse(courseId);
    }
}