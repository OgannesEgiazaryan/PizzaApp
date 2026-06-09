using PizzaAppProj.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<long> GetNextOrderNumberAsync(CancellationToken cancellationToken = default);
        Task<List<Order>> GetBoardOrdersAsync(CancellationToken cancellationToken = default);
        Task<List<Order>> GetHistoryOrdersAsync(CancellationToken cancellationToken = default);
        Task<List<Order>> GetOrdersReadyToSwitchAsync(DateTimeOffset currentTime, CancellationToken cancellationToken = default);
        Task<Order?> GetByNumberAsync(long orderNumber, CancellationToken cancellationToken = default);
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
