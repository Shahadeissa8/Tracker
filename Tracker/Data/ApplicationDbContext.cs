using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tracker.Models;

namespace Tracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        //public DbSet<ApplicationUser> Gender { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<Expense> Expenses { get; set; }
      

    }
}

