using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CourseApp.Data;
using CourseApp.Models;
using System;

namespace CourseApp.Repository
{
    public class CourseRepository
    {

       public bool IsModuleOpen(int userId, int moduleId)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }
        public void OpenModule(int userId, int moduleId)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = @"INSERT INTO UserProgress (UserId, ModuleId, status,ImageClicked) VALUES (@userId, @moduleId, 'not_completed',0)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool IsModuleImageClicked(int userId,int moduleId)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT ImageClicked FROM UserProgress WHERE ModuleId = @moduleId AND UserId = @userId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    command.Parameters.AddWithValue("@userId", userId);
                    var result = command.ExecuteScalar();
                    return result != null && Convert.ToBoolean(result);
                }
            }
        }
        public void ClickModuleImage(int userId,int moduleId)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "UPDATE UserProgress SET ImageClicked = 1 WHERE ModuleId = @moduleId AND UserId = @userId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }
        

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

        public void UpdateTimeSpent(int userId, int courseId, int seconds)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Enrollment SET TimeSpent = TimeSpent + @seconds WHERE UserId = @userId AND CourseId = @courseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    command.Parameters.AddWithValue("@seconds", seconds);
                    command.ExecuteNonQuery();
                }
            }
        }

        public int GetTimeSpent(int userId, int courseId)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT TimeSpent FROM Enrollment WHERE UserId = @userId AND CourseId = @courseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    var result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public int GetRequiredModulesCount(int courseId)
        {
            int count = 0;
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Modules WHERE CourseId = @courseId AND IsBonus = 0";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@courseId", courseId);
                    count = (int)command.ExecuteScalar();
                }
            }
            return count;
        }

        // Add method to get completed modules count
        public int GetCompletedModulesCount(int userId, int courseId)
        {
            int count = 0;
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = @"SELECT COUNT(*) FROM UserProgress up
                        INNER JOIN Modules m ON up.ModuleId = m.ModuleId
                        WHERE up.UserId = @userId AND m.CourseId = @courseId
                        AND up.status = 'completed' AND m.IsBonus = 0";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    count = (int)command.ExecuteScalar();
                }
            }
            return count;
        }

        // Add method to check if course is completed
        public bool IsCourseCompleted(int userId, int courseId)
        {
            int requiredModules = GetRequiredModulesCount(courseId);
            int completedModules = GetCompletedModulesCount(userId, courseId);
            return requiredModules > 0 && requiredModules == completedModules;
        }

        // Add method to check if module is available for completion (sequential logic)
        public bool IsModuleAvailable(int userId, int moduleId)
        {
            bool available = false;
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                // A module is available if:
                // 1. It's the first module in sequence (position = 1)
                // 2. The previous module is completed
                // 3. It's a bonus module (can be completed anytime)
                string query = @"
            SELECT 
                CASE
                    WHEN m.Position = 1 THEN 1
                    WHEN m.IsBonus = 1 THEN 1
                    WHEN EXISTS (
                        SELECT 1 FROM UserProgress up
                        INNER JOIN Modules prev ON up.ModuleId = prev.ModuleId
                        WHERE up.UserId = @userId
                        AND prev.CourseId = m.CourseId
                        AND prev.Position = m.Position - 1
                        AND up.status = 'completed'
                    ) THEN 1
                    ELSE 0
                END as IsAvailable
            FROM Modules m
            WHERE m.ModuleId = @moduleId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@moduleId", moduleId);
                    available = (int)command.ExecuteScalar() == 1;
                }
            }
            return available;
        }

        // Add method to track course completion
        public void MarkCourseAsCompleted(int userId, int courseId)
        {
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = @"
            IF NOT EXISTS (
                SELECT 1 FROM CourseCompletions 
                WHERE UserId = @userId AND CourseId = @courseId
            )
            BEGIN
                INSERT INTO CourseCompletions (UserId, CourseId, CompletionRewardClaimed, TimedRewardClaimed, CompletedAt)
                VALUES (@userId, @courseId, 0, 0, GETDATE())
            END";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Add method to claim completion reward
        public bool ClaimCompletionReward(int userId, int courseId)
        {
            bool claimed = false;
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();

                // First check if it's already claimed
                string checkQuery = @"
            SELECT CompletionRewardClaimed 
            FROM CourseCompletions 
            WHERE UserId = @userId AND CourseId = @courseId";

                bool alreadyClaimed = false;
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@userId", userId);
                    checkCommand.Parameters.AddWithValue("@courseId", courseId);
                    var result = checkCommand.ExecuteScalar();
                    alreadyClaimed = result != null && (bool)result;
                }

                if (!alreadyClaimed)
                {
                    string updateQuery = @"
                UPDATE CourseCompletions
                SET CompletionRewardClaimed = 1
                WHERE UserId = @userId AND CourseId = @courseId";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@userId", userId);
                        updateCommand.Parameters.AddWithValue("@courseId", courseId);
                        updateCommand.ExecuteNonQuery();
                        claimed = true;
                    }
                }
            }
            return claimed;
        }

        // Add method for timed reward
        public bool ClaimTimedReward(int userId, int courseId, int timeSpent, int timeLimit)
        {
            bool claimed = false;

            // Only claim if completed within time limit
            if (timeSpent <= timeLimit)
            {
                using (SqlConnection connection = DataLink.GetConnection())
                {
                    connection.Open();

                    // Check if already claimed
                    string checkQuery = @"
                SELECT TimedRewardClaimed 
                FROM CourseCompletions 
                WHERE UserId = @userId AND CourseId = @courseId";

                    bool alreadyClaimed = false;
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@userId", userId);
                        checkCommand.Parameters.AddWithValue("@courseId", courseId);
                        var result = checkCommand.ExecuteScalar();
                        alreadyClaimed = result != null && (bool)result;
                    }

                    if (!alreadyClaimed)
                    {
                        string updateQuery = @"
                    UPDATE CourseCompletions
                    SET TimedRewardClaimed = 1
                    WHERE UserId = @userId AND CourseId = @courseId";

                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@userId", userId);
                            updateCommand.Parameters.AddWithValue("@courseId", courseId);
                            updateCommand.ExecuteNonQuery();
                            claimed = true;
                        }
                    }
                }
            }

            return claimed;
        }

        // Add method to get course time limit
        public int GetCourseTimeLimit(int courseId)
        {
            int timeLimit = 0;
            using (SqlConnection connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT TimeToComplete FROM Courses WHERE CourseId = @courseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@courseId", courseId);
                    var result = command.ExecuteScalar();

                    // Check for null or DBNull before conversion
                    if (result != null && result != DBNull.Value)
                    {
                        timeLimit = Convert.ToInt32(result);
                    }
                }
            }
            return timeLimit;
        }


      
    }
}

