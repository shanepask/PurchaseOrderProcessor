using System;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Clients;

namespace PurchaseOrderProcessor.Infrastructure.Clients
{
    public class CustomerApiClient : ICustomerClient
    {
        public Task MembershipUpdateAsync(int customerId, string membership)
        {
            throw new NotImplementedException();
        }
    }
}
