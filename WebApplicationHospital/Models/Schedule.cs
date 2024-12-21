using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationHospital.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Day { get; set; } // Gün bilgisi (örneğin Salı)

        [Required]
        public string StartTime { get; set; } // Başlangıç saati

        [Required]
        public string EndTime { get; set; } // Bitiş saati

        // Navigation Property
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}