using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Duo.Models;
using Duo.Data;
using Windows.System;

namespace Duo.Repositories
{
    public class ModuleRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public ModuleRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Module>> GetModulesByCourseAsync(int courseId)
        {
            var modules = new List<Module>();

            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT ModuleId, CourseId, Title, Description, Position, IsBonusModule, UnlockCost FROM Modules WHERE CourseId = @CourseId ORDER BY Position";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", courseId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            modules.Add(new Module
                            (
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                this.IsModuleCompletedAsync(courseId, reader.GetInt32(0)).Result,
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetInt32(4),
                                reader.GetBoolean(5),
                                reader.GetInt32(6)
                            ));
                        }
                    }
                }
            }

            return modules;
        }

        public Module GetModuleById(int courseId, int moduleId)
        {
            var modules = this.GetModulesByCourseAsync(courseId).Result;
            foreach (var module in modules)
            {
                if (module.ModuleId == moduleId)
                {
                    return module;
                }
            }
            return null;

        }

        public async Task MarkModuleAsCompletedAsync(int courseId, int moduleId, int userId = 0)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                var query = @"IF EXISTS (SELECT 1 FROM UserProgress 
                               WHERE UserId = @UserId AND CourseId = @CourseId AND ModuleId = @ModuleId)
                              BEGIN
                                UPDATE UserProgress 
                                SET ProgressPercentage = 100, LastUpdated = @Now
                                WHERE UserId = @UserId AND CourseId = @CourseId AND ModuleId = @ModuleId
                              END
                              ELSE
                              BEGIN
                                INSERT INTO UserProgress (UserId, CourseId, ModuleId, ProgressPercentage, LastUpdated)
                                VALUES (@UserId, @CourseId, @ModuleId, 100, @Now)
                              END";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@CourseId", courseId);
                    command.Parameters.AddWithValue("@ModuleId", moduleId);
                    command.Parameters.AddWithValue("@Now", DateTime.UtcNow);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> IsModuleCompletedAsync(int courseId, int moduleId, int userId = 0)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT 1 FROM UserProgress WHERE UserId = @UserId AND CourseId = @CourseId AND ModuleId = @ModuleId AND ProgressPercentage = 100";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@CourseId", courseId);
                    command.Parameters.AddWithValue("@ModuleId", moduleId);

                    var result = await command.ExecuteScalarAsync();
                    return result != null;
                }
            }
        }

        public async void UnlockBonusModule(int moduleId, int userId = 0)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();
                var query = @"INSERT INTO UserModuleUnlock (UserId, ModuleId, UnlockedAt) VALUES (@UserId, @ModuleId, @Now)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@ModuleId", moduleId);
                    command.Parameters.AddWithValue("@Now", DateTime.UtcNow);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}