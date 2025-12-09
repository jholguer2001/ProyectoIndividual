using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Entities
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("identification_number")]
        public string IdentificationNumber { get; set; }

        [Required]
        [StringLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Column("last_name")]
        public string LastName { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Column("email")]
        public string? Email { get; set; }

        [StringLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [StringLength(300)]
        [Column("address")]
        public string? Address { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
