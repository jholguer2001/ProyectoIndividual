using System.ComponentModel.DataAnnotations;

namespace POS.DTOs
{
    public class CreateSalesOrderDTO
    {
        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        public string? Notes { get; set; }

        [Required(ErrorMessage = "At least one product is required")]
        [MinLength(1, ErrorMessage = "At least one product is required")]
        public List<SalesOrderDetailDTO> OrderDetails { get; set; } = new();
    }

    public class SalesOrderDetailDTO
    {
        [Required(ErrorMessage = "Product is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal UnitPrice { get; set; }

        // Additional properties for display
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class SalesOrderViewModel
    {
        public int SalesOrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }

        // Customer information
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerIdentification { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }

        // Order details
        public List<SalesOrderDetailViewModel> OrderDetails { get; set; } = new();
    }

    public class SalesOrderDetailViewModel
    {
        public int SalesOrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class SalesOrderSearchDTO
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public List<SalesOrderSummaryDTO> Results { get; set; } = new();
    }

    public class SalesOrderSummaryDTO
    {
        public int SalesOrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
    }

    public class TransactionResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? OrderId { get; set; }
        public string OrderNumber { get; set; }
    }
}
