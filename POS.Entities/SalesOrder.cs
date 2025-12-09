using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Entities
{
    [Table("Sales_Orders")]
    public class SalesOrder
    {
        [Key]
        [Column("sales_order_id")]
        public int SalesOrderId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("order_number")]
        public string OrderNumber { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column("subtotal", TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column("tax_amount", TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column("total_amount", TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; } = "COMPLETED";

        [StringLength(500)]
        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(100)]
        [Column("created_by")]
        public string CreatedBy { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }
    }
}
