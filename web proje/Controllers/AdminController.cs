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
            var viewModel = GetViewModelForSection(section);
            if (viewModel == null)
            {
                return View(section);
            }
            return View(section, viewModel);
        }

        private object GetViewModelForSection(string section)
        {
            return section switch
            {
                "_section1" => new AdminStylistViewModel
                {
                    Stylists = _context.Stylists.ToList(),
                    Shops = _context.Shops.ToList(),
                    Stylist = new Stylist(),
                },
                "_section2" => new AdminShopViewModel
                {
                    Shops = _context.Shops.Include(s => s.Stylists).ToList(),
                    Shop = new Shop(),
                },
                "_section3" => _context.Customers.ToList(),
                "_section4" => _context.Appointments
                    .Include(a => a.Stylist)
                    .Include(a => a.Shop)
                    .ToList(),
                _ => null,
            };
        }

        public IActionResult AdminPanel()
        {
            return RedirectToAction("LoadSection", new { section = "_section1" });
        }

        [HttpPost]
        public async Task<IActionResult> ShopRegisterAsync(Shop shop)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var shopExists = await _context.Shops.AnyAsync(s => s.ShopId == shop.ShopId);

                    if (shopExists)
                    {
                        var existingShop = await _context.Shops.FindAsync(shop.ShopId);
                        _context.Entry(existingShop).CurrentValues.SetValues(shop);
                        _context.Entry(existingShop).State = EntityState.Modified;
                    }
                    else
                    {
                        await _context.Shops.AddAsync(shop);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the shop. Please try again.");
                    Console.Error.WriteLine($"Error: {ex.Message}");
                }
                return RedirectToAction("AdminPanel");
            }
            else
            {
                var viewModel = new AdminShopViewModel
                {
                    Shops = await _context.Shops.ToListAsync(),
                    Shop = shop,
                };
                return View("_section2", viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ShopRemoveAsync(int id)
        {
            var shop = await _context.Shops.Include(s => s.Stylists).FirstOrDefaultAsync(s => s.ShopId == id);

            if (shop == null)
                return NotFound();

            try
            {
                if (shop.Stylists != null && shop.Stylists.Any())
                {
                    var stylistsToDelete = _context.Stylists.Where(s => s.ShopId == id);
                    _context.Stylists.RemoveRange(stylistsToDelete);

                    shop.Stylists = null;
                    await _context.SaveChangesAsync();
                }
                _context.Shops.Remove(shop);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while trying to remove the shop.");
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
            return RedirectToAction("AdminPanel");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStylistAsync(Stylist stylist)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var stylistExists = await _context.Stylists.AnyAsync(s => s.StylistId == stylist.StylistId);
                    var shop = await _context.Shops.FindAsync(stylist.ShopId);

                    if (stylistExists)
                    {
                        var existingStylist = await _context.Stylists.FindAsync(stylist.StylistId);
                        _context.Entry(existingStylist).CurrentValues.SetValues(stylist);
                        _context.Entry(existingStylist).State = EntityState.Modified;
                    }
                    else
                    {
                        await _context.Stylists.AddAsync(stylist);
                        shop.Stylists.Add(stylist);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the stylist. Please try again.");
                    Console.Error.WriteLine($"Error: {ex.Message}");
                }
                return RedirectToAction("AdminPanel");
            }
            else
            {
                var viewModel = new AdminStylistViewModel
                {
                    Stylist = stylist,
                    Stylists = await _context.Stylists.ToListAsync(),
                    Shops = await _context.Shops.ToListAsync(),
                };
                return View("_section1", viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> StylistRemoveAsync(int id)
        {
            var stylist = await _context.Stylists.FindAsync(id);
            if (stylist == null)
                return NotFound();

            try
            {
                var appointmentsToDelete = _context.Appointments.Where(a => a.StylistId == id);
                _context.Appointments.RemoveRange(appointmentsToDelete);

                stylist.ShopId = null;

                await _context.SaveChangesAsync();

                _context.Stylists.Remove(stylist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while trying to remove the stylist.");
                Console.Error.WriteLine($"Error: {ex.Message}");
            }

            return RedirectToAction("AdminPanel");
        }
    }
}
