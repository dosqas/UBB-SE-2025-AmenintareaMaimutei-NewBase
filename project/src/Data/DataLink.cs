using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;

namespace CourseApp.Data
{
    [ExcludeFromCodeCoverage]
    public class DataLink
    {
<<<<<<< HEAD
		private static readonly string ConnectionString = "Data Source=leptop-sarici;Initial Catalog=CourseApp;Integrated Security=True;TrustServerCertificate=True";
=======
		private static readonly string ConnectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=CourseApp;Integrated Security=True;TrustServerCertificate=True";
>>>>>>> a7b77180abe8db9be89b20062c4e96ea53a77a59

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
