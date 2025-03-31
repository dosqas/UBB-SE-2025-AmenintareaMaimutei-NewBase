using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duo.Models
{
    public class Course
    {
        public int Id { get; }

        public List<Module> Modules { get; set; }

        public string Title { get; }
        public string Description { get; }
        public List<Tag> Tags { get; }
        public string ImagePath { get; }
        public bool IsEnrolled { get; set; }
        public Type Type { get; }
        public DateTime CreatedAt { get; }
        public int DifficultyLevel { get; }
        public int TimerDurationMinutes { get; }
        public decimal TimerCompletionReward { get; }
        public decimal CompletionReward { get; }

        public Course(int id, string title, string description, List<Tag> tags, string imagePath, bool isEnrolled, Type type, DateTime createdAt, int difficultyLevel, int timerDurationMinutes, decimal timerCompletionReward, decimal completionReward, List<Module> modules)
        {
            Id = id;
            Title = title;
            Description = description;
            Tags = tags;
            ImagePath = imagePath;
            IsEnrolled = isEnrolled;
            Type = type;
            CreatedAt = createdAt;
            DifficultyLevel = difficultyLevel;
            TimerDurationMinutes = timerDurationMinutes;
            TimerCompletionReward = timerCompletionReward;
            CompletionReward = completionReward;
            Modules = modules;
        }
    }
}
