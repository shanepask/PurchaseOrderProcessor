using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using PurchaseOrderProcessor.Domain.Mediation;
using PurchaseOrderProcessor.Infrastructure;
using PurchaseOrderProcessor.Infrastructure.Clients;
using Xunit;

namespace UnitTests.Host
{
    public class ServicesTests
    {
        class HandlerMock1 : IHandler { }
        class HandlerMock2 : IHandler { }

        [Fact]
        public void AddPurchaseOrderProcessor_ExpectsFailure()
        {
            //arrange
            var serviceCollection = new ServiceCollection();
            var optionsMock = new Mock<IOptions<CustomerApiClient.Settings>>();

            //act
            serviceCollection.AddPurchaseOrderProcessor(optionsMock.Object);

            //assert
            serviceCollection.Count.Should().Be(21);
        }
        
        [Fact]
        public void AddHandler_ExpectsSuccess()
        {
            //arrange
            var serviceCollection = new ServiceCollection();

            //act
            serviceCollection.AddHandler<HandlerMock1>();
            serviceCollection.AddHandler<HandlerMock2>();
            var provider = serviceCollection.BuildServiceProvider();

            //assert
            serviceCollection.Count.Should().Be(2);
            (provider.GetService<IEnumerable<IHandler>>() ?? Array.Empty<IHandler>()).Count().Should().Be(2);
        }
    }
}
