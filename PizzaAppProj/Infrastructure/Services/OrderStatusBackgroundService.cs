using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PizzaAppProj.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Infrastructure.Services
{
    public sealed class OrderStatusBackgroundService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateOrdersAsync(stoppingToken);

                try
                {
                    await timer.WaitForNextTickAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task UpdateOrdersAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
            await orderService.UpdateStatusesAsync(cancellationToken);
        }
    }
}
