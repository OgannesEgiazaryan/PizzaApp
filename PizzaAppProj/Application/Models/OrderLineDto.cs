using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Models
{
    public sealed record OrderLineDto(
        string PizzaName,
        int Quantity,
        decimal UnitPrice,
        decimal LineTotal);
}
