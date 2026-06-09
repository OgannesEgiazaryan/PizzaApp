using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Infrastructure.Data
{
    public sealed class PizzaAppDbContextFactory : IDesignTimeDbContextFactory<PizzaAppDbContext>
    {
        public PizzaAppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("Postgres");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "Host=localhost;Port=5432;Database=pizza_app;Username=postgres;Password=postgres";
            }

            var optionsBuilder = new DbContextOptionsBuilder<PizzaAppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new PizzaAppDbContext(optionsBuilder.Options);
        }
    }
}
