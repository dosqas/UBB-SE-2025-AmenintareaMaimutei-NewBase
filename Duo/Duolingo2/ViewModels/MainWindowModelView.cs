using System.Collections.Generic;
using System.Linq;
using Duo.Models;
using Duo.ViewModels;

namespace Duo
{
    public class MainWindowModelView
    {
        private List<CourseModelView> _allCourses;

        public List<CourseModelView> CourseViews { get; private set; }

        private List<Tag> _appliedTags = new();
        private bool _filterEnrolled = false;
        private bool _filterFreeOnly = false;
        private int _userId = 1;
        private string _search = "";
        public int UserCoins { get; } = 100;

        public MainWindowModelView()
        {
            _allCourses = new List<CourseModelView>
            {
                new CourseModelView(new CourseForTest(1, "Intro to C#", true, new List<Tag> { new Tag("Programming"), new Tag("C#") }, "Assets/csharp.jpeg")) { IsEnrolled = true },
                new CourseModelView(new CourseForTest(2, "Advanced C#", false, new List<Tag> { new Tag("Programming"), new Tag("C#") }, "Assets/csharp.jpeg")) { IsEnrolled = false },
                new CourseModelView(new CourseForTest(3, "Intro to Python", true, new List<Tag> { new Tag("Python") }, "Assets/python.jpg")) { IsEnrolled = true },
                new CourseModelView(new CourseForTest(4, "Data Structures", false, new List<Tag> { new Tag("Algorithms") }, "Assets/algorithms.jpeg")) { IsEnrolled = false },
                new CourseModelView(new CourseForTest(5, "Machine Learning", false, new List<Tag> { new Tag("Python"), new Tag("ML") }, "Assets/ml.png")) { IsEnrolled = true },
            };


            CourseViews = new List<CourseModelView>(_allCourses);
        }

        public void SetSearchKeyword(string keyword)
        {
            _search = keyword?.Trim() ?? "";
            FilterAll();
        }

        public void ApplyFilters(List<Tag> Tags, bool enrolled, bool notEnrolled, bool freeOnly, bool paidOnly, int userId)
        {
            //logic for filtering in service
        }



        public void ClearFilters()
        {
            _appliedTags.Clear();
            _filterEnrolled = false;
            _filterFreeOnly = false;
            _search = "";
            _userId = 1;
            //logic for clearing filters in service
        }

        private void FilterAll()
        {
            //logic for filtering in service
        }
    }
}