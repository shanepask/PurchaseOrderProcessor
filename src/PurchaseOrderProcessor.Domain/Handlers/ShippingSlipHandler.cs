using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Mediation;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Domain.Handlers
{
    public class ShippingSlipHandler : IResultHandler
    {
        public Task HandleAsync(int customerId, IContext context, CancellationToken cancellationToken = default)
		{
			if (context.PhysicalProducts.Any())
            {
                var shippingSlipItems = context.PhysicalProducts
                    .GroupBy(p => p)
                    .Select(p => new ShippingSlipItem
                    {
                        Description = p.Key,
                        Quantity = p.Count()
                    });
                context.ShippingSlip = new()
                {
                    CustomerId = customerId,
                    Items = shippingSlipItems
                };
            }

            return Task.CompletedTask;
		}
    }
}
