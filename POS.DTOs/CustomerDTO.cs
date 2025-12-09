using System.ComponentModel.DataAnnotations;

namespace POS.DTOs
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Identification number is required")]
        [RegularExpression(@"^\d{10,13}$", ErrorMessage = "Invalid identification number format")]
        public string IdentificationNumber { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Only letters allowed")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Only letters allowed")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone format")]
        public string Phone { get; set; }

        public string Address { get; set; }
        public string FullName { get; set; }
    }

    public class CustomerSearchDTO
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public List<CustomerDTO> Results { get; set; } = new();
    }
}
