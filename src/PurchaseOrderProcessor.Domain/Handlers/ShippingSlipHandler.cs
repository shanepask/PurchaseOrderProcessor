using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Mediator;

namespace PurchaseOrderProcessor.Domain.Handlers
{
    public class ShippingSlipHandler : IResultHandler
    {
        public Task HandleAsync(int customerId, IContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
