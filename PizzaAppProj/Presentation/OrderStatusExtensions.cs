using PizzaAppProj.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Presentation
{
    public static class OrderStatusExtensions
    {
        public static string ToDisplayName(this OrderStatus status) => status switch
        {
            OrderStatus.Cooking => "Готовится",
            OrderStatus.ReadyForPickup => "Готов к выдаче",
            OrderStatus.Issued => "Выдан",
            _ => status.ToString()
        };

        public static string ToMarkupColor(this OrderStatus status) => status switch
        {
            OrderStatus.Cooking => "yellow",
            OrderStatus.ReadyForPickup => "green",
            OrderStatus.Issued => "grey",
            _ => "white"
        };
    }
}
