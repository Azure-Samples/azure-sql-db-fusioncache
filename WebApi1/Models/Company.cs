using Dapper.Contrib.Extensions;

namespace WebApi1.Models
{
	[Table("Companies")]
	public class Company
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? CatchPhrase { get; set; }
		public int FoundationYear { get; set; }
		public bool IsFullRemote { get; set; }
	}
}