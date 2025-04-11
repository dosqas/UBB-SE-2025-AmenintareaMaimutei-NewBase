using System.Diagnostics.CodeAnalysis;

namespace CourseApp.Models
{
    [ExcludeFromCodeCoverage]
    public class User
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
