using System.ComponentModel.DataAnnotations;

namespace BarberShopProject.Models
{
    public class Service
    {
        // Primary key for the Service model
        [Key]
        public int? ServiceId { get; set; }

        // Name of the service (e.g., Haircut, Shaving)
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Service name must contain only letters.")]
        public string ServiceName { get; set; }

        // Duration of the service in minutes
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes.")]
        public int DurationInMinutes { get; set; }

        // Price of the service
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        // A collection of stylists who can provide this service
        public ICollection<Stylist>? Stylists { get; set; } = new List<Stylist>();

        // A collection of appointments associated with this service
        public ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();
    }
}
