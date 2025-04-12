using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using CourseApp.Models;
using CourseApp.Data;

#pragma warning disable CA1822

namespace CourseApp.Repository
{
    [ExcludeFromCodeCoverage]
    public class TagModelView : DataLink
    {
        public List<Tag> GetAllTags()
        {
            var tags = new List<Tag>();
            using var connection = GetConnection();
            connection.Open();
            string query = "SELECT TagId, Name FROM Tags";
            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(new Tag
                {
                    TagId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
            return tags;
        }

        public List<Tag> GetTagsForCourse(int courseId)
        {
            var tags = new List<Tag>();
            using var connection = GetConnection();
            connection.Open();
            string query = @"
        SELECT t.TagId, t.Name 
        FROM Tags t
        INNER JOIN CourseTags ct ON t.TagId = ct.TagId
        WHERE ct.CourseId = @courseId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@courseId", courseId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(new Tag
                {
                    TagId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
            return tags;
        }
    }
}