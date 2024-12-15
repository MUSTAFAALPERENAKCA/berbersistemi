using BarberShopProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberShopProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult LoadSection(string section)
        {
            if (section == "_section1") // Stylists Management
            {
                var viewModel = new AdminStylistViewModel
                {
                    Stylists = _context.Stylists.ToList(),
                    Shops = _context.Shops.ToList(),
                    Stylist = new Stylist(),
                };
                return View(section, viewModel);
            }
            else if (section == "_section2") // Shops Management
            {
                var viewModel = new AdminShopViewModel
                {
                    Shops = _context.Shops
                        .Include(s => s.Stylists) // Include stylists associated with shops
                        .ToList(),
                    Shop = new Shop()
                };
                return View(section, viewModel);
            }
            else if (section == "_section3") // Customers Management
            {
                var customers = _context.Customers.ToList();
                return View(section, customers);
            }
            else if (section == "_section4") // Appointments Management
            {
                var appointments = _context.Appointments
                    .Include(a => a.Stylist) // Include related stylist
                    .Include(a => a.Shop)    // Include related shop
                    .ToList();

                return View(section, appointments);
            }
            return View(section);
        }

        public IActionResult AdminPanel()
        {
            return RedirectToAction("LoadSection", new { section = "_section1" });
        }

        [HttpPost]
        public IActionResult ShopRegister(Shop shop)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var shopExists = _context.Shops.Any(s => s.ShopId == shop.ShopId);

                    if (shopExists)
                    {
                        var existingShop = _context.Shops.Find(shop.ShopId);
                        _context.Entry(existingShop).CurrentValues.SetValues(shop);
                        _context.Entry(existingShop).State = EntityState.Modified;
                        Console.WriteLine("Shop modified");
                    }
                    else
                    {
                        _context.Shops.Add(shop);
                        Console.WriteLine("Shop added");
                    }
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the shop. Please try again.");
                }
                return RedirectToAction("AdminPanel");
            }
            else
            {
                var viewModel = new AdminShopViewModel
                {
                    Shops = _context.Shops.ToList(),
                    Shop = shop
                };
                return View("_section2", viewModel);
            }
        }

        [HttpPost]
        public IActionResult ShopRemove(int id)
        {
            var shop = _context.Shops.Include(s => s.Stylists).FirstOrDefault(s => s.ShopId == id);

            if (shop == null)
                return NotFound();

            try
            {
                if (shop.Stylists != null && shop.Stylists.Any())
                {
                    var stylistsToDelete = _context.Stylists.Where(s => s.ShopId == id);
                    _context.Stylists.RemoveRange(stylistsToDelete);

                    shop.Stylists = null;
                    _context.SaveChanges();
                }
                _context.Shops.Remove(shop);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while trying to remove the shop.");
            }
            return RedirectToAction("AdminPanel");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterStylist(Stylist stylist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var stylistExists = _context.Stylists.Any(s => s.StylistId == stylist.StylistId);
                    Shop shop = _context.Shops.Find(stylist.ShopId);

                    if (stylistExists)
                    {
                        var existingStylist = _context.Stylists.Find(stylist.StylistId);
                        _context.Entry(existingStylist).CurrentValues.SetValues(stylist);
                        _context.Entry(existingStylist).State = EntityState.Modified;
                        Console.WriteLine("Stylist modified");
                    }
                    else
                    {
                        _context.Stylists.Add(stylist);
                        shop.Stylists.Add(stylist);
                        Console.WriteLine("Stylist added");
                    }
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the stylist. Please try again.");
                }
                return RedirectToAction("AdminPanel");
            }
            else
            {
                var viewModel = new AdminStylistViewModel
                {
                    Stylist = stylist,
                    Stylists = _context.Stylists.ToList(),
                    Shops = _context.Shops.ToList()
                };
                return View("_section1", viewModel);
            }
        }

        [HttpPost]
        public IActionResult StylistRemove(int id)
        {
            var stylist = _context.Stylists.Find(id);
            if (stylist == null)
                return NotFound();

            var appointmentsToDelete = _context.Appointments.Where(a => a.StylistId == id);
            _context.Appointments.RemoveRange(appointmentsToDelete);

            stylist.ShopId = null;

            _context.SaveChanges();

            _context.Stylists.Remove(stylist);
            _context.SaveChanges();

            return RedirectToAction("AdminPanel");
        }
    }
}