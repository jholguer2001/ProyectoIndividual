using POS.DTOs;

namespace POS.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductSearchDTO> SearchProductsAsync(string searchTerm, int pageNumber, int pageSize);
        Task<ProductDTO> GetProductByIdAsync(int productId);
        Task<bool> ValidateStockAsync(int productId, int quantity);
    }
}
