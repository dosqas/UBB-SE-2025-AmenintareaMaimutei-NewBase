using System;
using System.Collections.Generic;
using System.Linq;

namespace Duo
{
    public class MainWindowModelView
    {
        private readonly CourseService _courseService;
        public List<CourseModelView> CourseViews { get; private set; }
        public List<Topic> AllTopics { get; private set; }
        public List<Topic> AppliedFilters { get; private set; }
        public bool AppliedEnrolledFilter { get; private set; }
        public bool AppliedFreeCourseFilter { get; private set; }
        public int AppliedUserId { get; private set; }
        public string SearchKeyword { get; private set; }

        public MainWindowModelView(CourseService courseService)
        {
            _courseService = courseService;
            AllTopics = _courseService.GetAllTopics();
            AppliedFilters = new List<Topic>();
            AppliedEnrolledFilter = false;
            AppliedFreeCourseFilter = false;
            AppliedUserId = -1;
            SearchKeyword = "";
            CourseViews = new List<CourseModelView>();
            LoadCourses();
        }

        public void ApplyFilters(List<Topic> topics, bool enrolled, bool freeCourses, int userId)
        {
            AppliedFilters = topics ?? new List<Topic>();
            AppliedEnrolledFilter = enrolled;
            AppliedFreeCourseFilter = freeCourses;
            AppliedUserId = userId;
            LoadCourses();
        }

        public void SetSearchKeyword(string keyword)
        {
            SearchKeyword = keyword?.Length > 100 ? keyword.Substring(0, 100) : keyword ?? "";
            LoadCourses();
        }

        public void ClearFilters()
        {
            AppliedFilters.Clear();
            AppliedEnrolledFilter = false;
            AppliedFreeCourseFilter = false;
            AppliedUserId = -1;
            SearchKeyword = "";
            LoadCourses();
        }

        public void LoadCourses()
        {
            CourseViews = _courseService.GetCourses(AppliedFilters, SearchKeyword, AppliedUserId, AppliedEnrolledFilter, AppliedFreeCourseFilter);
            PrintCurrentResults();
        }

        public void PrintCurrentResults()
        {
            Console.WriteLine("\nCurrent Results:");
            Console.WriteLine($"Filters: Topics=[{string.Join(",", AppliedFilters.Select(t => t.Name))}], " +
                              $"Enrolled={AppliedEnrolledFilter} (User:{AppliedUserId}), " +
                              $"FreeOnly={AppliedFreeCourseFilter}, " +
                              $"Search='{SearchKeyword}'");

            if (!CourseViews.Any())
            {
                Console.WriteLine("No courses match the current filters");
                return;
            }

            foreach (var course in CourseViews)
            {
                Console.WriteLine($"- {course.Course.Name} " +
                                  $"(Enrolled: {course.IsEnrolled}, " +
                                  $"Free: {course.Course.IsFree}, " +
                                  $"Topics: {string.Join(",", course.Topics.Select(t => t.Name))}");
            }
        }
    }
}