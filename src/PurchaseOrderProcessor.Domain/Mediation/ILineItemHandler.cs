using System.Threading;
using System.Threading.Tasks;

namespace PurchaseOrderProcessor.Domain.Mediation
{
    public interface ILineItemHandler : IHandler
    {
        public Task HandleAsync(int customerId, string lineItem, IContext context, CancellationToken cancellationToken = default);
    }
}
