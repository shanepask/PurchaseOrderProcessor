using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Domain.Mediation
{
    public class Mediator : IMediator
    {
        public class NoHandlersException : Exception { public NoHandlersException() : base("No handlers where found to handle the purchase order.") { } }

        private readonly IEnumerable<ILineItemHandler> _lineItemHandlers;
        private readonly IEnumerable<IResultHandler> _resultHandlers;

        public Mediator(IEnumerable<IHandler> handlers)
        {
            handlers = handlers.ToArray();

            if (!handlers.Any())
                throw new NoHandlersException();

            _lineItemHandlers = handlers.OfType<ILineItemHandler>().ToArray();
            _resultHandlers = handlers.OfType<IResultHandler>().ToArray();
        }

        public async Task<ShippingSlip> ProcessAsync(int customerId, PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
        {
            var context = new Context();
            foreach (var lineItem in purchaseOrder.Items)
            {
                foreach (var itemHandler in _lineItemHandlers)
                    await itemHandler.HandleAsync(customerId, lineItem, context, cancellationToken);
            }

            foreach (var resultHandler in _resultHandlers)
                await resultHandler.HandleAsync(customerId, context, cancellationToken);

            return context.ShippingSlip;
        }
    }
}