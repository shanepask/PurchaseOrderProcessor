using System.Threading;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Mediation;

namespace PurchaseOrderProcessor.Domain.Handlers;

public class PhysicalProductHandler : ILineItemHandler
{
    public Task HandleAsync(int customerId, string lineItem, IContext context, CancellationToken cancellationToken = default)
    {
        if (LineItemHelper.IsPhysicalProduct(lineItem))
            context.PhysicalProducts.Add(lineItem);

        return Task.CompletedTask;
    }
}