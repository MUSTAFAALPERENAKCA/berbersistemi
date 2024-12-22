using BarberShop.Data;
using BarberShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BarberShop.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Randevu listesi
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Stylist)
                .ToListAsync();
            return View(appointments);
        }

        // Randevu oluştur (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var stylists = await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                                                         _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Stylist")))
                .ToListAsync();
            ViewData["Stylists"] = stylists;
            return View();
        }

        // Randevu oluştur (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Appointment model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Randevu çakışma kontrolü
            var conflict = await _context.Appointments
                .AnyAsync(a => a.StylistId == model.StylistId &&
                               ((model.StartDate >= a.StartDate && model.StartDate < a.EndDate) ||
                                (model.EndDate > a.StartDate && model.EndDate <= a.EndDate)));

            if (conflict)
            {
                ModelState.AddModelError("", "Bu zaman dilimi dolu. Lütfen başka bir zaman seçin.");
                return View(model);
            }

            _context.Appointments.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Randevu onayla
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.IsConfirmed = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
    public async Task<IActionResult> GenerateWeeklyAppointments()
{
        // "Stylist" rolündeki kullanıcıları bul
        var stylistRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Stylist");
        if (stylistRole == null)
        {
            return NotFound("Stylist role not found.");
        }

        // Stylist rolüne sahip kullanıcıların Id'lerini al
        var stylistIds = await _context.UserRoles
            .Where(ur => ur.RoleId == stylistRole.Id)
            .Select(ur => ur.UserId)
            .ToListAsync();

        // Bu kullanıcıların detaylarını al
        var stylists = await _context.Users
            .Where(u => stylistIds.Contains(u.Id))
            .ToListAsync();

        // Tüm stilistler için haftalık randevular oluştur
        foreach (var stylist in stylists)
        {
            for (int i = 0; i < 7; i++) // Haftanın her günü
            {
                var currentDate = DateTime.Today.AddDays(i);

                for (int hour = 8; hour < 17; hour++) // 08:00 - 17:00 saatleri
                {
                    var startTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, hour, 0, 0);
                    var endTime = startTime.AddHours(1);

                    // Zaten var olan bir randevu kontrolü
                    var exists = await _context.Appointments.AnyAsync(a =>
                        a.StylistId == stylist.Id &&
                        a.StartDate == startTime &&
                        a.EndDate == endTime);

                    if (!exists)
                    {
                        var appointment = new Appointment
                        {
                            StylistId = stylist.Id,
                            StartDate = startTime,
                            EndDate = endTime,
                            Service = "Standard Service", // Varsayılan hizmet adı
                            Price = 100, // Varsayılan fiyat
                            Duration = 60, // 60 dakika
                            IsConfirmed = false, // Varsayılan olarak onaysız
                            //AssistantName = appointment.AssistantName,
                        };
                        _context.Appointments.Add(appointment);
                    }
                }
            }
        }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Randevu zaten onaylıysa, kullanıcıyı uyarabiliriz
            if (appointment.IsConfirmed)
            {
                return RedirectToAction(nameof(Index)); // Randevu zaten onaylı, listeye geri dön
            }

            // Randevu bilgilerini göstereceğiz
            return View(appointment);
        }


        [HttpPost]
        public async Task<IActionResult> BookPost(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Eğer randevu zaten onaylanmışsa, işlem yapma
            if (appointment.IsConfirmed)
            {
                ModelState.AddModelError("", "This appointment is already confirmed.");
                return RedirectToAction(nameof(Index));
            }

            // Talep oluşturma işlemi (IsPending true olarak bırakılır)
            appointment.IsPending = true;
            await _context.SaveChangesAsync();

            // Stilistin onay sayfasına yönlendirme (örnek)
            return RedirectToAction("PendingApprovals", "Schedule");
        }

    }
}
