using Microsoft.AspNetCore.Identity;
using VasosInteligentes.Models;

namespace VasosInteligentes.Seeds
{
    public class IdentitySeeds
    {
        public static async Task SeedRolesAndUser(IServiceProvider serviceProvider, string defaultPassowrd)
        {
            // Criação das nossas Roles (Administrador e Usuario)
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            string[] roleNames = { "Administrador", "Usuario" };
            foreach (var roleName in roleNames)
            {
                // Verifica se a Role já existe, caso contrário, cria a Role
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"SEED: Role '{roleName}' criada com sucesso.");
                    }
                    else { return; }
                }
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            // Criação do usuário Admin
            if (await userManager.FindByEmailAsync("admin@admin.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true
                };
                var resultAdmin = await userManager.CreateAsync(adminUser, defaultPassowrd);
                if (resultAdmin.Succeeded)
                {
                    Console.WriteLine($"SEED: Administrador foi criado com sucesso.");
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
                else { return; }
            }

            // Criação do usuário Usuário
            if (await userManager.FindByEmailAsync("teste@usuario.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "teste@usuario.com",
                    Email = "teste@usuario.com",
                    EmailConfirmed = true
                };
                var resultUser = await userManager.CreateAsync(user, "teste@123");
                if (resultUser.Succeeded)
                {
                    Console.WriteLine($"SEED: Usuário foi criado com sucesso.");
                    await userManager.AddToRoleAsync(user, "Usuario");
                }
                else { return; }
            }
        }
    }
}