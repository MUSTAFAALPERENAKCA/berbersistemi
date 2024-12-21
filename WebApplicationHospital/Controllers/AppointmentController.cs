using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationHospital.Data;
using WebApplicationHospital.Models;
using System.Linq;
using System.Threading.Tasks;

public class AppointmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AppointmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Müsait zamanları listele
    //public async Task<IActionResult> Index()
    //{
    //    // Sadece profesörlerin oluşturduğu zamanlar
    //    var availableCalendars = await _context.Calendars
    //        .Where(c => c.StartDate >= DateTime.Now)
    //        .Include(c => c.Assignments)
    //        .ToListAsync();

    //    return View(availableCalendars);
    //}


    // Randevu al (GET)
    [HttpGet]
    public async Task<IActionResult> Book(int calendarId)
    {
        var calendar = await _context.Calendars
            .Include(c => c.Assignments)
            .FirstOrDefaultAsync(c => c.Id == calendarId);

        if (calendar == null)
        {
            return NotFound("Calendar not found.");
        }

        return View(calendar);
    }

    [HttpPost]
    public async Task<IActionResult> BookPost(int calendarId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var calendar = await _context.Calendars
            .Include(c => c.Assignments)
            .FirstOrDefaultAsync(c => c.Id == calendarId);

        if (calendar == null || calendar.Assignments.Any())
        {
            // Calendar bulunamadı veya zaten alınmış
            return RedirectToAction("Index");
        }

        // Randevu ataması
        var assignment = new Assignment
        {
            AssistantId = user.Id,
            CalendarId = calendarId
        };

        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Index()
    {
        // Alınan ve alınmayan randevuları listele
        var allCalendars = await _context.Calendars
            .Include(c => c.Assignments)
            .ToListAsync();

        var viewModel = allCalendars.Select(calendar =>
        {
            var firstAssignment = calendar.Assignments.FirstOrDefault();
            var assistantName = firstAssignment != null
                ? _context.Users.FirstOrDefault(u => u.Id == firstAssignment.AssistantId)?.UserName
                : null;

            return new Appointment
            {
                CalendarId = calendar.Id,
                StartDate = calendar.StartDate,
                EndDate = calendar.EndDate,
                IsBooked = calendar.Assignments.Any(),
                AssistantName = assistantName
            };
        }).ToList();

        return View(viewModel);
    }
}
