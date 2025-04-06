using System.Collections.Generic;

namespace CourseApp.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsPremium { get; set; }
        public int Cost { get; set; }
        public required string ImageUrl { get; set; }
        public int TimeToComplete { get; set; } // in seconds
        public required string Difficulty { get; set; }
    }
}
