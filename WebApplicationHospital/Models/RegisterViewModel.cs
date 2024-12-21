using System.ComponentModel.DataAnnotations;

namespace WebApplicationHospital.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
