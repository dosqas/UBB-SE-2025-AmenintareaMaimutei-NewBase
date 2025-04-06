namespace CourseApp.Models
{
    public class Module
    {
        public int ModuleId { get; set; }
        public int CourseId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int Position { get; set; }
        public bool IsBonus { get; set; }
        public int Cost { get; set; }
        public string ShortDescription => Description.Length > 23 ? Description.Substring(0, 23) + "..." : Description;
        public required string ImageUrl { get; set; }
    }
}
