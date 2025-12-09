using POS.DataAccess.Repositories;
using POS.DTOs;
using POS.Services.Interfaces;

namespace POS.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductSearchDTO> SearchProductsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var (results, totalRecords) = await _productRepository.SearchProductsAsync(
                searchTerm, pageNumber, pageSize);

            return new ProductSearchDTO
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Results = results.Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    UnitPrice = p.UnitPrice,
                    StockQuantity = p.StockQuantity,
                    Taxable = p.Taxable
                }).ToList()
            };
        }

        public async Task<ProductDTO> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductWithStockAsync(productId);
            if (product == null)
                return null;

            return new ProductDTO
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                StockQuantity = product.StockQuantity,
                Taxable = product.Taxable
            };
        }

        /// <summary>
        /// Validates stock availability (for UI feedback only).
        /// ⚠️ This is NOT thread-safe. Real validation happens in sp_CreateSalesOrder with locks.
        /// </summary>
        public async Task<bool> ValidateStockAsync(int productId, int quantity)
        {
            var product = await _productRepository.GetProductWithStockAsync(productId);
            if (product == null)
                return false;

            return product.StockQuantity >= quantity;
        }
    }
}
