using System;
using FluentAssertions;
using Moq;
using PurchaseOrderProcessor.Domain.Mediation;
using Xunit;

namespace UnitTests.Mediation
{
    public class CreateTests
    {
        [Fact]
        public void WithHandlers_ExpectsSuccess()
        {
            //arrange
            var handlerMocks = new[] { new Mock<IHandler>().Object };

            //act
            var act = () => new Mediator(handlerMocks);

            //assert
            act.Should().NotThrow();
        }

        [Fact]
        public void WithoutHandlers_ExpectsFailure()
        {
            //arrange
            var handlerMocks = Array.Empty<IHandler>();

            //act
            var act = () => new Mediator(handlerMocks);

            //assert
            act.Should().Throw<Mediator.NoHandlersException>();
        }
    }
}
