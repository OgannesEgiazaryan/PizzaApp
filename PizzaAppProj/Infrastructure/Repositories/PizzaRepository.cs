using PizzaAppProj.Application.Interfaces;
using PizzaAppProj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaAppProj.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PizzaAppProj.Infrastructure.Repositories
{
    public sealed class PizzaRepository(PizzaAppDbContext dbContext) : IPizzaRepository
    {
        public Task<List<Pizza>> GetAllAsync(CancellationToken cancellationToken = default) =>
            dbContext.Pizzas
                .AsNoTracking()
                .OrderBy(pizza => pizza.Id)
                .ToListAsync(cancellationToken);

        public Task<List<Pizza>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default) =>
            dbContext.Pizzas
                .Where(pizza => ids.Contains(pizza.Id))
                .ToListAsync(cancellationToken);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
            dbContext.Pizzas.AnyAsync(pizza => pizza.Name.ToLower() == name.ToLower(), cancellationToken);

        public Task AddAsync(Pizza pizza, CancellationToken cancellationToken = default) =>
            dbContext.Pizzas.AddAsync(pizza, cancellationToken).AsTask();

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            dbContext.SaveChangesAsync(cancellationToken);
    }
}
