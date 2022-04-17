using System;
using System.Net.Http;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Clients;

namespace PurchaseOrderProcessor.Infrastructure.Clients
{
    public class CustomerApiClient : ICustomerClient
    {
        private readonly HttpClient _httpClient;

        public CustomerApiClient(HttpClient httpClient) => _httpClient = httpClient;

        public Task MembershipUpdateAsync(int customerId, string membership)
        {
            throw new NotImplementedException();
        }
    }
}
