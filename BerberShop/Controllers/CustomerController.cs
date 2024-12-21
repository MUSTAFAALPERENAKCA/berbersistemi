using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberShop.Data;
using BarberShop.Models;

namespace HospitalApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CustomerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // View customer list for everyone
        public async Task<IActionResult> Index()
        {
            var customerRole = await _context.Roles
                .Where(r => r.Name == "Customer")
                .FirstOrDefaultAsync();

            if (customerRole == null)
            {
                return NotFound();
            }

            var customers = await _context.UserRoles
                .Where(ur => ur.RoleId ==  customerRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var customerUsers = await _context.Users
                .Where(u => customers.Contains(u.Id))
                .ToListAsync();

            // Check if the current user is an Admin
            var currentUser = await _userManager.GetUserAsync(User);
            bool isAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            ViewData["IsAdmin"] = isAdmin; // Pass this to the view
            ViewData["Title"] = "Customer";

            return View(customerUsers);
        }



        // GET: Edit
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _userManager.FindByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            // Kullanıcıyı viewmodel'e veya direkt olarak kullanıcıyı model olarak gönderiyoruz
            return View(customer);  // View'da ApplicationUser modelini kullanıyoruz
        }

        // POST: Edit
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (userToUpdate == null)
            {
                return NotFound();
            }

            userToUpdate.Name = model.Name;
            userToUpdate.Email = model.Email;
            userToUpdate.PhoneNumber = model.PhoneNumber;
            userToUpdate.DepartmentId = model.DepartmentId;
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


        // GET /Assistant/Delete/{id}
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var customer = await _context.Users.FindAsync(id);
            if (customer == null)
            {
                return NotFound(); // Kullanıcı bulunamadıysa hata döner
            }
            return View(customer); // Kullanıcı bilgilerini Delete sayfasında gösterir
        }


        // POST /Assistant/Delete/{id} -> Silme işlemi yapılır
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await _context.Users.FindAsync(id);
            if (customer == null)
            {
                return NotFound(); // Kullanıcı bulunamadıysa hata döner
            }

            _context.Users.Remove(customer); // Kullanıcıyı sileriz
            await _context.SaveChangesAsync(); // Değişiklikleri kaydederiz

            return RedirectToAction(nameof(Index)); // Silme başarılıysa listeye yönlendirir
        }

    }
}
