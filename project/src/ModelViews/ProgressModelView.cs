using System;
using Microsoft.Data.SqlClient;
using CourseApp.Models;
using CourseApp.Data;

namespace CourseApp.Repository
{
    public class ProgressModelView : DataLink
    {
        public void UpdateTimeSpent(int userId, int courseId, int seconds)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "UPDATE Enrollment SET TimeSpent = TimeSpent + @seconds WHERE UserId = @userId AND CourseId = @courseId";
                using (var command = new SqlCommand(query, connection))
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
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT TimeSpent FROM Enrollment WHERE UserId = @userId AND CourseId = @courseId";
                using (var command = new SqlCommand(query, connection))
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
    }
}