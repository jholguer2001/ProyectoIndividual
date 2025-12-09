using POS.DTOs;

namespace POS.Services.Interfaces
{
    public interface ISalesOrderService
    {
        Task<TransactionResultDTO> CreateSalesOrderAsync(CreateSalesOrderDTO orderDto, string createdBy);
        Task<SalesOrderViewModel> GetSalesOrderByIdAsync(int salesOrderId);
        Task<SalesOrderSearchDTO> SearchSalesOrdersAsync(string searchTerm, int pageNumber, int pageSize);
        Task<ApiResponse<decimal>> CalculateTotalsAsync(CreateSalesOrderDTO orderDto);
        Task<string> GetNextOrderNumberAsync();
    }
}
