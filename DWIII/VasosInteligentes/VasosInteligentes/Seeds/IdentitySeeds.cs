using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using VasosInteligentes.Models;

namespace VasosInteligentes.Seeds
{
    public class IdentitySeeds
    {
        public static async Task SeedRolesAndUser(
            IServiceProvider serviceProvider, string defaultPassword)
        {
            // Criação das roles (Administrador e Usuario)
            var roleMenager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            string[] rolesNames = { "Administrador", "Usuario" };
            foreach (var roleName in rolesNames)
            {
                // verificar se já foi criado
                if(await roleMenager.FindByNameAsync(roleName) == null)
                {
                    // se não encontrou, será inserido
                    var result = await roleMenager.CreateAsync(
                        new ApplicationRole { Name = roleName }
                    );
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"SEED: Role {roleName} foi criada");
                    }
                    else { return; }
                }
            } // Fim do foreach
            // Criar usuarios 
            //Criar Admin
            var userManeger = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (await userManeger.FindByEmailAsync("admin@admin.com") == null)
            {
                // se não encontrou, será inserido
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true
                };
                var ResultAdmin = await userManeger.CreateAsync(adminUser, defaultPassword);
                    
                if (ResultAdmin.Succeeded)
                {
                    Console.WriteLine($"SEED: Administrador foi criado");
                    //Atribuindo a uma role para o usuario 
                    await userManeger.AddToRoleAsync(adminUser, "Administrador");
                }
                else { return; }

            } // usuario comum 
            if (await userManeger.FindByEmailAsync("teste@usuario.com") == null)
            {
                Console.WriteLine("Estou aqui");
                // se não encontrou, será inserido
                var user = new ApplicationUser
                {
                    UserName = "teste@usuario.com",
                    Email = "teste@usuario.com",
                    EmailConfirmed = true
                };
                var resultUser = await userManeger.CreateAsync(user, "Teste@123");

                if (resultUser.Succeeded)
                {
                    Console.WriteLine($"SEED: Administrador foi criado");
                    await userManeger.AddToRoleAsync(user, "Usuario");
                }
                else { return; }
            }

        }
    }
}
