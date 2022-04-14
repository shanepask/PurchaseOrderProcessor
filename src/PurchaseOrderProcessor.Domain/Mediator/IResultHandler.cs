using System.Threading.Tasks;

namespace PurchaseOrderProcessor.Domain.Mediator
{
    public interface IResultHandler : IHandler
    {
        public Task HandleAsync(int customerId, IContext context);
    }
}
