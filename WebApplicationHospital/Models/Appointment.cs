using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationHospital.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public int CalendarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsBooked { get; set; }
        public string AssistantName { get; set; }
    }
}