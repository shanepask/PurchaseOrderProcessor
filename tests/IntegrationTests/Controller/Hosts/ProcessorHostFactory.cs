using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PurchaseOrderProcessor.Domain.Clients;
using PurchaseOrderProcessor.Infrastructure.Clients;

namespace IntegrationTests.Controller.Hosts
{
    internal class ProcessorHostFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public class RecorderHttpHandler : HttpMessageHandler
        {
            public IEnumerable<HttpRequestMessage> Messages { get; private set; } = Array.Empty<HttpRequestMessage>();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Messages = Messages.Concat(new[] { request });
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
            }
        }

        public RecorderHttpHandler CustomerApiMessages { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddHttpClient<ICustomerClient, CustomerApiClient>().ConfigurePrimaryHttpMessageHandler(_ => CustomerApiMessages);
            });
        }
    }
}
