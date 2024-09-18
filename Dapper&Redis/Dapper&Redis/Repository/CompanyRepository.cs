using Dapper;
using Dapper_Redis.Entity;
using Dapper_Redis.Migration;

namespace Dapper_Redis.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;
        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            try
            {
                var query = "SELECT * FROM Company";
                using (var connection = _context.CreateConnection())
                {
                    var companies = await connection.QueryAsync<Company>(query);
                    return companies;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<Company> GetCompany(int id)
        {
            var query = "SELECT * FROM Company WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QueryFirstOrDefaultAsync<Company>(query, new { id });
                return company;
            }
        }
    }
}
