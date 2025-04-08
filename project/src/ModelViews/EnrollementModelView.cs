using System;
using CourseApp.Data;
using Microsoft.Data.SqlClient;

namespace CourseApp.Repository
{
    public class EnrollmentModelView : DataLink
    {
        public bool IsUserEnrolled(int userId, int courseId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Enrollment WHERE UserId = @userId AND CourseId = @courseId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    return (int)command.ExecuteScalar() > 0;
                }
            }
        }

        public void EnrollUser(int userId, int courseId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string query = @"
                    IF NOT EXISTS (SELECT 1 FROM Enrollment WHERE UserId=@userId AND CourseId=@courseId)
                    INSERT INTO Enrollment (UserId, CourseId, EnrolledAt, isCompleted) 
                    VALUES (@userId, @courseId, GETDATE(), 0)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}