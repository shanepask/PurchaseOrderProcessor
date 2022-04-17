#if DEBUG

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using PurchaseOrderProcessor.Infrastructure;
using Xunit;

namespace UnitTests.Host
{
    public class DemoCustomerClientHandlerTests
    {
        [Fact]
        public async Task HandleHttpRequest_ExpectsSuccess()
        {
            //arrange
            var handler = new DemoHttpHandler();
            var client = new HttpClient(handler);

            //act
            var act = () => client.SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://localhost:3000/"));

            //assert
            (await act.Should().NotThrowAsync()).Which.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
#endif
