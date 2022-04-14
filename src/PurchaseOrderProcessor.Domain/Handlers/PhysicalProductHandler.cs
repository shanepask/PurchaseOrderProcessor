using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Mediator;

namespace PurchaseOrderProcessor.Domain.Handlers;

public class PhysicalProductHandler : ILineItemHandler
{
    public Task HandleAsync(int customerId, string lineItem, IContext context)
    {
        throw new System.NotImplementedException();
    }
}