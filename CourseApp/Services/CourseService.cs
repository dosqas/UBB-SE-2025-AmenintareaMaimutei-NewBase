using System;
using System.Collections.Generic;
using System.Linq;
using CourseApp.Models;
using CourseApp.Repository;

namespace CourseApp.Services
{
    public class CourseService
    {
        private readonly CourseRepository repository;
        private const int UserId = 0;
        public CourseService()
        {
            repository = new CourseRepository();
        }

        public List<Course> GetCourses()
        {
            return repository.GetAllCourses();
        }

        public List<Tag> GetTags()
        {
            return repository.GetAllTags();
        }

        public List<Module> GetModules(int courseId)
        {
            return repository.GetModulesByCourseId(courseId);
        }

        public bool IsUserEnrolled(int courseId)
        {
            return repository.IsUserEnrolled(UserId, courseId);
        }

        public bool IsModuleCompleted(int moduleId)
        {
            return repository.IsModuleCompleted(UserId, moduleId);
        }

        public void EnrollInCourse(int courseId)
        {
            repository.EnrollUser(UserId, courseId);
        }

        public void CompleteModule(int moduleId)
        {
            repository.CompleteModule(UserId, moduleId);
        }

        /// <summary>
        /// Returns the courses filtered by search text, course type, enrollment status and selected tags.
        /// </summary>
        public List<Course> GetFilteredCourses(string searchText, bool filterPremium, bool filterFree, bool filterEnrolled, bool filterNotEnrolled, List<int> selectedTagIds)
        {
            // Start with all courses.
            var courses = repository.GetAllCourses();

            // Filter by search text (course title)
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                courses = courses
                    .Where(c => c.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            // Filter by course type.
            if (filterPremium && filterFree)
            {
                // No course can be both premium and free.
                courses = new List<Course>();
            }
            else if (filterPremium)
            {
                courses = courses.Where(c => c.IsPremium).ToList();
            }
            else if (filterFree)
            {
                courses = courses.Where(c => !c.IsPremium).ToList();
            }

            // Filter by enrollment status.
            if (filterEnrolled && filterNotEnrolled)
            {
                // No course can be both enrolled and not enrolled.
                courses = new List<Course>();
            }
            else if (filterEnrolled)
            {
                courses = courses.Where(c => repository.IsUserEnrolled(UserId, c.CourseId)).ToList();
            }
            else if (filterNotEnrolled)
            {
                courses = courses.Where(c => !repository.IsUserEnrolled(UserId, c.CourseId)).ToList();
            }

            // Filter by tags: Only courses having all selected tags will be kept.
            if (selectedTagIds.Any())
            {
                courses = courses.Where(c =>
                {
                    var courseTagIds = repository.GetTagsForCourse(c.CourseId)
                                        .Select(t => t.TagId)
                                        .ToList();
                    return selectedTagIds.All(id => courseTagIds.Contains(id));
                }).ToList();
            }

            return courses;
        }

        public void UpdateTimeSpent(int courseId, int seconds)
        {
            repository.UpdateTimeSpent(UserId, courseId, seconds);
        }

        public int GetTimeSpent(int courseId)
        {
            return repository.GetTimeSpent(UserId, courseId);
        }

    }
}
