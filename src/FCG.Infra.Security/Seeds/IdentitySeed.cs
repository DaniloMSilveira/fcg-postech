using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.Infra.Security.Constants;
using Microsoft.AspNetCore.Identity;

namespace FCG.Infra.Security.Seeds
{
    public static class IdentitySeed
    {
        public static async Task SeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles.ObterListaRoles())
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var email = "admin@fcg.com";
            var senha = "Admin123!";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, senha);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.USUARIO);
                    await userManager.AddToRoleAsync(user, Roles.ADMINISTRADOR);
                }
            }
        }
    }
}