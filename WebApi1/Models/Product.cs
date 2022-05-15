using Dapper.Contrib.Extensions;

namespace WebApi1.Models
{
	[Table("Products")]
	public class Product
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset LastUpdatedAt { get; set; }
	}
}