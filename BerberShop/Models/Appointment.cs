using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BarberShop.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StylistId { get; set; } // Randevu alınacak çalışan

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Service { get; set; }

        public decimal Price { get; set; } // Ücret

        public int Duration { get; set; } // Süre (dakika)

        public bool IsConfirmed { get; set; } = false; // Onay durumu
        public bool IsPending { get; set; } = false;   // Talep durum


        [ForeignKey("StylistId")]
        public ApplicationUser Stylist { get; set; }

        [NotMapped] // Veritabanında saklanmaz
        public string AssistantName => Stylist?.UserName ?? "Unknown"; // Stilist adı


    }
}
