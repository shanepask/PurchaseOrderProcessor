using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.Protected;
using PurchaseOrderProcessor.Domain.Clients;
using PurchaseOrderProcessor.Infrastructure.Clients;
using Xunit;

namespace UnitTests.CustomerClient
{
    public class MembershipUpdateTests
    {
        [Theory]
        [InlineData(4567890, "Book Club Membership")]
        [InlineData(4567890, "Video Club Membership")]
        [InlineData(4567890, "Premium Membership")]
        [InlineData(4567890, "XX")]
        [InlineData(0, "Book Club Membership")]
        [InlineData(int.MinValue, "Book Club Membership")]
        [InlineData(int.MaxValue, "Book Club Membership")]
        public async Task WithMembershipUpdate_ExpectsSuccess(int customerId, string membership)
        {
            //arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            ICustomerClient client = new CustomerApiClient(new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost:3000") });

            //act
            var act = () => client.MembershipUpdateAsync(customerId, membership, CancellationToken.None);

            //assert
            await act.Should().NotThrowAsync();
        }

        [Theory]
        [InlineData(4567890, "")]
        [InlineData(4567890, null)]
        public async Task WithoutMembershipUpdate_ExpectsFailure(int customerId, string membership)
        {
            //arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            ICustomerClient client = new CustomerApiClient(new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost:3000") });

            //act
            var act = () => client.MembershipUpdateAsync(customerId, membership, CancellationToken.None);

            //assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
        
        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        public async Task WithApiError_ExpectsFailure(HttpStatusCode statusCode)
        {
            //arrange
            var fixture = new Fixture();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = statusCode });

            ICustomerClient client = new CustomerApiClient(new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost:3000")});

            //act
            var act = () => client.MembershipUpdateAsync(fixture.Create<int>(), fixture.Create<string>(), CancellationToken.None);

            //assert
            await act.Should().ThrowAsync<HttpRequestException>();
        }
    }
}
