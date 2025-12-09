using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("product_code")]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(200)]
        [Column("product_name")]
        public string ProductName { get; set; }

        [StringLength(500)]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("unit_price", TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column("stock_quantity")]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("taxable")]
        public bool Taxable { get; set; } = true;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
