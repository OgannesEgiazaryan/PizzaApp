using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PizzaAppProj.Domain.Enums;

namespace PizzaAppProj.Domain.Entities
{
    public sealed class Order
    {
        public int Id { get; set; }
        public long OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public DateTimeOffset OrderedAt { get; set; }
        public DateTimeOffset ReadyAt { get; set; }
        public DateTimeOffset? IssuedAt { get; set; }
        public decimal TotalCost { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
