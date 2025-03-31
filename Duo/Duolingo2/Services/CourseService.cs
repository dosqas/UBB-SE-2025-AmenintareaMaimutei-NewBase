using Duo.Models;
using Duo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Some methods have as parameter userId, do not worry about it, by default it is 0, you do not need to give a value, just call the method without it
// ex. for method1(int courseId, int userId = 0) you would simply call it by method1(courseId)


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


        // Daily Login Reward method added here in CourseService, move at your own will

        public void GrantUserDailyReward(int userId = 0)
        {
            if(_coinRepository.CheckDailyLoginEligibility(userId).Result)
            {
                _coinRepository.SetUserCoinBalanceAsync(userId, _coinRepository.GetCoinsByUserIdAsync(userId).Result + 100); //hardcoded value for daily reward system
            }
        }

        // Daily Login Reward method added here in CourseService, move at your own will


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
        }//TODO May God have mercy on you if you got this

        public Pair GetCompletionStatus(int courseId, int userId = 0)
        {
            Course course = _courseRepository.GetCourseByIdAsync(courseId).Result;
            nrOfModules = course.Modules.Count;
            Pair pair = new Pair();
            int completedModules = 0;
            foreach (var module in course.Modules)
            {
                if (_moduleRepository.IsModuleCompletedAsync(userId, courseId, module.ModuleId).Result && !module.IsBonusModule)
                {
                    completedModules++;
                }
                if(module.IsBonusModule)
                {
                    nrOfModules--;
                }
            }
            pair.First = completedModules;
            pair.Second = nrOfModules;
            return pair;
        }

        public bool IsCourseCompleted(int courseId)
        {
            Course course = _courseRepository.GetCourseByIdAsync(courseId).Result;
            foreach (var module in course.Modules)
            {

                if (!_moduleRepository.IsModuleCompletedAsync(currentUserId, courseId, module.ModuleId).Result && !module.IsBonusModule)

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


        public int GetUserCourseTimer(int courseId, int userId = 0)
        {
            return _courseRepository.GetUserCourseTimer(courseId, userId).Result;
        }

        public void SetUserCourseTimer(int courseId, int userId, int timer)
        {
            _courseRepository.SetUserCourseTimer(courseId, userId, timer);
        }

        public bool IsUserWithinTimer(int courseId, int userId = 0)
        {
            int timer = GetUserCourseTimer(courseId, userId);
            int courseTimer = _courseRepository.GetCourseByIdAsync(courseId).Result.TimerDurationSeconds;
            if (timer < courseTimer)
            {
                return true;
            }
            return false;
        }

        public void GiveTimerCompletionReward(int courseId, int userId = 0)
        {
            Course course = _courseRepository.GetCourseByIdAsync(courseId).Result;
            int userCoins = _coinRepository.GetCoinsByUserIdAsync(userId).Result;
            _coinRepository.SetUserCoinBalanceAsync(currentUserId, userCoins + course.TimerCompletionReward);
        }

    }
}
