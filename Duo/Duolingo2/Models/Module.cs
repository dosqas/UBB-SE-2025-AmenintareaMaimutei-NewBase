using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duo.Models
{
    public class Module
    {
        public int ModuleId { get; }
        public int CourseId { get; }
        public string ImagePath { get; }
        public bool IsCompleted { get; }
        public string Title { get; }
        public string Description { get; }
        public int Position { get; }
        public bool IsBonusModule { get; }
        public int UnlockCost { get; }

        public Module(int moduleId, int courseId, string imagePath, bool isCompleted, string title, string description, int position, bool isBonusModule, int unlockCost)
        {
            ModuleId = moduleId;
            CourseId = courseId;
            ImagePath = imagePath;
            IsCompleted = isCompleted;
            Title = title;
            Description = description;
            Position = position;
            IsBonusModule = isBonusModule;
            UnlockCost = unlockCost;
        }
    }

}
