using System.Threading;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Domain.Mediation
{
    public interface IMediator
    {
        public Task<ShippingSlip> ProcessAsync(int customerId, PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    }
}
