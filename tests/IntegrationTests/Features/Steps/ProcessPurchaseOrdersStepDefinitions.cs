using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using IntegrationTests.Controller.Hosts;
using PurchaseOrderProcessor.Api;
using PurchaseOrderProcessor.Domain.Clients;
using PurchaseOrderProcessor.Domain.Models;
using TechTalk.SpecFlow;

namespace IntegrationTests
{
    [Binding]
    public class ProcessPurchaseOrdersStepDefinitions
    {
        public const string FeatureName = "PPOSteps";
        public const string PurchaseOrderKey = $"{FeatureName}_{nameof(PurchaseOrder)}";
        public const string PurchaseCustomerId = $"{FeatureName}_CustoemrId";
        public const string PurchaseResponse = $"{FeatureName}_Response";
        public const string PurchaseShippingSlip = $"{FeatureName}_{nameof(ShippingSlip)}";
        public const string PurchaseHostFactory = $"{FeatureName}_{nameof(ProcessorHostFactory<Startup>)}";

        private readonly ScenarioContext _scenarioContext;

        public ProcessPurchaseOrdersStepDefinitions(ScenarioContext scenarioContext)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            _scenarioContext = scenarioContext;
        }

        [Given(@"I a purchase with the ID of ""([^""]*)"" for customer ""([^""]*)"" has the line items")]
        public void GivenIAPurchaseWithTheIDOfForCustomerHasTheLineItems(int poId, int customerid, Table poLines)
        {
            _scenarioContext.TryAdd(PurchaseOrderKey, new PurchaseOrder { Id = poId, Items = poLines.Rows.Select(r => r[0]) }).Should().BeTrue();
            _scenarioContext.TryAdd(PurchaseCustomerId, customerid).Should().BeTrue();
        }

        [When(@"It is processed")]
        public async Task WhenItIsProcessed()
        {
            var po = _scenarioContext.Get<PurchaseOrder>(PurchaseOrderKey);
            var cid = _scenarioContext.Get<int>(PurchaseCustomerId);
            var factory = new ProcessorHostFactory<Startup>();
            var client = factory.CreateClient();
            _scenarioContext.TryAdd(PurchaseHostFactory, factory).Should().BeTrue();
            var resp = await client.PostAsync($"{cid}/purchase-orders", new StringContent(JsonSerializer.Serialize(po), Encoding.UTF8, "application/json"));
            _scenarioContext.TryAdd(PurchaseResponse, resp).Should().BeTrue();
        }

        [Then(@"It is successful")]
        public void ThenItIsSuccessful()
        {
            var resp = _scenarioContext.Get<HttpResponseMessage>(PurchaseResponse);
            resp.IsSuccessStatusCode.Should().BeTrue();
        }

        [Then(@"It produces a shipping slip")]
        public async Task ThenItProducesAShippingSlip(Table shippingSlipLines)
        {
            ThenItIsSuccessful();
            var resp = _scenarioContext.Get<HttpResponseMessage>(PurchaseResponse);
            var slip = await resp.Content.ReadFromJsonAsync<ShippingSlip>();

            slip.Should().NotBeNull();
            slip.Items.Should().HaveCount(shippingSlipLines.RowCount);
            _scenarioContext.TryAdd(PurchaseShippingSlip, slip).Should().BeTrue();

            var testLines = shippingSlipLines.Rows.Select(r => new ShippingSlipItem { Description = r[0], Quantity = int.Parse(r[1]) });
            slip.Items.Should().BeEquivalentTo(testLines);
        }

        [Then(@"The customers current membership is ""([^""]*)""")]
        public async Task ThenTheCustomersCurrentMembershipIs(string customerState)
        {
            var cid = _scenarioContext.Get<int>(PurchaseCustomerId);
            var factory = _scenarioContext.Get<ProcessorHostFactory<Startup>>(PurchaseHostFactory);
            var message = factory.CustomerApiMessages.Messages.LastOrDefault(m =>
                m.Method == HttpMethod.Put &&
                m.RequestUri.AbsolutePath == $"/customers/{cid}/membership");
            message.Should().NotBeNull();
            (await message.Content.ReadAsStringAsync()).Contains(customerState);
        }

        [Then(@"It produces no shipping slip")]
        public async Task ThenItProducesNoShippingSlip()
        {
            ThenItIsSuccessful();
            var resp = _scenarioContext.Get<HttpResponseMessage>(PurchaseResponse);
            (await resp.Content.ReadAsStringAsync()).Length.Should().Be(0);
        }
    }
}
