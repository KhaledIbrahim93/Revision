using Dapper_Redis.Entity;

namespace Dapper_Redis.Repository
{
    public interface ICompanyRepository
    {
        public Task<IEnumerable<Company>> GetCompanies();
        public Task<Company> GetCompany(int id);


    }
}
