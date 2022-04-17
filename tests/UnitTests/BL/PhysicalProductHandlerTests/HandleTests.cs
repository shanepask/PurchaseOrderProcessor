using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using PurchaseOrderProcessor.Domain.Handlers;
using PurchaseOrderProcessor.Domain.Mediation;
using Xunit;

namespace UnitTests.BL.PhysicalProductHandlerTests
{
    public class HandleTests
    {
        [Theory]
        [InlineData("Book \"Abc\"")]
        [InlineData("Book Abc")]
        [InlineData("Book")]
        [InlineData("Video \"Abc\"")]
        [InlineData("Video Abc")]
        [InlineData("Video")]
        public async Task HandlePhysicalLines_ExpectsSuccess(string lineItem)
        {
            //arrange
            var fixture = new Fixture();
            var customerId = fixture.Create<int>();
            var context = new Context();

            ILineItemHandler handler = new PhysicalProductHandler();

            //act
            var act = () => handler.HandleAsync(customerId, lineItem, context);

            //assert
            await act.Should().NotThrowAsync();
            context.PhysicalProducts.Should().Contain(lineItem);
            context.ShippingSlip.Should().BeNull();
        }

        [Theory]
        [InlineData("Book Club Membership")]
        [InlineData("Video Club Membership")]
        [InlineData("Premium Membership")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("XX")]
        public async Task HandleNonePhysicalLines_ExpectsNoItems(string lineItem)
        {
            //arrange
            var fixture = new Fixture();
            var customerId = fixture.Create<int>();
            var context = new Context();

            ILineItemHandler handler = new PhysicalProductHandler();

            //act
            var act = () => handler.HandleAsync(customerId, lineItem, context);

            //assert
            await act.Should().NotThrowAsync();
            context.PhysicalProducts.Should().NotContain(lineItem);
            context.ShippingSlip.Should().BeNull();
        }
    }
}
