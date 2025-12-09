using Microsoft.AspNetCore.Mvc;
using POS.Services.Interfaces;

namespace POS.Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customer/SearchModal
        [HttpGet]
        public IActionResult SearchModal()
        {
            return PartialView("_CustomerSearchModal");
        }

        // GET: Customer/Search
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm, int pageNumber = 1)
        {
            const int pageSize = 10;
            var result = await _customerService.SearchCustomersAsync(searchTerm ?? "", pageNumber, pageSize);
            return PartialView("_CustomerSearchResults", result);
        }

        // GET: Customer/GetCustomer/5
        [HttpGet]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Json(customer);
        }
    }
}
