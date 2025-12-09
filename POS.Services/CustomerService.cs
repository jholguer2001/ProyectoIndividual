using POS.DataAccess.Repositories;
using POS.DTOs;
using POS.Services.Interfaces;

namespace POS.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerSearchDTO> SearchCustomersAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var (results, totalRecords) = await _customerRepository.SearchCustomersAsync(
                searchTerm, pageNumber, pageSize);

            return new CustomerSearchDTO
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Results = results.Select(c => new CustomerDTO
                {
                    CustomerId = c.CustomerId,
                    IdentificationNumber = c.IdentificationNumber,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Address = c.Address,
                    FullName = c.FullName
                }).ToList()
            };
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                return null;

            return new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                IdentificationNumber = customer.IdentificationNumber,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                FullName = customer.FullName
            };
        }
    }
}
