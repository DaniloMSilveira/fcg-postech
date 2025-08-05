using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FCG.Application.Security;
using FCG.Application.Services;
using FCG.Domain.Interfaces.Repositories;
using FCG.Domain.Interfaces.Services;
using FCG.Domain.Services;
using FCG.Infra.Data.Contexts;
using FCG.Infra.Data.Repositories;
using FCG.Infra.Security.Contexts;
using FCG.Infra.Security.Services;
using Microsoft.EntityFrameworkCore;

namespace FCG.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDataContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("FCG")));

            services.AddDbContext<FCGDataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("FCG")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();

            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUsuarioAppService, UsuarioAppService>();
            services.AddScoped<IAutenticacaoAppService, AutenticacaoAppService>();

            services.AddScoped<IJogoRepository, JogoRepository>();
            services.AddScoped<IJogoService, JogoService>();
            services.AddScoped<IJogoAppService, JogoAppService>();

            services.AddScoped<IPromocaoRepository, PromocaoRepository>();
            services.AddScoped<IPromocaoService, PromocaoService>();
            services.AddScoped<IPromocaoAppService, PromocaoAppService>();

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}