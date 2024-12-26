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
                .Where(a => a.IsPending && !a.IsConfirmed) // Sadece onay bekleyen randevuları getir
                .Include(a => a.Stylist)
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
        public async Task<IActionResult> Create(Schedule schedule)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Stylist"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            if (ModelState.IsValid)
            {
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(schedule);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Schedule schedule)
        {
            if (id != schedule.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Schedules.Update(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(schedule);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
