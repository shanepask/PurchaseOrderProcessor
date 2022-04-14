using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Mediator;

namespace PurchaseOrderProcessor.Domain.Handlers;

public class MembershipHandler : ILineItemHandler
{
    public Task HandleAsync(int customerId, string lineItem, IContext context)
    {
        throw new System.NotImplementedException();
    }
}