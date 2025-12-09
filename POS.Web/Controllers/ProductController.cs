using Microsoft.AspNetCore.Mvc;
using POS.Services.Interfaces;

namespace POS.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Product/SearchModal
        [HttpGet]
        public IActionResult SearchModal()
        {
            return PartialView("_ProductSearchModal");
        }

        // GET: Product/Search
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm, int pageNumber = 1)
        {
            const int pageSize = 10;
            var result = await _productService.SearchProductsAsync(searchTerm ?? "", pageNumber, pageSize);
            return PartialView("_ProductSearchResults", result);
        }

        // GET: Product/GetProduct/5
        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Json(product);
        }
    }
}
