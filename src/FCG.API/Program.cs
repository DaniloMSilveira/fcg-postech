using System.Reflection;
using FCG.API.Extensions;
using FCG.API.Middlewares;
using FCG.Application.Security;
using FCG.Application.Services;
using FCG.Domain.Interfaces.Repositories;
using FCG.Domain.Interfaces.Services;
using FCG.Domain.Services;
using FCG.Infra.Data.Contexts;
using FCG.Infra.Data.Repositories;
using FCG.Infra.Data.Seeds;
using FCG.Infra.Security.Contexts;
using FCG.Infra.Security.Models;
using FCG.Infra.Security.Seeds;
using FCG.Infra.Security.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("FCG")));
builder.Services.AddIdentityAuthentication(builder.Configuration);
builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddDbContext<FCGDataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("FCG")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IAutenticacaoAppService, AutenticacaoAppService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioAppService, UsuarioAppService>();

builder.Services.AddScoped<IJogoRepository, JogoRepository>();
builder.Services.AddScoped<IJogoService, JogoService>();
builder.Services.AddScoped<IJogoAppService, JogoAppService>();

builder.Services.AddScoped<IPromocaoRepository, PromocaoRepository>();
builder.Services.AddScoped<IPromocaoService, PromocaoService>();
builder.Services.AddScoped<IPromocaoAppService, PromocaoAppService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Executando seed de dados identity (roles e usuário admin)
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityCustomUser>>();
    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    await IdentitySeed.SeedData(userManager, roleManager);
    await FCGSeed.SeedData(unitOfWork);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
