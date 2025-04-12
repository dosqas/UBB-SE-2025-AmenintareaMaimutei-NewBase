using System;
using System.Collections.Generic;
using System.Linq;
using CourseApp.Models;
using CourseApp.Repository;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly

namespace CourseApp.Services
{
    public class CourseService : ICourseService
    {
        public List<Tag> GetCourseTags(int courseId)
        {
            return repository.GetTagsForCourse(courseId);
        }
        private readonly ICourseRepository repository;
        private readonly ICoinsRepository coinsRepository = new CoinsRepository(new UserWalletModelView());
        private const int UserId = 0;
        public CourseService(ICourseRepository? courseRepository = null)
        {
            repository = courseRepository ?? new CourseRepository();
        }

        public CourseService(ICourseRepository repository, ICoinsRepository coinsRepository)
        {
            this.repository = repository;
            this.coinsRepository = coinsRepository;
        }

        public void OpenModule(int moduleId)
        {
            if (!repository.IsModuleOpen(UserId, moduleId))
            {
                repository.OpenModule(UserId, moduleId);
            }
        }
        public List<Course> GetCourses()
        {
            return repository.GetAllCourses();
        }

        public List<Tag> GetTags()
        {
            return repository.GetAllTags();
        }

        public bool BuyBonusModule(int moduleId, int courseId)
        {
            var module = repository.GetModule(moduleId);
            if (module == null || !module.IsBonus)
            {
                return false;
            }
            if (repository.IsModuleOpen(UserId, moduleId))
            {
                return false;
            }
            var course = repository.GetCourse(courseId);
            if (course == null)
            {
                return false;
            }
            if (!coinsRepository.TryDeductCoinsFromUserWallet(UserId, module.Cost))
            {
                return false;
            }
            repository.OpenModule(UserId, moduleId);
            return true;
        }

        public List<Module> GetNormalModules(int courseId)
        {
            return [.. repository.GetModulesByCourseId(courseId).Where(m => !m.IsBonus)];
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

        public bool EnrollInCourse(int courseId)
        {
            if (repository.IsUserEnrolled(UserId, courseId))
            {
                return false;
            }
            var course = repository.GetCourse(courseId);
            if (course == null)
            {
                return false;
            }
            if (course.IsPremium)
            {
                int cost = course.Cost;
                if (!coinsRepository.TryDeductCoinsFromUserWallet(UserId, cost))
                {
                    return false;
                }
            }
            repository.EnrollUser(UserId, courseId);
            return true;
        }

        public void CompleteModule(int moduleId, int courseId)
        {
            repository.CompleteModule(UserId, moduleId);

            if (repository.IsCourseCompleted(UserId, courseId))
            {
                repository.MarkCourseAsCompleted(UserId, courseId);
            }
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
                courses = [.. courses.Where(c => c.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))];
            }

            // Filter by course type.
            if (filterPremium && filterFree)
            {
                // No course can be both premium and free.
                courses = [];
            }
            else if (filterPremium)
            {
                courses = [.. courses.Where(c => c.IsPremium)];
            }
            else if (filterFree)
            {
                courses = [.. courses.Where(c => !c.IsPremium)];
            }

            // Filter by enrollment status.
            if (filterEnrolled && filterNotEnrolled)
            {
                // No course can be both enrolled and not enrolled.
                courses = [];
            }
            else if (filterEnrolled)
            {
                courses = [.. courses.Where(c => repository.IsUserEnrolled(UserId, c.CourseId))];
            }
            else if (filterNotEnrolled)
            {
                courses = [.. courses.Where(c => !repository.IsUserEnrolled(UserId, c.CourseId))];
            }

            // Filter by tags: Only courses having all selected tags will be kept.
            if (selectedTagIds.Count != 0)
            {
                courses = [.. courses.Where(c =>
                {
                    var courseTagIds = repository.GetTagsForCourse(c.CourseId)
                                        .Select(t => t.TagId)
                                        .ToList();
                    return selectedTagIds.All(id => courseTagIds.Contains(id));
                })];
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

        public bool ClickModuleImage(int moduleId)
        {
            if (repository.IsModuleImageClicked(UserId, moduleId))
            {
                return false;
            }

            repository.ClickModuleImage(UserId, moduleId);
            coinsRepository.AddCoinsToUserWallet(UserId, 10);
            return true;
        }

        public bool IsModuleInProgress(int moduleId)
        {
            return repository.IsModuleInProgress(0, moduleId);
        }

        public bool IsModuleAvailable(int moduleId)
        {
            return repository.IsModuleAvailable(UserId, moduleId);
        }

        public bool IsCourseCompleted(int courseId)
        {
            return repository.IsCourseCompleted(UserId, courseId);
        }

        public int GetCompletedModulesCount(int courseId)
        {
            return repository.GetCompletedModulesCount(UserId, courseId);
        }

        public int GetRequiredModulesCount(int courseId)
        {
            return repository.GetRequiredModulesCount(courseId);
        }

        public bool ClaimCompletionReward(int courseId)
        {
            bool claimed = repository.ClaimCompletionReward(UserId, courseId);
            if (claimed)
            {
                coinsRepository.AddCoinsToUserWallet(UserId, 50);
            }
            return claimed;
        }

        public bool ClaimTimedReward(int courseId, int timeSpent)
        {
            int timeLimit = repository.GetCourseTimeLimit(courseId);
            bool claimed = repository.ClaimTimedReward(UserId, courseId, timeSpent, timeLimit);

            if (claimed)
            {
                int rewardAmount = 300; // hardcoded reward for timed completion
                coinsRepository.AddCoinsToUserWallet(UserId, rewardAmount);
            }

            return claimed;
        }

        public int GetCourseTimeLimit(int courseId)
        {
            return repository.GetCourseTimeLimit(courseId);
        }
    }
}