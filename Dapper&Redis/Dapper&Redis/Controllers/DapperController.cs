using Dapper_Redis.Entity;
using Dapper_Redis.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Dapper_Redis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DapperController : ControllerBase
    {
        private readonly ICompanyRepository companyRepository;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DapperController> _logger;

        public DapperController(ILogger<DapperController> logger, ICompanyRepository companyRepository)
        {
            _logger = logger;
            this.companyRepository = companyRepository;
        }

        [HttpGet]
        public async Task<List<Company>> GetAllCompnies()
        {
            var companies = await companyRepository.GetCompanies();
            return companies.ToList();
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
