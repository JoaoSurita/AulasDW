using Microsoft.AspNetCore.Identity;
using VasosInteligentes.Models;


namespace VasosInteligentes.Seeds
{
    public class IdentitySeeds
    {
        public static async Task SeedRolesAndUser(
            IServiceProvider serviceProvider, 
            string defaultPassword)
        {
            //criação das roles (Administrador e Usuario)
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            string[] roleNames = { "Administrador", "Usuario" };
            foreach(var roleName in roleNames)
            {
                //verificar se já foi criado
                if(await roleManager.FindByNameAsync(roleName) == null)
                {
                    //se não encontrou será inserido
                    var result = await roleManager.CreateAsync(
                        new ApplicationRole { Name = roleName }
                    );
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"SEED: Role {roleName} foi criada");
                    }
                    else { return; }
                }
            }//fim do foreach
            //criar os usuários 
            //criar o administrador
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (await userManager.FindByEmailAsync("admin@admin.com") == null)
            {
                //se não encontrou será inserido
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true
                };
                var resultAdmin = await userManager.CreateAsync(adminUser, defaultPassword);
                if (resultAdmin.Succeeded)
                {
                    Console.WriteLine($"SEED: Administrador foi criado");
                    //atribuindo uma role para o usuário
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
                else { return; }
            }//fim do if
            //criar um usuário comum
            if (await userManager.FindByEmailAsync("vania@fgp.com.br") == null)
            {
                //se não encontrou será inserido
                var user = new ApplicationUser
                {
                    UserName = "vania@fgp.com.br",
                    Email = "vania@fgp.com.br",
                    EmailConfirmed = true
                };
                var resultUser = await userManager.CreateAsync(user, "Vania@123");
                if (resultUser.Succeeded)
                {
                    Console.WriteLine($"SEED: Usuário Comum foi criado");
                    //atribuindo uma role para o usuário
                    await userManager.AddToRoleAsync(user, "Usuario");
                }
                else { return; }
            }

        }
    }
}
