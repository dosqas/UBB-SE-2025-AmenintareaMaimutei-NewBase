using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseApp.Data;
using Microsoft.Data.SqlClient;

#pragma warning disable CA1822

namespace CourseApp.ModelViews
{
    [ExcludeFromCodeCoverage]
    internal class RewardModelView : DataLink
    {
        // Add method to claim completion reward
        public bool ClaimCompletionReward(int userId, int courseId)
        {
            bool claimed = false;
            using (var connection = DataLink.GetConnection())
            {
                connection.Open();

                // First check if it's already claimed
                string checkQuery = @"
        SELECT CompletionRewardClaimed 
        FROM CourseCompletions 
        WHERE UserId = @userId AND CourseId = @courseId";

                bool alreadyClaimed = false;
                using (var checkCommand = new SqlCommand(checkQuery, connection))
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

                    using (var updateCommand = new SqlCommand(updateQuery, connection))
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

        public bool ClaimTimedReward(int userId, int courseId, int timeSpent, int timeLimit)
        {
            bool claimed = false;

            // Only claim if completed within time limit
            if (timeSpent <= timeLimit)
            {
                using (var connection = DataLink.GetConnection())
                {
                    connection.Open();

                    // Check if already claimed
                    string checkQuery = @"
            SELECT TimedRewardClaimed 
            FROM CourseCompletions 
            WHERE UserId = @userId AND CourseId = @courseId";

                    bool alreadyClaimed = false;
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
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

                        using (var updateCommand = new SqlCommand(updateQuery, connection))
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

        public int GetCourseTimeLimit(int courseId)
        {
            int timeLimit = 0;
            using (var connection = DataLink.GetConnection())
            {
                connection.Open();
                string query = "SELECT TimeToComplete FROM Courses WHERE CourseId = @courseId";
                using (var command = new SqlCommand(query, connection))
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
