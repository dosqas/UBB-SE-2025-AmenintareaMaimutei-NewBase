using System;
using System.Collections.Generic;
using CourseApp.Models;

namespace CourseApp.Repository
{
    /// <summary>
    /// Interface for course-related data operations
    /// </summary>
    public interface ICourseRepository
    {
        // Course operations
        Course? GetCourse(int courseId);
        List<Course> GetAllCourses();

        // Module operations
        Module? GetModule(int moduleId);
        List<Module> GetModulesByCourseId(int courseId);
        bool IsModuleAvailable(int userId, int moduleId);
        bool IsModuleOpen(int userId, int moduleId);
        bool IsModuleCompleted(int userId, int moduleId);
        bool IsModuleInProgress(int userId, int moduleId);
        bool IsModuleImageClicked(int userId, int moduleId);
        void OpenModule(int userId, int moduleId);
        void CompleteModule(int userId, int moduleId);
        void ClickModuleImage(int userId, int moduleId);

        // Enrollment operations
        bool IsUserEnrolled(int userId, int courseId);
        void EnrollUser(int userId, int courseId);

        // Progress tracking
        void UpdateTimeSpent(int userId, int courseId, int seconds);
        int GetTimeSpent(int userId, int courseId);
        int GetRequiredModulesCount(int courseId);
        int GetCompletedModulesCount(int userId, int courseId);
        bool IsCourseCompleted(int userId, int courseId);
        void MarkCourseAsCompleted(int userId, int courseId);

        // Reward operations
        bool ClaimCompletionReward(int userId, int courseId);
        bool ClaimTimedReward(int userId, int courseId, int timeSpent, int timeLimit);
        int GetCourseTimeLimit(int courseId);

        // Tag operations
        List<Tag> GetAllTags();
        List<Tag> GetTagsForCourse(int courseId);
    }
}