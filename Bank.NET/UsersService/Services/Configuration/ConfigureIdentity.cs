using Microsoft.AspNetCore.Identity;
using UsersService.Models;

namespace UsersService.Services.Configurations
{
	public static class ConfigureIdentity
	{
		public static async Task ConfigureIdentityAsync(this WebApplication app)
		{
            using var serviceScope = app.Services.CreateScope();
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<Role>>();
            var config = app.Configuration.GetSection("SuperAdminConfig");

            if (config == null)
            {
                return;
            }

            var employeeRole = await roleManager.FindByNameAsync("Employee");
            if (employeeRole == null)
            {
                var result = await roleManager.CreateAsync(new Role()
                {
                    Name = "Employee"
                });

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create Employee role.");
                }

                employeeRole = await roleManager.FindByNameAsync("Employee");
            }

            var clientRole = await roleManager.FindByNameAsync("Client");
            if (clientRole == null)
            {
                var result = await roleManager.CreateAsync(new Role()
                {
                    Name = "Client"
                });

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create Client role.");
                }

                clientRole = await roleManager.FindByNameAsync("Client");
            }

            var superAdmin = await userManager.FindByNameAsync(config["AdminUserName"]);
            if (superAdmin == null)
            {
                var userResult = await userManager.CreateAsync(new User
                {
                    UserName = config["AdminUserName"],
                    Name = config["AdminUserName"],
                    Surname = config["AdminUserName"],
                    Patronymic = config["AdminUserName"],
                    CreateAt = DateTime.UtcNow,
                }, config["AdminPassword"]);

                if (!userResult.Succeeded)
                {
                    throw new InvalidOperationException($"Unable to create {config["AdminUserName"]} user");
                }

                superAdmin = await userManager.FindByNameAsync(config["AdminUserName"]);
            }

            if (!await userManager.IsInRoleAsync(superAdmin, employeeRole.Name))
            {
                await userManager.AddToRoleAsync(superAdmin, employeeRole.Name);
            }
        }
    }
}
