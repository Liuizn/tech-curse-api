using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace tech_curse_api.src.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {        
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Roles
        string[] roles = { "Admin", "Instructor", "Student" };

        foreach (var roleName in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Usuário Administrador padrão
        var adminEmail = "admin@seudominio.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true 
            };

            string adminPassword = "Admin@123456";

            var createAdminResult = await userManager.CreateAsync(newAdmin, adminPassword);

            if (createAdminResult.Succeeded)
            {                
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }

        // Instrutor Padrão
        var instrutorEmail = "instrutor@gmail.com";
        var instrutorUser = await userManager.FindByEmailAsync(instrutorEmail);

        if (instrutorUser == null)
        {
            var newInstrutor = new IdentityUser
            {
                UserName = instrutorEmail,
                Email = instrutorEmail,
                EmailConfirmed = true
            };

            string instrutorPassword = "Instrutor@123456";

            var createInstrutorResult = await userManager.CreateAsync(newInstrutor, instrutorPassword);

            if (createInstrutorResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newInstrutor, "Instructor");
            }
        }
    }
}
