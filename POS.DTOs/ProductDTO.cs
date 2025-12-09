using System.ComponentModel.DataAnnotations;

namespace POS.DTOs
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product code is required")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }
        public bool Taxable { get; set; }
    }

    public class ProductSearchDTO
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public List<ProductDTO> Results { get; set; } = new();
    }
}
