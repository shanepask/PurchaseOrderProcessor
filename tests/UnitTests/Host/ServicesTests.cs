using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

using PurchaseOrderProcessor.Domain;
using PurchaseOrderProcessor.Domain.Mediation;
using PurchaseOrderProcessor.Infrastructure;
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

            //act
            serviceCollection.AddPurchaseOrderProcessor();

            //assert
            serviceCollection.Count.Should().Be(1);
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
