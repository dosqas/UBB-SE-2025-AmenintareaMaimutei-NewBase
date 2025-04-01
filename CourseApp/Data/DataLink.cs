using Microsoft.Data.SqlClient;

namespace CourseApp.Data
{
    public class DataLink
    {
        private static readonly string connectionString = "Data Source=DESKTOP-TE49281;Initial Catalog=CourseApp;Integrated Security=True;TrustServerCertificate=True";
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
