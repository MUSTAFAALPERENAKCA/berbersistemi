using System.ComponentModel.DataAnnotations;

namespace BarberShopProject.Models
{
   public class Stylist     
    {
        [Key]
        public int? StylistId { get; set; }

        // First name of the stylist
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Name must contain only letters.")]
        public string Name { get; set; }

        // Last name of the stylist
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Surname must contain only letters.")]
        public string Surname { get; set; }

        // Full name of the stylist (read-only property)
        public string FullName => $"{Name} {Surname}";

        // Start hour of the stylist's shift
        private DateTime? startHour;
        [DataType(DataType.Time)]
        [Display(Name = "Start Hour")]
        public DateTime? StartHour
        {
            get => startHour;
            set
            {
                startHour = value;
                UpdateShift();
            }
        }

        // End hour of the stylist's shift
        private DateTime? endHour;
        [DataType(DataType.Time)]
        [Display(Name = "End Hour")]
        public DateTime? EndHour
        {
            get => endHour;
            set
            {
                endHour = value;
                UpdateShift();
            }
        }

        // Shift duration formatted as "HH:mm - HH:mm"
        public string Shift { get; private set; }

        // Updates the Shift property whenever StartHour or EndHour is modified
        public void UpdateShift()
        {
            Shift = $"{StartHour?.ToString("HH:mm")} - {EndHour?.ToString("HH:mm")}";
        }

        // The shop or salon the stylist works at
        public int? ShopId { get; set; }
        public Shop? Shop { get; set; }

        // Collection of appointments associated with the stylist
        public ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();
    }
}
