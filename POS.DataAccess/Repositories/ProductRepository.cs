using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POS.Entities;
using System.Data;

namespace POS.DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(POSDbContext context) : base(context)
        {
        }

        public async Task<(List<Product> Results, int TotalRecords)> SearchProductsAsync(
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

            var products = await _context.Products
                .FromSqlRaw("EXEC sp_SearchProducts @search_term, @page_number, @page_size, @total_records OUTPUT",
                    searchTermParam, pageNumberParam, pageSizeParam, totalRecordsParam)
                .ToListAsync();

            var totalRecords = (int)totalRecordsParam.Value;

            return (products, totalRecords);
        }

        // Para validaciones de UI (incluye stock = 0)
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Where(p => p.ProductId == productId && p.IsActive)
                .FirstOrDefaultAsync();
        }

        // Para búsquedas donde solo interesan productos disponibles
        public async Task<Product> GetProductWithStockAsync(int productId)
        {
            return await _context.Products
                .Where(p => p.ProductId == productId && p.IsActive && p.StockQuantity > 0)
                .FirstOrDefaultAsync();
        }
    }
}
