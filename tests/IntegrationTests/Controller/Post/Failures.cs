using System;
using System.Net;
using System.Net.Http;
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
    public class Failures
    {
        [Theory]
        [InlineData("Staging")]
        [InlineData("Production")]
        public async Task WhenProcessed_WithAMembershipOnlyProducts_ThenItFailsToConnectToCustomerApi(string environment)
        {
            //arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
            var factory = new ProcessorHostFactory<Startup>();
            var client = factory.CreateClient();
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
            resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
            factory.CustomerApiMessages.Messages.Should().HaveCount(1);
        }

        [Fact]
        public async Task WhenProcessed_WithinProduction_ThenSwaggerShouldBeUnavailable()
        {
            //arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
            var client = new ProcessorHostFactory<Startup>().CreateClient();

            //act
            var resp = await client.GetAsync("/swagger");

            //assert
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("application/xml")]
        [InlineData("plain/text")]
        public async Task WhenProcessed_WithUnsupportedContentType_ThenItFailsToConnectToCustomerApi(string contentType)
        {
            //arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var client = new ProcessorHostFactory<Startup>().CreateClient();
            var customerId = 9393;
            var purchaseOrder = new PurchaseOrder
            {
                Id = 123
            };

            //act
            var resp = await client.PostAsync($"{customerId}/purchase-orders", new StringContent(JsonSerializer.Serialize(purchaseOrder), Encoding.UTF8, contentType));

            //assert
            resp.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task WhenProcessed_WithMalformedContent_ThenItFails()
        {
            //arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var client = new ProcessorHostFactory<Startup>().CreateClient();
            var customerId = 9393;

            //act
            var resp = await client.PostAsync($"{customerId}/purchase-orders", new StringContent("{ {} }", Encoding.UTF8, "application/json"));

            //assert
            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
