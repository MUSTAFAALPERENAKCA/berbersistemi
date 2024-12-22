using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BarberShop.Data;
using BarberShop.Models;

public class DepartmentController : Controller
{
    private readonly ApplicationDbContext _context;

    public DepartmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // "Stylist" rolüne sahip kullanıcıları ve onların departmanlarını listele
        var departmentsWithStylists = await _context.Departments
            .Where(d => _context.Users.Any(u => u.DepartmentId == d.Id && u.DepartmentId != null &&
                                                 _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                                                                              _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Stylist"))))
            .GroupBy(d => d.Name) // Departmanları adlarına göre grupla
            .Select(group => new
            {
                DepartmentName = group.Key,
                Users = group.SelectMany(d => _context.Users
                    .Where(u => u.DepartmentId == d.Id &&
                                _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                                                             _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Stylist")))
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.UserName,
                        u.DepartmentId // Kullanıcının departman ID'sini de ekle
                    })).ToList()
            })
            .ToListAsync();

        // Görüntü için departmanları model olarak gönderiyoruz
        return View(departmentsWithStylists);
    }




    // Diğer metotlar (Create, Edit, Delete) burada devam edebilir
}
