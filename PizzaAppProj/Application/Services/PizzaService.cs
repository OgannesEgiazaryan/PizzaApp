using PizzaAppProj.Application.Interfaces;
using PizzaAppProj.Application.Models;
using PizzaAppProj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Services
{
    public sealed class PizzaService(IPizzaRepository pizzaRepository) : IPizzaService
    {
        public async Task<IReadOnlyCollection<PizzaMenuItemDto>> GetMenuAsync(CancellationToken cancellationToken = default)
        {
            var pizzas = await pizzaRepository.GetAllAsync(cancellationToken);

            return pizzas
                .OrderBy(pizza => pizza.Name)
                .Select(MapPizza)
                .ToArray();
        }

        public async Task<PizzaMenuItemDto> AddPizzaAsync(CreatePizzaRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new InvalidOperationException("Название пиццы не может быть пустым.");
            }

            if (request.Price <= 0 || request.WeightGrams <= 0 || request.CaloriesPer100Grams <= 0)
            {
                throw new InvalidOperationException("Цена, вес и калорийность должны быть больше нуля.");
            }

            if (await pizzaRepository.ExistsByNameAsync(request.Name.Trim(), cancellationToken))
            {
                throw new InvalidOperationException("Пицца с таким названием уже существует.");
            }

            var pizza = new Pizza
            {
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Ingredients = request.Ingredients.Trim(),
                Price = request.Price,
                WeightGrams = request.WeightGrams,
                CaloriesPer100Grams = request.CaloriesPer100Grams
            };

            await pizzaRepository.AddAsync(pizza, cancellationToken);
            await pizzaRepository.SaveChangesAsync(cancellationToken);

            return MapPizza(pizza);
        }

        private static PizzaMenuItemDto MapPizza(Pizza pizza) => new(
            pizza.Id,
            pizza.Name,
            pizza.Description,
            pizza.Ingredients,
            pizza.Price,
            pizza.WeightGrams,
            pizza.CaloriesPer100Grams);
    }
}
