#if DEBUG

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PurchaseOrderProcessor.Infrastructure
{
    public class DemoHttpHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
    }
}

#endif