using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POS.Entities;
using System.Data;

namespace POS.DataAccess.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(POSDbContext context) : base(context)
        {
        }

        public async Task<(List<Customer> Results, int TotalRecords)> SearchCustomersAsync(
            string searchTerm, int pageNumber, int pageSize)
        {
            var searchTermParam = new SqlParameter("@search_term", searchTerm ?? string.Empty);
            var pageNumberParam = new SqlParameter("@page_number", pageNumber);
            var pageSizeParam = new SqlParameter("@page_size", pageSize);
            var totalRecordsParam = new SqlParameter
            {
                ParameterName = "@total_records",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            var customers = await _context.Customers
                .FromSqlRaw("EXEC sp_SearchCustomers @search_term, @page_number, @page_size, @total_records OUTPUT",
                    searchTermParam, pageNumberParam, pageSizeParam, totalRecordsParam)
                .ToListAsync();

            var totalRecords = (int)totalRecordsParam.Value;

            return (customers, totalRecords);
        }
    }
}
