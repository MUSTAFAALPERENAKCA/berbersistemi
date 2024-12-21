using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BarberShopProject.Models
{

    public class Customer : ApplicationUser   // inherits IdentityUser
    {
        // Collection of appointments associated with the customer
        public ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();
    }

}