using System;

namespace CourseApp.Models
{
    public class Enrollment
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public int TimeSpent { get; set; } // in seconds
        public bool IsCompleted { get; set; }
    }
}
