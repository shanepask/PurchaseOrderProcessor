using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using PurchaseOrderProcessor.Domain.Handlers;
using PurchaseOrderProcessor.Domain.Mediation;
using Xunit;

namespace UnitTests.BL.ShippingSlipGenerationHandlerTests
{
    public class HandleTests
    {
        [Fact]
        public async Task HandlePhysicalLines_ExpectsSuccess()
        {
            //arrange
            var fixture = new Fixture();
            var customerId = fixture.Create<int>();
            var context = new Context();
            context.PhysicalProducts.AddRange(new Fixture().CreateMany<string>());
            context.PhysicalProducts.Add("Duplicate");
            context.PhysicalProducts.Add("Duplicate");

            IResultHandler handler = new ShippingSlipHandler();

            //act
            var act = () => handler.HandleAsync(customerId, context);

            //assert
            await act.Should().NotThrowAsync();
            context.ShippingSlip.Should().NotBeNull();
            context.ShippingSlip.Items.Should().HaveCount(context.PhysicalProducts.Count - 1);
            context.ShippingSlip.Items.First(i => i.Description == "Duplicate").Quantity.Should().Be(2);
        }

        [Fact]
        public async Task HandleNoPhysicalLines_ExpectsNoItems()
        {
            //arrange
            var fixture = new Fixture();
            var customerId = fixture.Create<int>();
            var context = new Context();

            IResultHandler handler = new ShippingSlipHandler();

            //act
            var act = () => handler.HandleAsync(customerId, context);

            //assert
            await act.Should().NotThrowAsync();
            context.ShippingSlip.Should().BeNull();
        }
    }
}
