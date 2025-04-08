using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CourseApp.Models;
using CourseApp.Data;

namespace CourseApp.Repository
{
    public class CourseModelView : DataLink
    {
        public Course? GetCourse(int courseId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT CourseId, Title, Description, isPremium, Cost, ImageUrl, timeToComplete, difficulty FROM Courses WHERE CourseId = @courseId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@courseId", courseId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Course
                            {
                                CourseId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2),
                                IsPremium = reader.GetBoolean(3),
                                Cost = reader.GetInt32(4),
                                ImageUrl = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                TimeToComplete = reader.GetInt32(6),
                                Difficulty = reader.IsDBNull(7) ? "Easy" : reader.GetString(7)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Course> GetAllCourses()
        {
            var courses = new List<Course>();
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT CourseId, Title, Description, isPremium, Cost, ImageUrl, timeToComplete, difficulty FROM Courses";
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.GetString(2),
                            IsPremium = reader.GetBoolean(3),
                            Cost = reader.GetInt32(4),
                            ImageUrl = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            TimeToComplete = reader.GetInt32(6),
                            Difficulty = reader.IsDBNull(7) ? "Easy" : reader.GetString(7)
                        });
                    }
                }
            }
            return courses;
        }
    }
}