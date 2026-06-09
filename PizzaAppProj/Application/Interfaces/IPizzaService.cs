using PizzaAppProj.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Interfaces
{
    public interface IPizzaService
    {
        Task<IReadOnlyCollection<PizzaMenuItemDto>> GetMenuAsync(CancellationToken cancellationToken = default);
        Task<PizzaMenuItemDto> AddPizzaAsync(CreatePizzaRequest request, CancellationToken cancellationToken = default);
    }
}
