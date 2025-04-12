using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;

namespace CourseApp.Data
{
    [ExcludeFromCodeCoverage]
    public class DataLink
    {
		private static readonly string ConnectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=CourseApp;Integrated Security=True;TrustServerCertificate=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
