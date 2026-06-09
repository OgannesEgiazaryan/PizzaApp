using PizzaAppProj.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Models
{
    public sealed record OrderSummaryDto(
        long OrderNumber,
        string CustomerName,
        OrderStatus Status,
        DateTimeOffset OrderedAt,
        DateTimeOffset ReadyAt,
        DateTimeOffset? IssuedAt,
        decimal TotalCost,
        IReadOnlyCollection<OrderLineDto> Items);
}
