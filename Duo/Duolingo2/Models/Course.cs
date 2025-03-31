//Aici am facut un model de curs ajutator
using System.Collections.Generic;

namespace Duolingo2.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsFree { get; set; }
        public List<Topic> Topics { get; set; }
        public string ImagePath { get; set; } 

        public Course(int id, string name, bool isFree, List<Topic> topics, string imagePath)
        {
            Id = id;
            Name = name;
            IsFree = isFree;
            Topics = topics;
            ImagePath = imagePath;
        }

        public bool IsUserEnrolled(int userId)
        {
            return userId % 2 == Id % 2;
        }
    }

}