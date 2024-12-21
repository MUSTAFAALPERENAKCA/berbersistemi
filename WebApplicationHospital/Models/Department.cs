

namespace WebApplicationHospital.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }  // Departman adı
        public string Description { get; set; }  // Departman açıklaması

        // Bir departmanda birden fazla kullanıcı olabilir
        public ICollection<ApplicationUser> Users { get; set; }
    }

}
