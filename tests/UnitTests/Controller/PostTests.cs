using System;
using System.Threading;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PurchaseOrderProcessor.Api.Controllers;
using PurchaseOrderProcessor.Domain.Mediation;
using PurchaseOrderProcessor.Domain.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace UnitTests.Controller
{
    public class PostTests
    {
        [Fact]
        public async Task PostPurchaseOrder_ExpectsNoContext()
        {
            //arrange
            var fixture = new Fixture();
            var customerId = fixture.Create<int>();
            var po = fixture.Create<PurchaseOrder>();
            var loggerMock = new Mock<ILogger<PurchaseOrderController>>();
            var mediatorMock = new Mock<IMediator>();

            mediatorMock
                .Setup(m => m.ProcessAsync(It.IsAny<int>(), It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ShippingSlip)null);

            var controller = new PurchaseOrderController(loggerMock.Object);

            //act
            var res = await controller.Post(po, customerId, mediatorMock.Object, CancellationToken.None);

            //assert
            res.Should().BeAssignableTo<NoContentResult>();

            mediatorMock.Verify(m => m.ProcessAsync(customerId, po, It.IsAny<CancellationToken>()), Times.Once());
            loggerMock.Verify(l => l.Log(LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.AtLeastOnce());
        }

        [Fact]
        public async Task PostPurchaseOrder_ExpectsShippingSlip()
        {
            //arrange
            var fixture = new Fixture();
            var customerId = fixture.Create<int>();
            var po = fixture.Create<PurchaseOrder>();
            var slip = fixture.Create<ShippingSlip>();
            var loggerMock = new Mock<ILogger<PurchaseOrderController>>();
            var mediatorMock = new Mock<IMediator>();

            mediatorMock
                .Setup(m => m.ProcessAsync(It.IsAny<int>(), It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(slip);

            var controller = new PurchaseOrderController(loggerMock.Object);

            //act
            var res = await controller.Post(po, customerId, mediatorMock.Object, CancellationToken.None);

            //assert
            res.Should().BeAssignableTo<OkObjectResult>();
            ((OkObjectResult)res).Value.Should().BeEquivalentTo(slip);

            mediatorMock.Verify(m => m.ProcessAsync(customerId, po, It.IsAny<CancellationToken>()), Times.Once());
            loggerMock.Verify(l => l.Log(LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.AtLeastOnce());
        }

        [Fact]
        public async Task PostPurchaseOrder_WithMediatorError_ExpectsFailure()
        {
            //arrange
            var fixture = new Fixture();
            var customerId = fixture.Create<int>();
            var po = fixture.Create<PurchaseOrder>();
            var loggerMock = new Mock<ILogger<PurchaseOrderController>>();
            var mediatorMock = new Mock<IMediator>();

            mediatorMock
                .Setup(m => m.ProcessAsync(It.IsAny<int>(), It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TestException());

            var controller = new PurchaseOrderController(loggerMock.Object);

            //act
            var act = ()=> controller.Post(po, customerId, mediatorMock.Object, CancellationToken.None);

            //assert
            await act.Should().ThrowAsync<TestException>();
            loggerMock.Verify(l => l.Log(LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<TestException>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.AtLeastOnce());
        }
    }
}
