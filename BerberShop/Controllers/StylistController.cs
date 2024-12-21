using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BarberShop.Data;
using BarberShop.Models;

namespace HospitalApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StylistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StylistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var stylistRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Stylist");

            if (stylistRole == null)
            {
                return NotFound();
            }

            var stylists = await _context.UserRoles
                .Where(ur => ur.RoleId == stylistRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var stylistUsers = await _context.Users
                .Include(u => u.Departments) // Include the Department navigation property
                .Where(u => stylists.Contains(u.Id))
                .ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            ViewData["IsAdmin"] = isAdmin;
            ViewData["Title"] = "Stylist";

            return View(stylistUsers);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(string id)
        {
            var stylist = await _context.Users.Include(u => u.Departments).FirstOrDefaultAsync(u => u.Id == id);
            if (stylist == null)
            {
                return NotFound();
            }
            return View(stylist);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var stylistToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (stylistToUpdate == null) 
            {
                return NotFound();
            }

            stylistToUpdate.Name = model.Name;
            stylistToUpdate.Email = model.Email;
            stylistToUpdate.PhoneNumber = model.PhoneNumber;
            stylistToUpdate.DepartmentId = model.DepartmentId; // Corrected: Assign DepartmentId directly

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "The record was updated by another user.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string id)
        {
            var stylist = await _context.Users.Include(u => u.Departments).FirstOrDefaultAsync(u => u.Id == id);
            if (stylist == null)
            {
                return NotFound();
            }
            return View(stylist);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var stylist = await _context.Users.FindAsync(id);
            if (stylist != null)
            {
                _context.Users.Remove(stylist);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
