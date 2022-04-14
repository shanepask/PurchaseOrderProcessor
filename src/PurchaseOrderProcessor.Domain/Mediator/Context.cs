using System.Collections.Generic;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Domain.Mediator
{
    internal class Context : IContext
    {
        public List<string> PhysicalProducts { get; } = new();
        public ShippingSlip ShippingSlip { get; set; }
    }
}
