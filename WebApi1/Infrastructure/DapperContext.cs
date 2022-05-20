using System.Data;
using Microsoft.Data.SqlClient;

namespace WebApi1.Infrastructure
{
	public class DapperContext
	{
		private readonly string _conn;

		public DapperContext(string conn)
		{
			_conn = conn;
		}

		public IDbConnection CreateConnection()
		{
			return new SqlConnection(_conn);
		}
	}
}
