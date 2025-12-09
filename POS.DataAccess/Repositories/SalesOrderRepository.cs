using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POS.Entities;
using System.Data;
using System.Text.Json;

namespace POS.DataAccess.Repositories
{
    public class SalesOrderRepository : Repository<SalesOrder>, ISalesOrderRepository
    {
        public SalesOrderRepository(POSDbContext context) : base(context)
        {
        }

        public async Task<(int OrderId, string OrderNumber, string ErrorMessage)> CreateSalesOrderAsync(
            int customerId, List<(int ProductId, int Quantity, decimal UnitPrice)> orderDetails,
            string notes, string createdBy)
        {
            // Convert order details to JSON
            var orderDetailsJson = JsonSerializer.Serialize(
                orderDetails.Select(od => new
                {
                    product_id = od.ProductId,
                    quantity = od.Quantity,
                    unit_price = od.UnitPrice
                })
            );

            var customerIdParam = new SqlParameter("@customer_id", customerId);
            var orderDetailsParam = new SqlParameter("@order_details", orderDetailsJson);
            var notesParam = new SqlParameter("@notes", (object)notes ?? DBNull.Value);
            var createdByParam = new SqlParameter("@created_by", createdBy ?? "SYSTEM");

            var orderIdParam = new SqlParameter
            {
                ParameterName = "@order_id",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            var orderNumberParam = new SqlParameter
            {
                ParameterName = "@order_number",
                SqlDbType = SqlDbType.NVarChar,
                Size = 50,
                Direction = ParameterDirection.Output
            };

            var errorMessageParam = new SqlParameter
            {
                ParameterName = "@error_message",
                SqlDbType = SqlDbType.NVarChar,
                Size = 500,
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_CreateSalesOrder @customer_id, @order_details, @notes, @created_by, @order_id OUTPUT, @order_number OUTPUT, @error_message OUTPUT",
                customerIdParam, orderDetailsParam, notesParam, createdByParam,
                orderIdParam, orderNumberParam, errorMessageParam
            );

            var orderId = orderIdParam.Value != DBNull.Value ? (int)orderIdParam.Value : 0;
            var orderNumber = orderNumberParam.Value != DBNull.Value ? orderNumberParam.Value.ToString() : null;
            var errorMessage = errorMessageParam.Value != DBNull.Value ? errorMessageParam.Value.ToString() : null;

            return (orderId, orderNumber, errorMessage);
        }

        public async Task<SalesOrder> GetSalesOrderByIdAsync(int salesOrderId)
        {
            return await _context.SalesOrders
                .Include(so => so.Customer)
                .Include(so => so.SalesOrderDetails)
                    .ThenInclude(sod => sod.Product)
                .FirstOrDefaultAsync(so => so.SalesOrderId == salesOrderId);
        }

        public async Task<string> GetNextOrderNumberAsync()
        {
            // Opción 1: Usando consulta SQL directa (más eficiente)
            var nextNumber = await _context.SalesOrders
                .OrderByDescending(so => so.SalesOrderId)
                .Select(so => so.SalesOrderId)
                .FirstOrDefaultAsync();

            // Si no hay órdenes, nextNumber será 0
            nextNumber += 1;

            return $"ORD-{nextNumber:000000}";
        }

        public async Task<(List<SalesOrder> Results, int TotalRecords)> SearchSalesOrdersAsync(
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

            try
            {
                var orders = await _context.SalesOrders
    .FromSqlRaw("EXEC sp_SearchSalesOrders @search_term, @page_number, @page_size, @total_records OUTPUT",
        searchTermParam, pageNumberParam, pageSizeParam, totalRecordsParam)
    .AsNoTracking() // opcional, mejora el rendimiento
    .ToListAsync();

                // Cargar relaciones manualmente
                foreach (var order in orders)
                {
                    _context.Entry(order).Reference(o => o.Customer).Load();
                }

                var totalRecords = (int)totalRecordsParam.Value;

                return (orders, totalRecords);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchSalesOrdersAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
