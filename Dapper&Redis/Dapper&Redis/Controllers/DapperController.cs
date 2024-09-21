using Dapper_Redis.Entity;
using Dapper_Redis.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text;

namespace Dapper_Redis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DapperController : ControllerBase
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IDistributedCache _cache;

        private readonly ILogger<DapperController> _logger;

        public DapperController(ILogger<DapperController> logger, ICompanyRepository companyRepository, IDistributedCache cache)
        {
            _logger = logger;
            this.companyRepository = companyRepository;
            _cache = cache;
        }

        [HttpGet]
        public async Task<List<Company>> GetAllCompnies()
        {
            var cachKey = "All_Companies";
            var companyList=new List<Company>();
            var cachedData = await _cache.GetAsync(cachKey);
            if (cachedData != null)
            {
                // If data found in cache, encode and deserialize cached data
                var cachedDataString = Encoding.UTF8.GetString(cachedData);
                companyList = JsonSerializer.Deserialize<List<Company>>(cachedDataString) ?? new List<Company>();

            }
            else
            {
                // If not found, then fetch data from database
                var companies = await companyRepository.GetCompanies();
                companyList=companies.ToList(); 
                // serialize data
                var cachedDataString = JsonSerializer.Serialize(companies);
                var newDataToCache = Encoding.UTF8.GetBytes(cachedDataString);

                // set cache options 
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(2))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                // Add data in cache

                await _cache.SetAsync(cachKey, newDataToCache, options);
               
            }
            return companyList.ToList();
        }
        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                var company = await companyRepository.GetCompany(id);
                if (company == null)
                    return NotFound();
                return Ok(company);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}
