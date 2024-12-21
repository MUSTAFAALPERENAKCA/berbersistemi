using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationHospital.Models
{
    public class Calendar
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; } // Başlangıç tarihi ve zamanı

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; } // Bitiş tarihi ve zamanı

        public ICollection<Assignment>? Assignments { get; set; }
    }
}