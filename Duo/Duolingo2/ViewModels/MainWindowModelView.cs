//E un MainWindowModelView ajutator
using System.Collections.Generic;
using System.Linq;
using Duolingo2.Models;

namespace Duolingo2
{
    public class MainWindowModelView
    {
        private List<CourseModelView> _allCourses;

        public List<CourseModelView> CourseViews { get; private set; }

        private List<Topic> _appliedTopics = new();
        private bool _filterEnrolled = false;
        private bool _filterFreeOnly = false;
        private int _userId = 1;
        private string _search = "";
        public int UserCoins { get; } = 100;

        public MainWindowModelView()
        {
            _allCourses = new List<CourseModelView>
{
                new CourseModelView(new Course(1, "Intro to C#", true, new List<Topic> { new Topic("Programming"), new Topic("C#") }, "Assets/csharp.jpeg")) { IsEnrolled = true },
                new CourseModelView(new Course(2, "Advanced C#", false, new List<Topic> { new Topic("Programming"), new Topic("C#") }, "Assets/csharp.jpeg")) { IsEnrolled = false },
                new CourseModelView(new Course(3, "Intro to Python", true, new List<Topic> { new Topic("Python") }, "Assets/python.jpg")) { IsEnrolled = true },
                new CourseModelView(new Course(4, "Data Structures", false, new List<Topic> { new Topic("Algorithms") }, "Assets/algorithms.jpeg")) { IsEnrolled = false },
                new CourseModelView(new Course(5, "Machine Learning", false, new List<Topic> { new Topic("Python"), new Topic("ML") }, "Assets/ml.png")) { IsEnrolled = true },
            };


            CourseViews = new List<CourseModelView>(_allCourses);
        }

        public void SetSearchKeyword(string keyword)
        {
            _search = keyword?.Trim() ?? "";
            FilterAll();
        }

        public void ApplyFilters(List<Topic> topics, bool enrolled, bool notEnrolled, bool freeOnly, bool paidOnly, int userId)
        {
            _appliedTopics = topics ?? new();
            _filterEnrolled = enrolled;
            _filterFreeOnly = freeOnly;
            _userId = userId;

            var filtered = _allCourses.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(_search))
            {
                filtered = filtered.Where(c => c.Course.Name.Contains(_search, System.StringComparison.OrdinalIgnoreCase));
            }

            if (_appliedTopics.Any())
            {
                filtered = filtered.Where(c => c.Topics.Any(t => _appliedTopics.Any(sel => sel.Name == t.Name)));
            }

            if (enrolled)
            {
                filtered = filtered.Where(c => c.IsEnrolled);
            }
            else if (notEnrolled)
            {
                filtered = filtered.Where(c => !c.IsEnrolled);
            }

            if (freeOnly)
            {
                filtered = filtered.Where(c => c.Course.IsFree);
            }
            else if (paidOnly)
            {
                filtered = filtered.Where(c => !c.Course.IsFree);
            }

            CourseViews = filtered.ToList();
        }



        public void ClearFilters()
        {
            _appliedTopics.Clear();
            _filterEnrolled = false;
            _filterFreeOnly = false;
            _search = "";
            _userId = 1;

            CourseViews = new List<CourseModelView>(_allCourses);
        }

        private void FilterAll()
        {
            IEnumerable<CourseModelView> results = _allCourses;

            if (!string.IsNullOrWhiteSpace(_search))
            {
                results = results.Where(c => c.Course.Name.Contains(_search, System.StringComparison.OrdinalIgnoreCase));
            }

            if (_appliedTopics.Any())
            {
                results = results.Where(c => c.Topics.Any(t => _appliedTopics.Any(sel => sel.Name == t.Name)));
            }

            if (_filterEnrolled)
            {
                results = results.Where(c => c.IsEnrolled);
            }

            if (_filterFreeOnly)
            {
                results = results.Where(c => c.Course.IsFree);
            }

            CourseViews = results.ToList();
        }
    }
}