using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationHospital.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public int DepartmentId { get; set; } // Departman ID'si

    }
}
