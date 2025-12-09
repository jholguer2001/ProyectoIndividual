using POS.Entities;

namespace POS.DataAccess.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<(List<Customer> Results, int TotalRecords)> SearchCustomersAsync(
            string searchTerm, int pageNumber, int pageSize);
    }
}
