using Microsoft.Data.SqlClient;

namespace CourseApp.Data
{
    public class DataLink
    {
        private static readonly string connectionString = "Data Source=DESKTOP-2JEMU2O\\SQLEXPRESS;Initial Catalog=CourseApp;Integrated Security=True;TrustServerCertificate=True";
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
