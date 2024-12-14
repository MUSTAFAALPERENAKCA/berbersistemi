using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BarberShopProject.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "User Password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }

    public class RegisterViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters.")]
        [MaxLength(16)]
        [Display(Name = "Your name")]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Surname must contain only letters.")]
        [MaxLength(16)]
        [Display(Name = "Your surname")]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class AdminStylistViewModel
    {
        public Stylist Stylist { get; set; }
        public List<Stylist> Stylists { get; set; }
        public List<Shop>? Shops { get; set; } = new List<Shop>();
    }

    public class AdminShopViewModel
    {
        public Shop Shop { get; set; }
        public List<Shop> Shops { get; set; }
    }

    public class CustomerViewModel // for booking new appointments
    {
        public Customer? Customer { get; set; }
        public Appointment? Appointment { get; set; }
        public List<Shop>? Shops { get; set; } = new List<Shop>();
        public List<Stylist>? Stylists { get; set; } = new List<Stylist>();
    }
}