using System;

namespace CourseApp.Models
{
    public class CourseCompletion
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public bool CompletionRewardClaimed { get; set; }
        public bool TimedRewardClaimed { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}