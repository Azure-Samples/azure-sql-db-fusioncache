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

		public TimeSpan GetCacheDuration()
		{
			// MORE FRESH THAN 10min: CACHE FOR 30sec
			if (LastUpdatedAt > DateTimeOffset.UtcNow - TimeSpan.FromMinutes(10))
				return TimeSpan.FromSeconds(30);

			// MORE FRESH THAN 1h: CACHE FOR 2min
			if (LastUpdatedAt > DateTimeOffset.UtcNow - TimeSpan.FromHours(1))
				return TimeSpan.FromMinutes(2);

			// OLDER: CACHE FOR 5min
			return TimeSpan.FromMinutes(5);
		}
	}
}