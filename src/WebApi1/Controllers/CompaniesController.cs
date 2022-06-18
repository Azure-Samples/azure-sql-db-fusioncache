using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using WebApi1.Infrastructure;
using WebApi1.Models;
using ZiggyCreatures.Caching.Fusion;

namespace WebApi1.Controllers
{
	[ApiController]
	[Route("api/companies")]
	public class CompaniesController : ControllerBase
	{
		private readonly DapperContext _dapperContext;
		private readonly IFusionCache _cache;

		public CompaniesController(DapperContext dapperContext, IFusionCache cache)
		{
			_dapperContext = dapperContext;
			_cache = cache;
		}

		[HttpPost]
		[Route("generate/{quantity:int}")]
		public ActionResult<Company> Generate(int quantity)
		{
			if (quantity < 0)
				return BadRequest("The quantity must be a positive integer");

			using var conn = _dapperContext.CreateConnection();

			var companies = new List<Company>(quantity);

			for (int i = 0; i < quantity; i++)
			{
				var company = new Company
				{
					Name = Faker.Company.Name(),
					CatchPhrase = Faker.Company.CatchPhrase(),
					FoundationYear = Faker.RandomNumber.Next(1800, DateTime.UtcNow.Year),
					IsFullRemote = Faker.Boolean.Random()
				};

				conn.Insert(company);

				companies.Add(company);
			}

			return Ok(companies);
		}

		[HttpPost]
		public ActionResult<Company> Insert(Company company)
		{
			if (company is null)
				return BadRequest();

			if (string.IsNullOrWhiteSpace(company.Name))
				return BadRequest();

			using var conn = _dapperContext.CreateConnection();

			// DATABASE
			var newId = conn.Insert(company);
			if (newId <= 0)
				return BadRequest();

			return CreatedAtAction("Get", new { Id = newId }, company);
		}

		[HttpGet]
		[Route("{id:int}")]
		public ActionResult<Company> Get(int id)
		{
			using var conn = _dapperContext.CreateConnection();

			var company = _cache.GetOrSet(
				$"company:{id}",
				_ => conn.Get<Company>(id)
			);

			if (company is null)
				return NotFound();

			return Ok(company);
		}

		[HttpPut]
		public ActionResult<Company> Update(Company company)
		{
			if (company is null)
				return NotFound();

			using var conn = _dapperContext.CreateConnection();

			// DATABASE
			var isUpdated = conn.Update(company);
			if (isUpdated == false)
				return NotFound();

			// CACHE
			_cache.Set(
				$"company:{company.Id}",
				company
			);

			return Ok(company);
		}

		[HttpDelete]
		[Route("{id:int}")]
		public ActionResult<bool> Delete(int id)
		{
			using var conn = _dapperContext.CreateConnection();

			// DATABASE
			var isDeleted = conn.Delete<Company>(new Company { Id = id });
			if (isDeleted == false)
				return NotFound();

			// CACHE
			_cache.Remove(
				$"company:{id}"
			);

			return true;
		}
	}
}