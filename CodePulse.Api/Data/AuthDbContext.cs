using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.Api.Data
{
    public class AuthDbContext:IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //const string readerRoleId = "eff77fb3-78b2-4445-be07-a7953abbdcf0";
            //const string writerRoleId = "4e2531f7-5a7f-4f34-be6e-750eccc31521";

            // create reader and writer role
            var roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Id = "eff77fb3-78b2-4445-be07-a7953abbdcf0",
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper(),
                    ConcurrencyStamp = "eff77fb3-78b2-4445-be07-a7953abbdcf0"
                },
                new IdentityRole()
                {
                    Id = "4e2531f7-5a7f-4f34-be6e-750eccc31521",
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper(),
                    ConcurrencyStamp = "4e2531f7-5a7f-4f34-be6e-750eccc31521"
                }
            };

            // seed the roles
            builder.Entity<IdentityRole>().HasData(roles);

            // create an admin user or default user
            //const string adminUserId = "25599244-36e5-4bb2-842e-fc61b3eb69bf";
            var admin = new IdentityUser()
            {
                Id = "25599244-36e5-4bb2-842e-fc61b3eb69bf",
                ConcurrencyStamp = "25599244-36e5-4bb2-842e-fc61b3eb69bf",
                UserName = "admin@sandeeppachal.com",
                Email = "admin@sandeeppachal.com",
                NormalizedEmail = "admin@sandeeppachal.com".ToUpper(),
                NormalizedUserName = "admin@sandeeppachal.com".ToUpper(),
                SecurityStamp = "e12ee174-16e4-4281-85d2-196e3a323183"
            };

            admin.PasswordHash = "AQAAAAIAAYagAAAAEKwzZD0bFAXyLWEmQTTP/DrW0Hx2YE20gRxqZs2SxFRq/3664sKl5/kTCz2AUm994Q==";
            //new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@123");

            // seed the admin
            builder.Entity<IdentityUser>().HasData(admin);

            // Give Roles to Admin

            var adminRoles = new List<IdentityUserRole<string>>()
            {
                new()
                {
                    UserId = "25599244-36e5-4bb2-842e-fc61b3eb69bf",
                    RoleId = "eff77fb3-78b2-4445-be07-a7953abbdcf0"
                },
                new()
                {
                    UserId = "25599244-36e5-4bb2-842e-fc61b3eb69bf",
                    RoleId = "4e2531f7-5a7f-4f34-be6e-750eccc31521"
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
        }
    }
}
