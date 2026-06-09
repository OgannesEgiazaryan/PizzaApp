using PizzaAppProj.Application.Interfaces;
using PizzaAppProj.Application.Models;
using PizzaAppProj.Domain.Entities;
using PizzaAppProj.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Services
{
    public sealed class OrderService(
    IOrderRepository orderRepository,
    IPizzaRepository pizzaRepository,
    TimeProvider timeProvider) : IOrderService
    {
        public async Task<OrderSummaryDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                throw new InvalidOperationException("Имя клиента не может быть пустым.");
            }

            if (request.Items.Count == 0)
            {
                throw new InvalidOperationException("Выберите хотя бы одну пиццу.");
            }

            await UpdateStatusesAsync(cancellationToken);

            var normalizedItems = request.Items
                .GroupBy(item => item.PizzaId)
                .Select(group => new OrderLineRequest(group.Key, group.Sum(item => item.Quantity)))
                .ToArray();

            if (normalizedItems.Any(item => item.Quantity <= 0))
            {
                throw new InvalidOperationException("Количество каждой пиццы должно быть больше нуля.");
            }

            var pizzas = await pizzaRepository.GetByIdsAsync(normalizedItems.Select(item => item.PizzaId).ToArray(), cancellationToken);
            var pizzaMap = pizzas.ToDictionary(pizza => pizza.Id);

            if (pizzaMap.Count != normalizedItems.Length)
            {
                throw new InvalidOperationException("Часть выбранных пицц не найдена в меню.");
            }

            var currentTime = timeProvider.GetUtcNow();
            var order = new Order
            {
                OrderNumber = await orderRepository.GetNextOrderNumberAsync(cancellationToken),
                CustomerName = request.CustomerName.Trim(),
                OrderedAt = currentTime,
                ReadyAt = currentTime.AddMinutes(15),
                Status = OrderStatus.Cooking
            };

            foreach (var item in normalizedItems)
            {
                var pizza = pizzaMap[item.PizzaId];
                order.Items.Add(new OrderItem
                {
                    PizzaId = pizza.Id,
                    Quantity = item.Quantity,
                    UnitPrice = pizza.Price
                });
            }

            order.TotalCost = order.Items.Sum(item => item.UnitPrice * item.Quantity);

            await orderRepository.AddAsync(order, cancellationToken);
            await orderRepository.SaveChangesAsync(cancellationToken);

            return MapOrder(order);
        }

        public async Task<IReadOnlyCollection<OrderSummaryDto>> GetBoardAsync(CancellationToken cancellationToken = default)
        {
            await UpdateStatusesAsync(cancellationToken);
            var orders = await orderRepository.GetBoardOrdersAsync(cancellationToken);

            return orders
                .OrderBy(order => order.Status)
                .ThenBy(order => order.ReadyAt)
                .Select(MapOrder)
                .ToArray();
        }

        public async Task<IReadOnlyCollection<OrderSummaryDto>> GetHistoryAsync(CancellationToken cancellationToken = default)
        {
            await UpdateStatusesAsync(cancellationToken);
            var orders = await orderRepository.GetHistoryOrdersAsync(cancellationToken);

            return orders
                .OrderByDescending(order => order.IssuedAt)
                .ThenByDescending(order => order.OrderedAt)
                .Select(MapOrder)
                .ToArray();
        }

        public async Task<OrderSummaryDto?> MarkAsIssuedAsync(long orderNumber, CancellationToken cancellationToken = default)
        {
            await UpdateStatusesAsync(cancellationToken);

            var order = await orderRepository.GetByNumberAsync(orderNumber, cancellationToken);
            if (order is null)
            {
                return null;
            }

            if (order.Status != OrderStatus.Issued)
            {
                order.Status = OrderStatus.Issued;
                order.IssuedAt = timeProvider.GetUtcNow();
                await orderRepository.SaveChangesAsync(cancellationToken);
            }

            return MapOrder(order);
        }

        public async Task<int> UpdateStatusesAsync(CancellationToken cancellationToken = default)
        {
            var currentTime = timeProvider.GetUtcNow();
            var outdatedOrders = await orderRepository.GetOrdersReadyToSwitchAsync(currentTime, cancellationToken);
            if (outdatedOrders.Count == 0)
            {
                return 0;
            }

            foreach (var order in outdatedOrders)
            {
                order.Status = OrderStatus.ReadyForPickup;
            }

            await orderRepository.SaveChangesAsync(cancellationToken);
            return outdatedOrders.Count;
        }

        private static OrderSummaryDto MapOrder(Order order) => new(
            order.OrderNumber,
            order.CustomerName,
            order.Status,
            order.OrderedAt,
            order.ReadyAt,
            order.IssuedAt,
            order.TotalCost,
            order.Items
                .OrderBy(item => item.Pizza.Name)
                .Select(item => new OrderLineDto(
                    item.Pizza.Name,
                    item.Quantity,
                    item.UnitPrice,
                    item.UnitPrice * item.Quantity))
                .ToArray());
    }
}
