using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Clients;

namespace PurchaseOrderProcessor.Infrastructure.Clients
{
    public class CustomerApiClient : ICustomerClient
    {
        record CustomerMembershipUpdate
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("activated")]
            public bool Activated { get; set; }
        }

        public class Settings
        {
            public string BaseUrl { get; set; }
        }

        private readonly HttpClient _httpClient;

        public CustomerApiClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task MembershipUpdateAsync(int customerId, string membership, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(membership))
                throw new ArgumentException("Membership is invalid and cannot be updated for customer.", membership);

            var membershipUpdate = new CustomerMembershipUpdate { Name = membership, Activated = true };
            using var resp = await _httpClient.PutAsync($"/customers/{customerId}/membership", new StringContent(JsonSerializer.Serialize(membershipUpdate)), cancellationToken);
            resp.EnsureSuccessStatusCode();
        }
    }
}
