using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplicationHospital.Data;
using WebApplicationHospital.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApplicationHospital.Controllers
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
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Professor"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var schedules = await _context.Calendars.ToListAsync();
            return View(schedules);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Professor"))
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

            if (user == null || !await _userManager.IsInRoleAsync(user, "Professor"))
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

            if (user == null || !await _userManager.IsInRoleAsync(user, "Professor"))
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

            if (user == null || !await _userManager.IsInRoleAsync(user, "Professor"))
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

            if (user == null || !await _userManager.IsInRoleAsync(user, "Professor"))
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

            if (user == null || !await _userManager.IsInRoleAsync(user, "Professor"))
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