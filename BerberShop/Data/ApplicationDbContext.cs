using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using BarberShop.Models;

namespace BarberShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Assignment>()
                .HasKey(a => a.Id);

            builder.Entity<Calendar>()
                .HasMany(c => c.Assignments)  // Correctly referencing Assignments
                .WithOne(a => a.Calendar)
                .HasForeignKey(a => a.CalendarId);

            builder.Entity<Assignment>()
                .HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.AssistantId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict to prevent cascading delete
        }
    }
}
