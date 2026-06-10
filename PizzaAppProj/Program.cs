using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PizzaAppProj.Application.Interfaces;
using PizzaAppProj.Application.Services;
using PizzaAppProj.Infrastructure.Data;
using PizzaAppProj.Infrastructure.Repositories;
using PizzaAppProj.Infrastructure.Services;
using PizzaAppProj.Presentation;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var culture = CultureInfo.GetCultureInfo("ru-RU");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = Host.CreateApplicationBuilder(args);
var databasePath = Path.Combine(AppContext.BaseDirectory, "pizza-app.db");
var databaseProvider = builder.Configuration["Database:Provider"];
var postgresConnectionString = builder.Configuration.GetConnectionString("Postgres");

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddDbContext<PizzaAppDbContext>(options =>
{
    if (string.Equals(databaseProvider, "Postgres", StringComparison.OrdinalIgnoreCase)
        && !string.IsNullOrWhiteSpace(postgresConnectionString))
    {
        options.UseNpgsql(postgresConnectionString);
        return;
    }

    options.UseSqlite($"Data Source={databasePath}");
});

builder.Services.AddScoped<IPizzaRepository, PizzaRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPizzaService, PizzaService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<ConsoleApplication>();
builder.Services.AddHostedService<OrderStatusBackgroundService>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
await initializer.InitializeAsync();

var application = scope.ServiceProvider.GetRequiredService<ConsoleApplication>();
await application.RunAsync();
