using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BarberShop.Data;
using BarberShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BarberShop.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ScheduleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Where(a => a.IsPending && !a.IsConfirmed) // IsPending = true ve IsConfirmed = false olanları filtrele
                .Select(a => new Appointment
                {
                    Id = a.Id,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Service = a.Service,
                    //CustomerName = a.Customer.Name, 
                    //StylistName = a.Stylist.Name,      // !!!! NOT: en son IsPending = true ve IsConfirmed = false olanları filtrele bu sekılde lıstelemeye calsıtım schedule/ındex sayfasında ama olmadı 
                    IsPending = a.IsPending,
                    IsConfirmed = a.IsConfirmed
                })
                .ToListAsync();

            return View(appointments);
        }



        public async Task<IActionResult> PendingApprovals()
        {
            var pendingAppointments = await _context.Appointments
                .Where(a => a.IsPending && !a.IsConfirmed)
                .Include(a => a.Stylist)
                .ToListAsync();

            return View(pendingAppointments);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Stilist onayı
            appointment.IsPending = false;
            appointment.IsConfirmed = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingApprovals));
        }

        [HttpPost]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Randevu talebini reddet ve sil
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PendingApprovals));
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Stylist"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }

        [HttpPost("Schedule/Create")]
        public async Task<IActionResult> Create(Calendar calendar)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Stylist"))
            {
                return RedirectToAction("AccessDenied", "Schedule");
            }

            // Tarih aralığı doğrulaması
            if (calendar.StartDate >= calendar.EndDate)
            {
                ModelState.AddModelError("", "End date must be greater than start date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Calendars.Add(calendar);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Calendar ID: {calendar.Id}, StartDate: {calendar.StartDate}, EndDate: {calendar.EndDate} successfully saved.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database Save Error: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the data. Please try again.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Validation failed. Please check the input values.");
            }

            return View(calendar);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Stylist"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var calendar = await _context.Calendars.FindAsync(id);
            if (calendar == null)
            {
                return NotFound("Calendar not found.");
            }

            return View(calendar);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Calendar calendar)
        {
            if (id != calendar.Id)
            {
                return BadRequest("Invalid calendar ID.");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Stylist"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            try
            {
                _context.Calendars.Update(calendar);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Calendar ID: {calendar.Id} successfully updated.");
                return RedirectToAction("Index", "Schedule", new { calendarId = calendar.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Update Error: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the data. Please try again.");
                return View(calendar);
            }
          
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Stylist"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var calendar = await _context.Calendars.FindAsync(id);
            if (calendar == null)
            {
                return NotFound("Calendar not found.");
            }

            return View(calendar);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Stylist"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var calendar = await _context.Calendars.FindAsync(id);
            if (calendar == null)
            {
                return NotFound("Calendar not found.");
            }

            try
            {
                _context.Calendars.Remove(calendar);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Calendar ID: {id} successfully deleted.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Delete Error: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while deleting the data. Please try again.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}