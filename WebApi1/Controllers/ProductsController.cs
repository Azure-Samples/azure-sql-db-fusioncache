using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using WebApi1.Infrastructure;
using WebApi1.Models;
using ZiggyCreatures.Caching.Fusion;

namespace WebApi1.Controllers
{
	[ApiController]
	[Route("api/products")]
	public class ProductsController : ControllerBase
	{
		private readonly DapperContext _dapperContext;
		private readonly IFusionCache _cache;

		public ProductsController(DapperContext dapperContext, IFusionCache cache)
		{
			_dapperContext = dapperContext;
			_cache = cache;
		}

		[HttpPost]
		[Route("generate/{quantity:int}")]
		public ActionResult<Product> Generate(int quantity)
		{
			if (quantity < 0)
				return BadRequest("The quantity must be a positive integer");

			using var conn = _dapperContext.CreateConnection();

			var products = new List<Product>(quantity);

			for (int i = 0; i < quantity; i++)
			{
				var product = new Product
				{
					Name = Faker.Company.Name(),
					CreatedAt = DateTimeOffset.Now.AddDays(-1 * Faker.RandomNumber.Next(1, 365 * 5)),
					LastUpdatedAt = DateTimeOffset.Now
				};

				conn.Insert(product);

				products.Add(product);
			}

			return Ok(products);
		}

		[HttpPost]
		public ActionResult<Product> Insert(Product product)
		{
			if (product is null)
				return BadRequest();

			using var conn = _dapperContext.CreateConnection();

			// CREATED AT
			if (product.CreatedAt == DateTimeOffset.MinValue)
				product.CreatedAt = DateTimeOffset.UtcNow;

			// LAST UPDATED AT
			product.LastUpdatedAt = DateTimeOffset.UtcNow;

			// DATABASE
			var newId = conn.Insert(product);
			if (newId <= 0)
				return BadRequest();

			return CreatedAtAction("Get", new { Id = newId }, product);
		}

		[HttpGet]
		[Route("{id:int}")]
		public ActionResult<Product> Get(int id)
		{
			using var conn = _dapperContext.CreateConnection();

			var product = _cache.GetOrSet(
				$"product:{id}",
				(ctx, _) =>
				{
					var x = conn.Get<Product>(id);

					ctx.Options.Duration = x.GetCacheDuration();

					return x;
				}
			);

			if (product is null)
				return NotFound();

			return Ok(product);
		}

		[HttpPut]
		public ActionResult<Product> Update(Product product)
		{
			if (product is null)
				return NotFound();

			using var conn = _dapperContext.CreateConnection();

			// LAST UPDATED AT
			product.LastUpdatedAt = DateTimeOffset.UtcNow;

			// DATABASE
			var isUpdated = conn.Update(product);
			if (isUpdated == false)
				return NotFound();

			// CACHE
			_cache.Set(
				$"product:{product.Id}",
				product,
				product.GetCacheDuration()
			);

			return Ok(product);
		}

		[HttpDelete]
		[Route("{id:int}")]
		public ActionResult<bool> Delete(int id)
		{
			using var conn = _dapperContext.CreateConnection();

			// DATABASE
			var isDeleted = conn.Delete<Product>(new Product { Id = id });
			if (isDeleted == false)
				return NotFound();

			// CACHE
			_cache.Remove(
				$"product:{id}"
			);

			return true;
		}
	}
}