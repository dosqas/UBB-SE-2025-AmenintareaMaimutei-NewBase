using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CourseApp.Data;
using CourseApp.Models;

namespace CourseApp.Repository
{
    public class CourseRepository
    {
        public List<Course> GetAllCourses()
        {
            List<Course> courses = new List<Course>();
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT CourseId, Title, Description, isPremium, Cost, ImageUrl, timeToComplete FROM Courses";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Course course = new Course
                        {
                            CourseId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.GetString(2),
                            IsPremium = reader.GetBoolean(3),
                            Cost = reader.GetInt32(4),
                            ImageUrl = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            TimeToComplete = reader.GetInt32(6)
                        };
                        courses.Add(course);
                    }
                }
            }
            return courses;
        }

        public List<Tag> GetAllTags()
        {
            List<Tag> tags = new List<Tag>();
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT TagId, Name FROM Tags";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tag tag = new Tag
                        {
                            TagId = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                        tags.Add(tag);
                    }
                }
            }
            return tags;
        }

        public List<Module> GetModulesByCourseId(int courseId)
        {
            List<Module> modules = new List<Module>();
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = @"SELECT ModuleId, CourseId, Title, Description, Position, isBonus, Cost, ImageUrl 
                                 FROM Modules 
                                 WHERE CourseId = @courseId ORDER BY Position";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@courseId", courseId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Module module = new Module
                            {
                                ModuleId = reader.GetInt32(0),
                                CourseId = reader.GetInt32(1),
                                Title = reader.GetString(2),
                                Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Position = reader.GetInt32(4),
                                IsBonus = reader.GetBoolean(5),
                                Cost = reader.GetInt32(6),
                                ImageUrl = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                            };
                            modules.Add(module);
                        }
                    }
                }
            }
            return modules;
        }

        public bool IsUserEnrolled(int userId, int courseId)
        {
            bool enrolled = false;
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Enrollment WHERE UserId = @userId AND CourseId = @courseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    enrolled = (int)command.ExecuteScalar() > 0;
                }
            }
            return enrolled;
        }

        public bool IsModuleCompleted(int userId, int moduleId)
        {
            bool completed = false;
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId AND status = 'completed'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    completed = (int)command.ExecuteScalar() > 0;
                }
            }
            return completed;
        }

        public void CompleteModule(int userId, int moduleId)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = @"IF EXISTS (SELECT 1 FROM UserProgress WHERE UserId=@userId AND ModuleId=@moduleId)
                                 UPDATE UserProgress SET status='completed' WHERE UserId=@userId AND ModuleId=@moduleId
                                 ELSE
                                 INSERT INTO UserProgress (UserId, ModuleId, status) VALUES (@userId, @moduleId, 'completed')";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EnrollUser(int userId, int courseId)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = @"IF NOT EXISTS (SELECT 1 FROM Enrollment WHERE UserId=@userId AND CourseId=@courseId)
                                 INSERT INTO Enrollment (UserId, CourseId, EnrolledAt, isCompleted) VALUES (@userId, @courseId, GETDATE(), 0)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Tag> GetTagsForCourse(int courseId)
        {
            List<Tag> tags = new List<Tag>();
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = @"SELECT t.TagId, t.Name FROM Tags t
                                 INNER JOIN CourseTags ct ON t.TagId = ct.TagId
                                 WHERE ct.CourseId = @courseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@courseId", courseId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tag tag = new Tag
                            {
                                TagId = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            };
                            tags.Add(tag);
                        }
                    }
                }
            }
            return tags;
        }
    }
}
