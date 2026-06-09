using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Models
{
    public sealed record CreatePizzaRequest(
    string Name,
    string Description,
    string Ingredients,
    decimal Price,
    int WeightGrams,
    int CaloriesPer100Grams);
}
