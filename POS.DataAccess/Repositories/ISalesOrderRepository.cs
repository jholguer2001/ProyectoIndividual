using POS.Entities;

namespace POS.DataAccess.Repositories
{
    public interface ISalesOrderRepository : IRepository<SalesOrder>
    {
        Task<(int OrderId, string OrderNumber, string ErrorMessage)> CreateSalesOrderAsync(
            int customerId, List<(int ProductId, int Quantity, decimal UnitPrice)> orderDetails,
            string notes, string createdBy);
        Task<string> GetNextOrderNumberAsync();
        Task<SalesOrder> GetSalesOrderByIdAsync(int salesOrderId);
        Task<(List<SalesOrder> Results, int TotalRecords)> SearchSalesOrdersAsync(
            string searchTerm, int pageNumber, int pageSize);
    }
}
