using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.API.Middlewares;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Data.Seeds;
using FCG.Infra.Security.Models;
using FCG.Infra.Security.Seeds;
using Microsoft.AspNetCore.Identity;

namespace FCG.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityCustomUser>>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            await IdentitySeed.SeedData(userManager, roleManager);
            await FCGSeed.SeedData(unitOfWork);
        }

        public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }
    }
}