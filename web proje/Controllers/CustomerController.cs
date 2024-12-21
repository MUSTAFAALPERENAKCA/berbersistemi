using BarberShopProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberShopProject.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult CustomerPanel()
        {
            return RedirectToAction("LoadSection", new { section = "_section1" });
        }

        public async Task<IActionResult> LoadSection(string section)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            if (userId == null)
            {
                return Unauthorized();
            }

            switch (section)
            {
                case "_section1": // View Appointments
                    return await LoadAppointmentsSection(userId);

                case "_section2": // Book New Appointment
                    return LoadBookingSection(userId);

                case "_section3": // Profile Management
                    return LoadProfileSection(userId);

                default:
                    return NotFound();
            }
        }

        private async Task<IActionResult> LoadAppointmentsSection(string userId)
        {
            var customer = await _context.Customers
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.Stylist)
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.Shop)
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer != null)
            {
                return View("_section1", customer.Appointments);
            }
            return NotFound();
        }

        private IActionResult LoadBookingSection(string userId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == userId);

            if (customer != null)
            {
                var viewModel = new CustomerViewModel
                {
                    Customer = customer,
                    Appointment = new Appointment(),
                    Shops = _context.Shops.ToList(),
                    Stylists = _context.Stylists.ToList()
                };
                return View("_section2", viewModel);
            }
            return NotFound();
        }

        private IActionResult LoadProfileSection(string userId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == userId);

            if (customer != null)
            {
                return View("_section3", customer);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CustomerViewModel
                {
                    Appointment = appointment,
                    Shops = await _context.Shops.ToListAsync(),
                    Stylists = await _context.Stylists.ToListAsync()
                };
                return View("_section2", viewModel);
            }

            try
            {
                var shop = await _context.Shops.FindAsync(appointment.ShopId);
                var stylist = await _context.Stylists.FindAsync(appointment.StylistId);
                var customer = await _context.Customers.FindAsync(appointment.CustomerId);

                if (shop == null || stylist == null || customer == null)
                {
                    ModelState.AddModelError(string.Empty, "Shop, Stylist, or Customer not found.");
                    return RedirectToAction("CustomerPanel");
                }

                appointment.Shop = shop;
                appointment.Stylist = stylist;
                appointment.Customer = customer;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                Console.WriteLine("Appointment added");
                return RedirectToAction("CustomerPanel");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while saving the appointment: {ex.Message}");
                Console.Error.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("CustomerPanel");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Shop)
                .Include(a => a.Stylist)
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            try
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                Console.WriteLine("Appointment canceled");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while trying to remove the appointment.");
                Console.Error.WriteLine($"Error: {ex.Message}");
            }

            return RedirectToAction("CustomerPanel");
        }
    }
}
