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

            if (section == "_section1") // View Appointments
            {
                var customer = await _context.Customers
                    .Include(c => c.Appointments)
                        .ThenInclude(a => a.Stylist)
                    .Include(c => c.Appointments)
                        .ThenInclude(a => a.Shop)
                    .FirstOrDefaultAsync(c => c.Id == userId);

                if (customer != null)
                {
                    var customerAppointments = customer.Appointments;
                    return View(section, customerAppointments);
                }
                else
                {
                    return NotFound();
                }
            }
            else if (section == "_section2") // Book New Appointment
            {
                var customer = _context.Customers.FirstOrDefault(c => c.Id == userId);
                if (customer != null)
                {
                    var viewModel = new CustomerViewModel
                    {
                        Customer = customer,
                        Appointment = new Appointment(),
                        Shops = _context.Shops.ToList() ?? new List<Shop>(),
                        Stylists = _context.Stylists.ToList() ?? new List<Stylist>()
                    };
                    return View(section, viewModel);
                }
                else
                {
                    Console.WriteLine("Couldn't fetch the data");
                }
            }
            else if (section == "_section3") // Profile Management
            {
                var customer = _context.Customers.FirstOrDefault(c => c.Id == userId);
                if (customer != null)
                {
                    return View(section, customer);
                }
                else
                {
                    return NotFound();
                }
            }

            return View(section);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAppointment(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Shop shop = _context.Shops.Find(appointment.ShopId);
                    Stylist stylist = _context.Stylists.Find(appointment.StylistId);
                    Customer customer = _context.Customers.Find(appointment.CustomerId);

                    if (shop != null && stylist != null && customer != null)
                    {
                        appointment.Shop = shop;
                        appointment.Stylist = stylist;
                        appointment.Customer = customer;

                        // Add the appointment to the related collections
                        shop.Appointments.Add(appointment);
                        stylist.Appointments.Add(appointment);
                        customer.Appointments.Add(appointment);

                        // Add the appointment to the main Appointments DbSet
                        _context.Appointments.Add(appointment);

                        // Save changes
                        _context.SaveChanges();

                        Console.WriteLine("Appointment added");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Shop, Stylist, or Customer not found.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred while saving the appointment: {ex.Message}");
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return RedirectToAction("CustomerPanel");
            }
            else
            {
                CustomerViewModel viewModel = new CustomerViewModel
                {
                    Appointment = appointment
                };
                return View("_section2", viewModel);
            }
        }

        [HttpPost]
        public IActionResult CancelAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            try
            {
                if (appointment.Shop != null && appointment.Stylist != null && appointment.Customer != null)
                {
                    appointment.Shop = null;
                    appointment.Stylist = null;
                    appointment.Customer = null;
                    _context.SaveChanges();
                }
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while trying to remove the appointment.");
                Console.WriteLine($"Error: {ex.Message}");
            }

            return RedirectToAction("CustomerPanel");
        }
    }
}
