using System.Threading;
using System.Threading.Tasks;

namespace PurchaseOrderProcessor.Domain.Mediation
{
    public interface IResultHandler : IHandler
    {
        public Task HandleAsync(int customerId, IContext context, CancellationToken cancellationToken = default);
    }
}
