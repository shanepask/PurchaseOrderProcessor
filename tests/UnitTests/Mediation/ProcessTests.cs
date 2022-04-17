using System;
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
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns(new IHandler[]{ lineItemHandler.Object, resultHandler.Object});

            IMediator mediator = new Mediator(serviceProvider.Object);

            //act
            var act = ()=> mediator.ProcessAsync(customerId, purchaseOrder);

            //assert
            await act.Should().NotThrowAsync();
            act().Result.Should().BeNull();

            serviceProvider.Verify(s => s.GetService(It.IsAny<Type>()), Times.Once);
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

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns(Array.Empty<IHandler>());

            IMediator mediator = new Mediator(serviceProvider.Object);

            //act
            var act = () => mediator.ProcessAsync(customerId, purchaseOrder);

            //assert
            await act.Should().ThrowAsync<Mediator.NoHandlersException>();
            serviceProvider.Verify(s => s.GetService(It.IsAny<Type>()), Times.Once);
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
            var serviceProvider = new Mock<IServiceProvider>();

            resultHandler
                .Setup(s => s.HandleAsync(It.IsAny<int>(), It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
                .Callback((int _, IContext c, CancellationToken _) => c.ShippingSlip = shippingSlip);
            serviceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Returns(new IHandler[] { resultHandler.Object });

            IMediator mediator = new Mediator(serviceProvider.Object);

            //act
            var act = () => mediator.ProcessAsync(customerId, purchaseOrder);

            //assert
            await act.Should().NotThrowAsync();
            act().Result.Should().Be(shippingSlip);

            serviceProvider.Verify(s => s.GetService(It.IsAny<Type>()), Times.Once);
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

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup(s => s.GetService(It.IsAny<Type>())).Throws<TestException>();

            IMediator mediator = new Mediator(serviceProvider.Object);

            //act
            var act = () => mediator.ProcessAsync(customerId, purchaseOrder);

            //assert
            await act.Should().ThrowAsync<TestException>();
            serviceProvider.Verify(s => s.GetService(It.IsAny<Type>()), Times.Once);
        }
    }
}
