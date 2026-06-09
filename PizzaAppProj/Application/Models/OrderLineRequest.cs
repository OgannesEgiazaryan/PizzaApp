using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaAppProj.Application.Models
{
    public sealed record OrderLineRequest(int PizzaId, int Quantity);
}
