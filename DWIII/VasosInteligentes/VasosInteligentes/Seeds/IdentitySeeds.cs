using Microsoft.AspNetCore.Identity;
using VasosInteligentes.Models;

namespace VasosInteligentes.Seeds
{
    public class IdentitySeeds
    {
        public static async Task SeedRolesAndUser(IServiceProvider serviceProvider, string defaultPassword)
        {
            // Criar as Roles(adm e usuario)
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            string[] roleNames = { "Administrador", "Usuario" };
            foreach (var roleName in roleNames)
            {
                // Verificar se ja existe
                if (await RoleManager.FindByNameAsync(roleName) == null)
                {
                    // Se não encontrou, sera inserido
                    var result = await RoleManager.CreateAsync(
                        new ApplicationRole { Name = roleName }
                    );
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"SEED: Role {roleName} foi criada");
                    }
                    else { return; }
                }
            } // Fim foreach
            // Criar os Usuarios
            // Criar o adm
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (await UserManager.FindByEmailAsync("admin@gmail.com") == null)
            {
                // Se não encontrou, sera inserido
                var admUser = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };
                var resultAdm = await UserManager.CreateAsync(admUser, defaultPassword);
                if (resultAdm.Succeeded)
                {
                    Console.WriteLine($"SEED: Adm foi criado");
                    // Atribuindo o usuario a sua roler. Se ele é adm ou usuario normal
                    await UserManager.AddToRoleAsync(admUser, "Administrador");
                }
                else { return; }
            }

            // Criar o usuario
            if (await UserManager.FindByEmailAsync("user@gmail.com") == null)
            {
                // Se não encontrou, sera inserido
                var user = new ApplicationUser
                {
                    UserName = "user@gmail.com",
                    Email = "user@gmail.com",
                    EmailConfirmed = true
                };
                var resultUser = await UserManager.CreateAsync(user, "Teste@123");
                if (resultUser.Succeeded)
                {
                    Console.WriteLine($"SEED: Usuario foi criado");
                    // Atribuindo o usuario a sua roler. Se ele é adm ou usuario normal
                    await UserManager.AddToRoleAsync(user, "Usuario");
                }
                else { return; }
            }
        } // Fim método
    }
}