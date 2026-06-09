using PizzaAppProj.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderSummaryDto> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<OrderSummaryDto>> GetBoardAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<OrderSummaryDto>> GetHistoryAsync(CancellationToken cancellationToken = default);
        Task<OrderSummaryDto?> MarkAsIssuedAsync(long orderNumber, CancellationToken cancellationToken = default);
        Task<int> UpdateStatusesAsync(CancellationToken cancellationToken = default);
    }
}
