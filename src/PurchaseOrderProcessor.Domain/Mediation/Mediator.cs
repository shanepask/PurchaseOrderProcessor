using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Domain.Mediation
{
    public class Mediator : IMediator
    {
        public class NoHandlersException : Exception { public NoHandlersException() : base("No handlers where found to handle the purchase order.") { } }

        public Mediator(IServiceProvider serviceProvider)
        {
            var s = serviceProvider.GetServices<IHandler>();
        }

        public Task<ShippingSlip> ProcessAsync(int customerId, PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}