using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PurchaseOrderProcessor.Domain.Clients;
using PurchaseOrderProcessor.Domain.Handlers;
using PurchaseOrderProcessor.Domain.Mediation;
using Xunit;

namespace UnitTests.BL.MembershipHandlerTests
{
    public class HandleTests
    {
        [Theory]
        [InlineData("Book \"Abc\"")]
        [InlineData("Book Abc")]
        [InlineData("Book")]
        [InlineData("Book Membership")]
        [InlineData("Video \"Abc\"")]
        [InlineData("Video Abc")]
        [InlineData("Video")]
        [InlineData("Video Membership")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("XX")]
        public async Task HandleNoneMembershipLines_ExpectsNoItems(string lineItem)
        {
            //arrange
            var fixture = new Fixture();
            var customerId = fixture.Create<int>();
            var context = new Context();
            var customerMock = new Mock<ICustomerClient>();
            
            ILineItemHandler handler = new MembershipHandler(customerMock.Object);

            //act
            var act = () => handler.HandleAsync(customerId, lineItem, context);

            //assert
            await act.Should().NotThrowAsync();
            context.PhysicalProducts.Should().BeEmpty();
            context.ShippingSlip.Should().BeNull();
            customerMock.Verify(c => c.MembershipUpdateAsync(customerId, lineItem, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("Book Club Membership")]
        [InlineData("Video Club Membership")]
        [InlineData("Premium Membership")]
        public async Task HandleMembershipLines_ExpectsSuccess(string lineItem)
        {
            //arrange
            var fixture = new Fixture();
            var customerId = fixture.Create<int>();
            var context = new Context();
            var customerMock = new Mock<ICustomerClient>();

            ILineItemHandler handler = new MembershipHandler(customerMock.Object);

            //act
            var act = () => handler.HandleAsync(customerId, lineItem, context);

            //assert
            await act.Should().NotThrowAsync();
            context.PhysicalProducts.Should().BeEmpty();
            context.ShippingSlip.Should().BeNull();
            customerMock.Verify(c => c.MembershipUpdateAsync(customerId, lineItem, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
