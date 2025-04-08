using System.Collections.Generic;
using CourseApp.Models;

namespace CourseApp.Services
{
    public interface ICourseFilterService
    {
        List<Course> GetFilteredCourses(
            string searchText,
            bool filterPremium,
            bool filterFree,
            bool filterEnrolled,
            bool filterNotEnrolled,
            List<int> selectedTagIds);
    }
}