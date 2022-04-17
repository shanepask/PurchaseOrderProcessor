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
using PurchaseOrderProcessor.Domain.Models;
using TechTalk.SpecFlow;

namespace IntegrationTests.Features.Steps
{
    [Binding]
    public class ProcessPurchaseOrdersStepDefinitions
    {
        private const string FEATURE_NAME = "PPOSteps";
        private const string PURCHASE_ORDER_KEY = $"{FEATURE_NAME}_{nameof(PurchaseOrder)}";
        private const string PURCHASE_CUSTOMER_ID = $"{FEATURE_NAME}_CustoemrId";
        private const string PURCHASE_RESPONSE = $"{FEATURE_NAME}_Response";
        private const string PURCHASE_SHIPPING_SLIP = $"{FEATURE_NAME}_{nameof(ShippingSlip)}";
        private const string PURCHASE_HOST_FACTORY = $"{FEATURE_NAME}_{nameof(ProcessorHostFactory<Startup>)}";

        private readonly ScenarioContext _scenarioContext;

        public ProcessPurchaseOrdersStepDefinitions(ScenarioContext scenarioContext)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            _scenarioContext = scenarioContext;
        }

        [Given(@"I a purchase with the ID of ""([^""]*)"" for customer ""([^""]*)"" has the line items")]
        public void GivenIAPurchaseWithTheIDOfForCustomerHasTheLineItems(int poId, int customerId, Table poLines)
        {
            _scenarioContext.TryAdd(PURCHASE_ORDER_KEY, new PurchaseOrder { Id = poId, Items = poLines.Rows.Select(r => r[0]) }).Should().BeTrue();
            _scenarioContext.TryAdd(PURCHASE_CUSTOMER_ID, customerId).Should().BeTrue();
        }

        [When(@"It is processed")]
        public async Task WhenItIsProcessed()
        {
            var po = _scenarioContext.Get<PurchaseOrder>(PURCHASE_ORDER_KEY);
            var cid = _scenarioContext.Get<int>(PURCHASE_CUSTOMER_ID);
            var factory = new ProcessorHostFactory<Startup>();
            var client = factory.CreateClient();
            _scenarioContext.TryAdd(PURCHASE_HOST_FACTORY, factory).Should().BeTrue();
            var resp = await client.PostAsync($"{cid}/purchase-orders", new StringContent(JsonSerializer.Serialize(po), Encoding.UTF8, "application/json"));
            _scenarioContext.TryAdd(PURCHASE_RESPONSE, resp).Should().BeTrue();
        }

        [Then(@"It is successful")]
        public void ThenItIsSuccessful()
        {
            var resp = _scenarioContext.Get<HttpResponseMessage>(PURCHASE_RESPONSE);
            resp.IsSuccessStatusCode.Should().BeTrue();
        }

        [Then(@"It produces a shipping slip")]
        public async Task ThenItProducesAShippingSlip(Table shippingSlipLines)
        {
            ThenItIsSuccessful();
            var resp = _scenarioContext.Get<HttpResponseMessage>(PURCHASE_RESPONSE);
            var slip = await resp.Content.ReadFromJsonAsync<ShippingSlip>();

            slip.Should().NotBeNull();
            slip.Items.Should().HaveCount(shippingSlipLines.RowCount);
            _scenarioContext.TryAdd(PURCHASE_SHIPPING_SLIP, slip).Should().BeTrue();

            var testLines = shippingSlipLines.Rows.Select(r => new ShippingSlipItem { Description = r[0], Quantity = int.Parse(r[1]) });
            slip.Items.Should().BeEquivalentTo(testLines);
        }

        [Then(@"The customers current membership is ""([^""]*)""")]
        public async Task ThenTheCustomersCurrentMembershipIs(string customerState)
        {
            var cid = _scenarioContext.Get<int>(PURCHASE_CUSTOMER_ID);
            var factory = _scenarioContext.Get<ProcessorHostFactory<Startup>>(PURCHASE_HOST_FACTORY);
            var message = factory.CustomerApiMessages.Messages.LastOrDefault(m =>
                m.Method == HttpMethod.Put &&
                m.RequestUri?.AbsolutePath == $"/customers/{cid}/membership");
            message.Should().NotBeNull();
            (await message.Content.ReadAsStringAsync()).Contains(customerState);
        }

        [Then(@"It produces no shipping slip")]
        public async Task ThenItProducesNoShippingSlip()
        {
            ThenItIsSuccessful();
            var resp = _scenarioContext.Get<HttpResponseMessage>(PURCHASE_RESPONSE);
            (await resp.Content.ReadAsStringAsync()).Length.Should().Be(0);
        }
    }
}
