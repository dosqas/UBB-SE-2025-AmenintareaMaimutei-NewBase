//E un CursModelView ajutator

using System.Collections.Generic;

namespace Duolingo2.Models
{
    public class CourseModelView
    {
        public Course Course { get; }
        public bool IsEnrolled { get; set; }

        public List<Topic> Topics => Course.Topics;

        public CourseModelView(Course course)
        {
            Course = course;
        }
    }
}
