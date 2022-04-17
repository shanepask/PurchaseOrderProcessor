using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PurchaseOrderProcessor.Api;
using PurchaseOrderProcessor.Domain.Clients;
using PurchaseOrderProcessor.Domain.Mediation;
using Xunit;

namespace UnitTests.Host
{
    public class StartupTests
    {
        [Fact]
        public void ConfigureServices_ExpectsSuccess()
        {
            //arrange
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var serviceCollection = new ServiceCollection();
            var startup = new Startup(config);

            //act
            startup.ConfigureServices(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            //assert
            serviceCollection.Count.Should().BeInRange(10, 50);
            provider.GetService<IMediator>().Should().NotBeNull();
            provider.GetService<IEnumerable<IHandler>>().Should().HaveCount(3);
            provider.GetService<IEnumerable<ICustomerClient>>().Should().NotBeNull();
        }

        [Theory]
        [InlineData("Development")]
        [InlineData("Staging")]
        [InlineData("Docker")]
        [InlineData("Production")]
        [InlineData("Other")]
        [InlineData("")]
        [InlineData(null)]
        public void Configure_ExpectsSuccess(string environment)
        {
            //arrange
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var serviceCollection = new ServiceCollection();
            var startup = new Startup(config);
            var hostMock = new Mock<IWebHostEnvironment>();

            hostMock.Setup(h => h.EnvironmentName).Returns(environment);

            startup.ConfigureServices(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            var app = new ApplicationBuilder(provider);

            //act
            var act = () => startup.Configure(app, hostMock.Object);
            var reqDelegate = () => app.Build();

            //assert
            act.Should().NotThrow();
            app.Properties.Count.Should().Be(2);
            reqDelegate.Should().NotThrow();
        }
    }
}
