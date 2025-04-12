using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CourseApp.Models;
using CourseApp.Data;

#pragma warning disable CA1822

namespace CourseApp.Repository
{
    [ExcludeFromCodeCoverage]
    public class ModuleModelView : DataLink
    {
        public Module? GetModule(int moduleId)
        {
            using var connection = GetConnection();
            connection.Open();
            string query = "SELECT ModuleId, CourseId, Title, Description, Position, isBonus, Cost, ImageUrl FROM Modules WHERE ModuleId = @moduleId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@moduleId", moduleId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Module
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
            }
            return null;
        }

        public List<Module> GetModulesByCourseId(int courseId)
        {
            var modules = new List<Module>();
            using var connection = GetConnection();
            connection.Open();
            string query = "SELECT ModuleId, CourseId, Title, Description, Position, isBonus, Cost, ImageUrl FROM Modules WHERE CourseId = @courseId ORDER BY Position";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@courseId", courseId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                modules.Add(new Module
                {
                    ModuleId = reader.GetInt32(0),
                    CourseId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Position = reader.GetInt32(4),
                    IsBonus = reader.GetBoolean(5),
                    Cost = reader.GetInt32(6),
                    ImageUrl = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                });
            }
            return modules;
        }

        public bool IsModuleOpen(int userId, int moduleId)
        {
            using var connection = DataLink.GetConnection();
            connection.Open();
            string query = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@moduleId", moduleId);
            return (int)command.ExecuteScalar() > 0;
        }

        public void OpenModule(int userId, int moduleId)
        {
            using var connection = DataLink.GetConnection();
            connection.Open();
            string query = @"INSERT INTO UserProgress (UserId, ModuleId, status,ImageClicked) VALUES (@userId, @moduleId, 'not_completed',0)";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@moduleId", moduleId);
            command.ExecuteNonQuery();
        }

        public bool IsModuleImageClicked(int userId, int moduleId)
        {
            using var connection = DataLink.GetConnection();
            connection.Open();
            string query = "SELECT ImageClicked FROM UserProgress WHERE ModuleId = @moduleId AND UserId = @userId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@moduleId", moduleId);
            command.Parameters.AddWithValue("@userId", userId);
            var result = command.ExecuteScalar();
            return result != null && Convert.ToBoolean(result);
        }

        public void ClickModuleImage(int userId, int moduleId)
        {
            using var connection = DataLink.GetConnection();
            connection.Open();
            string query = "UPDATE UserProgress SET ImageClicked = 1 WHERE ModuleId = @moduleId AND UserId = @userId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@moduleId", moduleId);
            command.Parameters.AddWithValue("@userId", userId);
            command.ExecuteNonQuery();
        }

        public bool IsModuleCompleted(int userId, int moduleId)
        {
            using var connection = DataLink.GetConnection();
            connection.Open();
            string query = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId AND status = 'completed'";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@moduleId", moduleId);
            return (int)command.ExecuteScalar() > 0;
        }

        public void CompleteModule(int userId, int moduleId)
        {
            using var connection = DataLink.GetConnection();
            connection.Open();
            string query = @"IF EXISTS (SELECT 1 FROM UserProgress WHERE UserId=@userId AND ModuleId=@moduleId)
                     UPDATE UserProgress SET status='completed' WHERE UserId=@userId AND ModuleId=@moduleId
                     ELSE
                     INSERT INTO UserProgress (UserId, ModuleId, status) VALUES (@userId, @moduleId, 'completed')";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@moduleId", moduleId);
            command.ExecuteNonQuery();
        }

        public bool IsModuleAvailable(int userId, int moduleId)
        {
            bool available = false;
            using var connection = DataLink.GetConnection();
            connection.Open();
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

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@moduleId", moduleId);
            available = (int)command.ExecuteScalar() == 1;
            return available;
        }

        public bool IsModuleInProgress(int userId, int moduleId)
        {
            using var connection = DataLink.GetConnection();
            connection.Open();
            string query = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@moduleId", moduleId);
            return (int)command.ExecuteScalar() > 0;
        }
    }
}