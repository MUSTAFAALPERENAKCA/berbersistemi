using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationHospital.Data;
using WebApplicationHospital.Models;

public class DepartmentController : Controller
{
    private readonly ApplicationDbContext _context;

    public DepartmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Kullanıcıları departmana göre grupla, ancak Departmanı null veya boş olmayanları seç
        var departmentsWithUsers = await _context.Departments
.Where(d => _context.Users.Any(u => u.DepartmentId == d.Id && u.DepartmentId != null))
            .GroupBy(d => d.Name) // Aynı adı olan departmanları grupla
            .Select(group => new
            {
                DepartmentName = group.Key,
                Users = group.SelectMany(d => _context.Users.Where(u => u.DepartmentId == d.Id)).ToList() // Grubun kullanıcılarını listele
            })
            .ToListAsync();

        // Görüntü için departmanları model olarak gönderiyoruz
        return View(departmentsWithUsers);
    }


    // Diğer metotlar (Create, Edit, Delete) burada devam edebilir
}
