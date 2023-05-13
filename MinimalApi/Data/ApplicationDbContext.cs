using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Core;
using MinimalApi.Data.Entity;

namespace MinimalApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } 


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            modelBuilder.Entity<IdentityRole>()
                .HasData(
                   new IdentityRole()
                   { 
                       Name = Constants.Admin,
                       NormalizedName = "ADMIN"
                   },
                   new IdentityRole()
                   {
                       Name = Constants.User,
                       NormalizedName = "USER"
                   }
                );
        }
    } 
}
