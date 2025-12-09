using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Entities
{
    [Table("Sales_Order_Details")]
    public class SalesOrderDetail
    {
        [Key]
        [Column("sales_order_detail_id")]
        public int SalesOrderDetailId { get; set; }

        [Required]
        [Column("sales_order_id")]
        public int SalesOrderId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("quantity")]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column("unit_price", TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Column("line_total", TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("SalesOrderId")]
        public virtual SalesOrder SalesOrder { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
