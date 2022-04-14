using System.Collections.Generic;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Domain.Mediator
{
    public interface IContext
    {
        public List<string> PhysicalProducts { get; }
        public ShippingSlip ShippingSlip { get; set; }
    }
}
