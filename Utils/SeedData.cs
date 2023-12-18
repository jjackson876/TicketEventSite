using Microsoft.AspNetCore.Identity;

namespace EventsAPI.Utils
{
    public class SeedData
    {
        public static async Task Intialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            await SeedRoles(roleManager);
            await SeedAdminUser(userManager);
            await SeedNormalUser(userManager);
        }

        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "admin", "user" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
                    //Console.WriteLine($"Role {role} created.");
            }
        }

        public static async Task SeedAdminUser(UserManager<IdentityUser> userManager)
        {
            var adminUser = await userManager.FindByNameAsync("admin");

            if (adminUser == null)
            {
                var admin = new IdentityUser()
                {
                    UserName = "admin123",
                    Email = "pulseadmin@gmail.com"
                };

                var createAdminUser = await userManager.CreateAsync(admin, "Pulse@dmin123");

                if (createAdminUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }

        public static async Task SeedNormalUser(UserManager<IdentityUser> userManager)
        {
            var regularUser = await userManager.FindByNameAsync("user");

            if (regularUser == null)
            {
                var user = new IdentityUser()
                {
                    UserName = "user",
                    Email = "regularuser@gmail.com"
                };

                var createRegularUser = await userManager.CreateAsync(user, "RegularUser12!");

                if (createRegularUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "user");
                }
            }
        }
    }
}
