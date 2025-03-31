using Duo.Models;
using Duo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Still needed(not neccessarily in this class): TimeCompletionReward, DailyLoginReward, get/set Course Timer, 

namespace Duo.Services
{
    public class CourseService
    {
        private readonly CourseRepository _courseRepository;
        private readonly ModuleRepository _moduleRepository;
        private readonly CoinRepository _coinRepository;
        private readonly int currentUserId;

        public CourseService(CourseRepository courseRepository, ModuleRepository moduleRepository, CoinRepository coinRepository)
        {
            _courseRepository = courseRepository;
            _moduleRepository = moduleRepository;
            _coinRepository = coinRepository;
            this.currentUserId = 0;
        }

        public List<Course> GetAllCourses()
        {
            var courses = _courseRepository.GetAllCoursesAsync().Result;
            return courses;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tagFilters">List of the active tags excluding CourseType and EnrollmentStatus</param>
        /// <param name="SearchTerm">String of whatever is inside the search input or "" otherwise</param>
        /// <param name="enrollmentFilter">4 possibilities: "None" "Enrolled" "Not Enrolled" "Both" - will return empty list for "Both", will not filter if "None"</param>
        /// <param name="TypeFilter">4 possibilities: "None" "Free" "Premium" "Both" - will return empty list for "Both", will not filter if "None"</param>
        /// <returns></returns>
        public List<Course> GetFilteredCourses(List<Tag> tagFilters, string searchTerm, string enrollmentFilter, string typeFilter)
        {
            var courses = _courseRepository.GetAllCoursesAsync().Result;
            if (typeFilter == "Both" || enrollmentFilter == "Both")
                return new List<Course>();

            if (typeFilter != "None")
            {
                courses = courses.Where(c => c.Type.TypeName == typeFilter).ToList();
            }

            if (enrollmentFilter != "None")
            {
                courses = courses.Where(c => c.IsEnrolled == (enrollmentFilter == "Enrolled")).ToList();
            }

            if (tagFilters.Count > 0)
            {
                courses = courses.Where(c => tagFilters.All(filter => c.Tags.Any(tag => tag.TagName == filter.TagName))).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courses = courses.Where(c => c.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return courses; // this method is untested. As is every other one for that matter. TODO: Try catch blocks for absolutely everything in Services, Repositories, DataConnection
        }// Also need to change return values of bool functions into void and use exceptions instead

        public bool EnrollUser(int courseId)
        {
            Course course = _courseRepository.GetCourseByIdAsync(courseId).Result;
            if (course != null)
            {
                if (course.Type.TypeName == "Premium")
                {
                    int courseCost = (int)course.Type.Price;
                    int userCoins = _coinRepository.GetCoinsByUserIdAsync(currentUserId).Result;

                    if (userCoins >= courseCost)
                    {
                        _coinRepository.SetUserCoinBalanceAsync(currentUserId, userCoins - courseCost);
                        return _courseRepository.EnrollUser(currentUserId, courseId).Result;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return _courseRepository.EnrollUser(currentUserId, courseId).Result;
                }
            }
            return false;
        }

        public bool IsCourseCompleted(int courseId)
        {
            Course course = _courseRepository.GetCourseByIdAsync(courseId).Result;
            foreach (var module in course.Modules)
            {
                if (!_moduleRepository.IsModuleCompletedAsync(currentUserId, courseId, module.ModuleId).Result)
                {
                    return false;
                }
            }
            return true;
        }

        public void GiveCompletionReward(int courseId)
        {
            Course course = _courseRepository.GetCourseByIdAsync(courseId).Result;
            int userCoins = _coinRepository.GetCoinsByUserIdAsync(currentUserId).Result;
            _coinRepository.SetUserCoinBalanceAsync(currentUserId, userCoins + course.CompletionReward);
        }
    }
}
