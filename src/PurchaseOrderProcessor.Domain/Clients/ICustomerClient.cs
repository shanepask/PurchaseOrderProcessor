using System.Threading.Tasks;

namespace PurchaseOrderProcessor.Domain.Clients
{
    public interface ICustomerClient
    {
        public Task MembershipUpdateAsync(int customerId, string membership);
    }
}
