using PizzaAppProj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Interfaces
{
    public interface IPizzaRepository
    {
        Task<List<Pizza>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Pizza>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
        Task AddAsync(Pizza pizza, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
