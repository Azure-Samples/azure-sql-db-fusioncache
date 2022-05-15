using WebApi1.Infrastructure;
using ZiggyCreatures.Caching.Fusion;

namespace WebApi1
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// CONNECTION STRINGS
			var sqlConn = builder.Configuration.GetConnectionString("Sql");
			var redisConn = builder.Configuration.GetConnectionString("Redis");

			if (string.IsNullOrWhiteSpace(sqlConn))
				throw new NullReferenceException("You must specify a sql connection (see appsettings.json)");

			// ADD SERVICES: DAPPER CONTEXT
			builder.Services.AddSingleton<DapperContext>(new DapperContext(sqlConn));

			// ADD SERVICES: REDIS
			if (string.IsNullOrWhiteSpace(redisConn) == false)
			{
				// ADD SERVICES: REDIS DISTRIBUTED CACHE
				builder.Services.AddStackExchangeRedisCache(options =>
				{
					options.Configuration = redisConn;
				});

				// ADD SERVICES: REDIS BACKPLANE
				builder.Services.AddFusionCacheStackExchangeRedisBackplane(options =>
				{
					options.Configuration = redisConn;
				});
			}

			// ADD SERVICES: FUSIONCACHE
			builder.Services.AddFusionCache(options =>
			{
				// SET DEFAULT OPTIONS
				options.DefaultEntryOptions = new FusionCacheEntryOptions()
					.SetDuration(TimeSpan.FromMinutes(5))
					.SetFailSafe(true, TimeSpan.FromMinutes(1))
					.SetFactoryTimeouts(TimeSpan.FromMilliseconds(100));
			});

			// ADD SERVICES: OTHER STUFF
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthorization();
			app.MapControllers();

			app.Run();
		}
	}
}