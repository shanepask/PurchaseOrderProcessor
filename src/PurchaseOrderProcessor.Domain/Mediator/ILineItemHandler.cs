using System.Threading.Tasks;

namespace PurchaseOrderProcessor.Domain.Mediator
{
    public interface ILineItemHandler : IHandler
    {
        public Task HandleAsync(int customerId, string lineItem, IContext context);
    }
}
