using System;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Domain.Mediator
{
    public class Mediator : IMediator
    {
        public Mediator(IServiceProvider serviceProvider)
        {
            
        }

        public ShippingSlip ProcessAsync(int customerId, PurchaseOrder purchaseOrder)
        {
            throw new NotImplementedException();
        }
    }
}