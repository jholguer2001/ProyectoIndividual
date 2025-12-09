using POS.DTOs;

namespace POS.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerSearchDTO> SearchCustomersAsync(string searchTerm, int pageNumber, int pageSize);
        Task<CustomerDTO> GetCustomerByIdAsync(int customerId);
    }
}
