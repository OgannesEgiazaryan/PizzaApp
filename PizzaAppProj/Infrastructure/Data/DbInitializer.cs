using Microsoft.EntityFrameworkCore;
using PizzaAppProj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Infrastructure.Data
{
    public sealed class DbInitializer(PizzaAppDbContext dbContext)
    {
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (dbContext.Database.IsNpgsql())
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            else
            {
                await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            }

            if (await dbContext.Pizzas.AnyAsync(cancellationToken))
            {
                return;
            }

            var pizzas = new[]
            {
            new Pizza
            {
                Name = "Маргарита",
                Description = "Классика с томатным соусом и моцареллой.",
                Ingredients = "Томатный соус, моцарелла, базилик",
                Price = 420m,
                WeightGrams = 480,
                CaloriesPer100Grams = 220
            },
            new Pizza
            {
                Name = "Пепперони",
                Description = "Острая салями и тягучий сыр.",
                Ingredients = "Томатный соус, моцарелла, пепперони",
                Price = 560m,
                WeightGrams = 530,
                CaloriesPer100Grams = 285
            },
            new Pizza
            {
                Name = "Четыре сыра",
                Description = "Насыщенный сливочный вкус из 4 сыров.",
                Ingredients = "Моцарелла, дорблю, пармезан, чеддер, сливочный соус",
                Price = 610m,
                WeightGrams = 510,
                CaloriesPer100Grams = 310
            },
            new Pizza
            {
                Name = "Барбекю",
                Description = "Курица, бекон и фирменный соус барбекю.",
                Ingredients = "Соус барбекю, курица, бекон, красный лук, моцарелла",
                Price = 640m,
                WeightGrams = 560,
                CaloriesPer100Grams = 295
            },
            new Pizza
            {
                Name = "Овощная",
                Description = "Легкий вариант с яркими овощами.",
                Ingredients = "Томатный соус, моцарелла, томаты, перец, шампиньоны, маслины",
                Price = 470m,
                WeightGrams = 500,
                CaloriesPer100Grams = 205
            }
        };

            await dbContext.Pizzas.AddRangeAsync(pizzas, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
