using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCG.API.Middlewares;
using FCG.Domain.Interfaces.Repositories;
using FCG.Infra.Data.Contexts;
using FCG.Infra.Data.Seeds;
using FCG.Infra.Security.Contexts;
using FCG.Infra.Security.Models;
using FCG.Infra.Security.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FCG.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var identityDataContext = scope.ServiceProvider.GetRequiredService<IdentityDataContext>();
            var fcgDataContext = scope.ServiceProvider.GetRequiredService<FCGDataContext>();

            await identityDataContext.Database.MigrateAsync();
            await fcgDataContext.Database.MigrateAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityCustomUser>>();
            await IdentitySeed.SeedRoles(roleManager);
            
            var adminEmail = config["IdentitySeedAdmin:Email"];
            var adminPassword = config["IdentitySeedAdmin:Password"];

            if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
                await IdentitySeed.SeedAdminUser(userManager, adminEmail, adminPassword);
            
            if (app.Environment.IsDevelopment())
                {
                    await IdentitySeed.SeedTestingData(userManager);
                    await FCGSeed.SeedTestingData(unitOfWork);
                }
        }

        public static WebApplication UseCustomMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            return app;
        }

        public static WebApplication UseCustomSwagger(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var swaggerEnabled = config["Swagger:Enabled"];
            if (!string.IsNullOrEmpty(swaggerEnabled) && swaggerEnabled.ToLower() == "true")
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }

        public static WebApplication UseCustomMetrics(this WebApplication app)
        {
            app.MapPrometheusScrapingEndpoint();

            return app;
        }
    }
}