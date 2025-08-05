using FCG.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDataContexts(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddIdentityAuthentication(builder.Configuration);
builder.Services.AddCustomSwagger();

var app = builder.Build();

await app.SeedDatabaseAsync();

app.UseCustomSwagger(app.Environment);

app.UseAuthentication();
app.UseAuthorization();

app.UseCustomMiddlewares();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
