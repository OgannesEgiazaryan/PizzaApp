using PizzaAppProj.Application.Interfaces;
using PizzaAppProj.Domain.Entities;
using PizzaAppProj.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaAppProj.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PizzaAppProj.Infrastructure.Repositories
{
    public sealed class OrderRepository(PizzaAppDbContext dbContext) : IOrderRepository
    {
        public async Task<long> GetNextOrderNumberAsync(CancellationToken cancellationToken = default)
        {
            var currentMax = await dbContext.Orders
                .MaxAsync(order => (long?)order.OrderNumber, cancellationToken) ?? 999;

            return currentMax + 1;
        }

        public Task<List<Order>> GetBoardOrdersAsync(CancellationToken cancellationToken = default) =>
            dbContext.Orders
                .AsNoTracking()
                .Include(order => order.Items)
                .ThenInclude(item => item.Pizza)
                .Where(order => order.Status != OrderStatus.Issued)
                .ToListAsync(cancellationToken);

        public Task<List<Order>> GetHistoryOrdersAsync(CancellationToken cancellationToken = default) =>
            dbContext.Orders
                .AsNoTracking()
                .Include(order => order.Items)
                .ThenInclude(item => item.Pizza)
                .Where(order => order.Status == OrderStatus.Issued)
                .ToListAsync(cancellationToken);

        public Task<List<Order>> GetOrdersReadyToSwitchAsync(DateTimeOffset currentTime, CancellationToken cancellationToken = default) =>
            dbContext.Orders
                .Include(order => order.Items)
                .ThenInclude(item => item.Pizza)
                .Where(order => order.Status == OrderStatus.Cooking && order.ReadyAt <= currentTime)
                .ToListAsync(cancellationToken);

        public Task<Order?> GetByNumberAsync(long orderNumber, CancellationToken cancellationToken = default) =>
            dbContext.Orders
                .Include(order => order.Items)
                .ThenInclude(item => item.Pizza)
                .FirstOrDefaultAsync(order => order.OrderNumber == orderNumber, cancellationToken);

        public Task AddAsync(Order order, CancellationToken cancellationToken = default) =>
            dbContext.Orders.AddAsync(order, cancellationToken).AsTask();

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            dbContext.SaveChangesAsync(cancellationToken);
    }
}
