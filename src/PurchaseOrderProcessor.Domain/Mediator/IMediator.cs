using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Domain.Mediator
{
    public interface IMediator
    {
        public ShippingSlip ProcessAsync(int customerId, PurchaseOrder purchaseOrder);
    }
}
