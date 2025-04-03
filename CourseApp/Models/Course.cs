namespace CourseApp.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPremium { get; set; }
        public int Cost { get; set; }
        public string ImageUrl { get; set; }
        public int TimeToComplete { get; set; } // in seconds
        public string Difficulty { get; set; }
    }
}
