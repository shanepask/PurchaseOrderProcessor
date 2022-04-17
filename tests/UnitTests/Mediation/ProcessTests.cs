using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using PurchaseOrderProcessor.Domain.Mediation;
using PurchaseOrderProcessor.Domain.Models;
using Xunit;

namespace UnitTests.Mediation
{
    public class ProcessTests
    {
        [Theory]
        [InlineData(4567890, 3344656, 3)]
        [InlineData(0,0,0)]
        [InlineData(int.MinValue, int.MinValue, 10)]
        [InlineData(int.MaxValue, int.MaxValue, 10)]
        public async Task WithHandlers_ExpectsSuccess(int customerId, int poId, int numLineItems)
        {
            //arrange
            var fixture = new Fixture();
            var purchaseOrder = new PurchaseOrder
            {
                Id = poId, 
                Items = fixture.CreateMany<string>(numLineItems)
            };

            var lineItemHandler = new Mock<ILineItemHandler>();
            var resultHandler = new Mock<IResultHandler>();
            var handlerMocks = new Mock<IEnumerable<IHandler>>();

            IMediator mediator = new Mediator(handlerMocks.Object);

            //act
            var act = ()=> mediator.ProcessAsync(customerId, purchaseOrder);

            //assert
            await act.Should().NotThrowAsync();
            act().Result.Should().BeNull();

            lineItemHandler.Verify(s => s.HandleAsync(It.Is<int>(v=>v == customerId), It.IsIn(purchaseOrder.Items), It.IsAny<IContext>(), It.IsAny<CancellationToken>()), Times.Once);
            resultHandler.Verify(s => s.HandleAsync(It.Is<int>(v => v == customerId), It.IsAny<IContext>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(4567890, 3344656, 3)]
        [InlineData(0, 0, 0)]
        [InlineData(int.MinValue, int.MinValue, 10)]
        [InlineData(int.MaxValue, int.MaxValue, 10)]
        public async Task WithoutHandlers_ExpectsFailure(int customerId, int poId, int numLineItems)
        {
            //arrange
            var fixture = new Fixture();
            var purchaseOrder = new PurchaseOrder
            {
                Id = poId,
                Items = fixture.CreateMany<string>(numLineItems)
            };

            var handlerMocks = new Mock<IEnumerable<IHandler>>();

            IMediator mediator = new Mediator(handlerMocks.Object);

            //act
            var act = () => mediator.ProcessAsync(customerId, purchaseOrder);

            //assert
            await act.Should().ThrowAsync<Mediator.NoHandlersException>();
        }

        [Theory]
        [InlineData(4567890, 3344656, 3)]
        [InlineData(0, 0, 0)]
        [InlineData(int.MinValue, int.MinValue, 10)]
        [InlineData(int.MaxValue, int.MaxValue, 10)]
        public async Task WithShippingSlipHandlerResult_ExpectsSuccess(int customerId, int poId, int numLineItems)
        {
            //arrange
            var fixture = new Fixture();
            var shippingSlip = fixture.Create<ShippingSlip>();
            var purchaseOrder = new PurchaseOrder
            {
                Id = poId,
                Items = fixture.CreateMany<string>(numLineItems)
            };

            var resultHandler = new Mock<IResultHandler>();
            var handlerMocks = new Mock<IEnumerable<IHandler>>();

            resultHandler
                .Setup(s => s.HandleAsync(It.IsAny<int>(), It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
                .Callback((int _, IContext c, CancellationToken _) => c.ShippingSlip = shippingSlip);

            IMediator mediator = new Mediator(handlerMocks.Object);

            //act
            var act = () => mediator.ProcessAsync(customerId, purchaseOrder);

            //assert
            await act.Should().NotThrowAsync();
            act().Result.Should().Be(shippingSlip);

            resultHandler.Verify(s => s.HandleAsync(It.Is<int>(v => v == customerId), It.IsAny<IContext>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Theory]
        [InlineData(4567890, 3344656, 3)]
        [InlineData(0, 0, 0)]
        [InlineData(int.MinValue, int.MinValue, 10)]
        [InlineData(int.MaxValue, int.MaxValue, 10)]
        public async Task WithHandlerException_ExpectsFailure(int customerId, int poId, int numLineItems)
        {
            //arrange
            var fixture = new Fixture();
            var purchaseOrder = new PurchaseOrder
            {
                Id = poId,
                Items = fixture.CreateMany<string>(numLineItems)
            };

            var handlerMocks = new Mock<IEnumerable<IHandler>>();
            
            IMediator mediator = new Mediator(handlerMocks.Object);

            //act
            var act = () => mediator.ProcessAsync(customerId, purchaseOrder);

            //assert
            await act.Should().ThrowAsync<TestException>();
        }
    }
}
