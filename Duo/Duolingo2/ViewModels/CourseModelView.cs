using System;
using System.Collections.Generic;

namespace Duo
{
    public class CourseModelView
    {
        public Course Course { get; }
        public bool IsEnrolled { get; private set; }
        public List<Topic> Topics => Course.Topics;

        public CourseModelView(Course course)
        {
            Course = course;
            IsEnrolled = false;
        }

        public void UpdateEnrollmentStatus(int userId)
        {
            IsEnrolled = userId >= 0 ? Course.IsUserEnrolled() : false;
        }
    }
}