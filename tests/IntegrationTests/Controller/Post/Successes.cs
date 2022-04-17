using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using IntegrationTests.Controller.Hosts;
using PurchaseOrderProcessor.Api;
using PurchaseOrderProcessor.Domain.Models;
using Xunit;

namespace IntegrationTests.Controller.Post
{
    [Collection("UsingEnvironment")]
    public class Successes
    {
        [Theory]
        [InlineData("Development")]
        [InlineData("Docker")]
        public async Task WhenProcessed_WithAMixOfProducts_ThenItProcessesSuccessfully(string environment)
        {
            //arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
            var client = new ProcessorHostFactory<Startup>().CreateClient();
            var customerId = 9393;
            var purchaseOrder = new PurchaseOrder
            {
                Id = 123,
                Items = new []
                {
                    "Premium Membership",
                    "Book \"Test Book\"",
                    "Video \"Test Video\"",
                    "Video \"Test Video\""
                }
            };

            //act
            var resp = await client.PostAsync($"{customerId}/purchase-orders", new StringContent(JsonSerializer.Serialize(purchaseOrder), Encoding.UTF8, "application/json"));

            //assert
            resp.IsSuccessStatusCode.Should().Be(true);
            var shippingSlip = await resp.Content.ReadFromJsonAsync<ShippingSlip>();
            shippingSlip.Should().NotBeNull();
            shippingSlip.Items.Should().ContainEquivalentOf(new ShippingSlipItem { Description = "Book \"Test Book\"", Quantity = 1 });
            shippingSlip.Items.Should().ContainEquivalentOf(new ShippingSlipItem { Description = "Video \"Test Video\"", Quantity = 2 });

        }


        [Theory]
        [InlineData("Development")]
        [InlineData("Docker")]
        public async Task WhenProcessed_WithAMembershipOnlyProducts_ThenItProcessesSuccessfully(string environment)
        {
            //arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
            var client = new ProcessorHostFactory<Startup>().CreateClient();
            var customerId = 9393;
            var purchaseOrder = new PurchaseOrder
            {
                Id = 123,
                Items = new[]
                {
                    "Premium Membership"
                }
            };

            //act
            var resp = await client.PostAsync($"{customerId}/purchase-orders", new StringContent(JsonSerializer.Serialize(purchaseOrder), Encoding.UTF8, "application/json"));

            //assert
            resp.IsSuccessStatusCode.Should().Be(true);
            var shippingSlip = (await resp.Content.ReadAsStringAsync()).Length.Should().Be(0);
        }
    }
}
