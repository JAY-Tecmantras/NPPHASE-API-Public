using Microsoft.AspNetCore.Identity;
using NPPHASE.Data.Model;

namespace NPPHASE.Data.Seeder
{
    public class IdentityDataSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityDataSeeder(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            //var adminRole = new IdentityRole
            //{
            //    Id = "297dd7e4-b5b0-48a5-9dc4-3b11ab9c0724",
            //    Name = "Admin",
            //    NormalizedName = "ADMIN"
            //};
            //await CreateRoleAsync(adminRole);

            //var superAdminRole = new IdentityRole
            //{
            //    Id = "00C6ED80-9F86-493A-BCF6-91C47665C937",
            //    Name = "SuperAdmin",
            //    NormalizedName = "SUPERADMIN"
            //};
            //await CreateRoleAsync(superAdminRole);

            //var userRole = new IdentityRole
            //{
            //    Id = "68C5C2EB-257E-4F8B-A2D2-C1E7A36E95AD",
            //    Name = "User",
            //    NormalizedName = "USER"
            //};
            //await CreateRoleAsync(userRole);

            //var adminUserPassword = "Test@123";
            //var adminUser = new User
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    UserName = "admin@gmail.com",
            //    NormalizedUserName = "ADMIN@GMAIL.COM",
            //    Email = "admin@gmail.com",
            //    NormalizedEmail = "ADMIN@GMAIL.COM",
            //    EmailConfirmed = true,
            //    Name = "Admin",
            //    CreationDate = DateTimeOffset.UtcNow
            //};
            //await CreateUserAsync(adminUser, adminUserPassword);

            //var adminInRole = await _userManager.IsInRoleAsync(adminUser, adminRole.Name);
            //if (!adminInRole)
            //    await _userManager.AddToRoleAsync(adminUser, adminRole.Name);

            //var superAdminUserPassword = "Test@123";
            //var superAdminUser = new User
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    UserName = "superadmin@gmail.com",
            //    NormalizedUserName = "SUPERADMIN@GMAIL.COM",
            //    Email = "superadmin@gmail.com",
            //    NormalizedEmail = "SUPERADMIN@GMAIL.COM",
            //    EmailConfirmed = true,
            //    Name = "Super Admin",
            //    CreationDate = DateTimeOffset.UtcNow
            //};
            //await CreateUserAsync(superAdminUser, superAdminUserPassword);

            //var superAdminInRole = await _userManager.IsInRoleAsync(superAdminUser, superAdminRole.Name);
            //if (!superAdminInRole)
            //    await _userManager.AddToRoleAsync(superAdminUser, superAdminRole.Name);
        }

        private async Task CreateRoleAsync(IdentityRole role)
        {
            var exits = await _roleManager.RoleExistsAsync(role.Name);
            if (!exits)
                await _roleManager.CreateAsync(role);
        }

        private async Task CreateUserAsync(User user, string password)
        {
            var exists = await _userManager.FindByEmailAsync(user.Email);
            if (exists == null)
                await _userManager.CreateAsync(user, password);
        }
    }
}
