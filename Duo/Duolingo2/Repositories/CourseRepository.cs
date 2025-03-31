using Duo.Models;
using Duo.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Duo.Repositories
{
    public class CourseRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public CourseRepository(DatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            var courses = new List<Course>();

            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                var query = @"SELECT c.CourseId, c.Title, c.Description, c.TypeId, ct.TypeName, ct.Price, c.CreatedAt, c.ImagePath, c.DifficultyLevel, c.TimerDurationMinutes, c.TimerCompletionReward, c.CompletionReward
                             FROM Courses c
                             JOIN CourseTypes ct ON c.TypeId = ct.TypeId";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var courseId = reader.GetInt32(0);
                            var tags = await GetCourseTagsAsync(courseId);
                            var isEnrolled = await IsEnrolledByUserId(0, courseId);
                            var modules = await new ModuleRepository(_dbConnection).GetModulesByCourseAsync(courseId);
                            var cType = new CourseType(
                                reader.GetInt32(3),
                                reader.GetString(4),
                                reader.GetInt32(5)
                            );

                            courses.Add(new Course(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                tags,
                                reader.GetString(7),
                                isEnrolled,
                                cType,
                                reader.GetDateTime(6),
                                reader.GetInt32(8),
                                reader.GetInt32(9),
                                reader.GetInt32(10),
                                reader.GetInt32(11),
                                modules
                            ));
                        }
                    }
                }
            }

            return courses;
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                var query = @"SELECT c.CourseId, c.Title, c.Description, c.TypeId, ct.TypeName, ct.Price, c.CreatedAt, c.ImagePath, c.DifficultyLevel, c.TimerDurationMinutes, c.TimerCompletionReward, c.CompletionReward
                             FROM Courses c
                             JOIN CourseTypes ct ON c.TypeId = ct.TypeId
                             WHERE c.CourseId = @CourseId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", courseId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var tags = await GetCourseTagsAsync(courseId);
                            var modules = await new ModuleRepository(_dbConnection).GetModulesByCourseAsync(courseId);
                            var isEnrolled = await IsEnrolledByUserId(0, courseId);
                            var cType = new CourseType(
                                reader.GetInt32(3),
                                reader.GetString(4),
                                reader.GetInt32(5)
                            );

                            return new Course(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                tags,
                                reader.GetString(7),
                                isEnrolled,
                                cType,
                                reader.GetDateTime(6),
                                reader.GetInt32(8),
                                reader.GetInt32(9),
                                reader.GetInt32(10),
                                reader.GetInt32(11),
                                modules
                            );
                            
                        }
                    }
                }
            }

            return null;
        }

        public async Task<List<Tag>> GetCourseTagsAsync(int courseId)
        {
            var tags = new List<Tag>();

            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();

                var query = @"SELECT t.TagId, t.TagName
                         FROM CourseTags ct
                         JOIN Tags t ON ct.TagId = t.TagId
                         WHERE ct.CourseId = @CourseId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseId", courseId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tags.Add(new Tag(reader.GetInt32(0), reader.GetString(1)));
                        }
                    }
                }
            }

            return tags;
        }

        public async Task<bool> IsEnrolledByUserId(int UserId, int CourseId)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();
                var query = @"SELECT * FROM Enrollment WHERE UserId = @UserId AND CourseId = @CourseId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", UserId);
                    command.Parameters.AddWithValue("@CourseId", CourseId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync();
                    }
                }
            }
        }

        public async Task<bool> EnrollUser(int userId, int courseId)
        {
            using (var connection = _dbConnection.GetConnection())
            {
                await connection.OpenAsync();
                var query = @"INSERT INTO Enrollment (UserId, CourseId, EnrolledAt) VALUES (@UserId, @CourseId, @Now)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@CourseId", courseId);
                    command.Parameters.AddWithValue("@Now", System.DateTime.Now);
                    bool success = await command.ExecuteNonQueryAsync() > 0;
                    return success;
                }
            }
        }
    }
}