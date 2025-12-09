using POS.Entities;

namespace POS.DataAccess.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<(List<Product> Results, int TotalRecords)> SearchProductsAsync(
            string searchTerm, int pageNumber, int pageSize);
        Task<Product> GetProductWithStockAsync(int productId);
    }
}
