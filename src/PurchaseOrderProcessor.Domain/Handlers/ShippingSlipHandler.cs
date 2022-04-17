using System.Threading;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Mediation;

namespace PurchaseOrderProcessor.Domain.Handlers
{
    public class ShippingSlipHandler : IResultHandler
    {
        public Task HandleAsync(int customerId, IContext context, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
