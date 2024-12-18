using System.ComponentModel.DataAnnotations;

namespace HospitalProject.Models
{
    public class Department
    {
        [Key]
        public int? DepartmentId { get; set; }
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Department name must contain only letters.")]
        public string DepartmentName { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Doctor>? Doctors { get; set; } = new List<Doctor>();
        public ICollection<Appointment>? Appointments { get; set; } = new List<Appointment>();
    }
}