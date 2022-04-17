using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Clients;
using PurchaseOrderProcessor.Domain.Mediation;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Domain.Handlers;

public class MembershipHandler : ILineItemHandler
{
    private readonly ICustomerClient _client;

    public MembershipHandler(ICustomerClient client) => _client = client;

    public Task HandleAsync(int customerId, string lineItem, IContext context, CancellationToken cancellationToken = default)
    {
        if (LineItemHelper.IsMembershipProduct(lineItem))
            _client.MembershipUpdateAsync(customerId, lineItem, cancellationToken);
        return Task.CompletedTask;
    }
}