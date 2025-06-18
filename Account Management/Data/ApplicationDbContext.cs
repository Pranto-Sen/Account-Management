using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Account_Management.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRole = new IdentityRole
            {
                Id = "1", 
                Name = "Admin",
                NormalizedName = "ADMIN"
            };

            var accountantRole = new IdentityRole
            {
                Id = "2",
                Name = "Accountant",
                NormalizedName = "ACCOUNTANT"
            };

            var viewerRole = new IdentityRole
            {
                Id = "3",
                Name = "Viewer",
                NormalizedName = "VIEWER"
            };

            builder.Entity<IdentityRole>().HasData(adminRole, accountantRole, viewerRole);
        }
    }
}
