using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using PurchaseOrderProcessor.Api;
using PurchaseOrderProcessor.Domain.Clients;
using PurchaseOrderProcessor.Domain.Mediation;
using Xunit;

namespace UnitTests.Host
{
    public class StartupTests
    {
        class MockWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
        {
            public ServiceProvider ServiceProvider { get; private set; }
            private string _environment;

            public MockWebApplicationFactory(string environment) => _environment = environment;

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.ConfigureServices(services => ServiceProvider = services.BuildServiceProvider());
                builder.UseEnvironment(_environment);
            }
        }

        [Theory]
        [InlineData("Development")]
        [InlineData("Staging")]
        [InlineData("Docker")]
        [InlineData("Production")]
        [InlineData("Other")]
        [InlineData("")]
        [InlineData(null)]
        public void ConfigureServices_ExpectsSuccess(string environment)
        {
            //arrange
            var serviceCollection = new ServiceCollection();
            var appSettings = new Dictionary<string, string> { { "CustomerApi:BaseUrl", "http://localhost:3000" } };
            var config = new ConfigurationBuilder().AddInMemoryCollection(appSettings).Build();
            var env = new HostingEnvironment { EnvironmentName = environment, ContentRootPath = "/"};
            var startup = new Startup(config, env);

            //act
            startup.ConfigureServices(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            //assert
            serviceCollection.Count.Should().BeInRange(100, 150);
            provider.GetService<IEnumerable<IHandler>>().Should().HaveCount(3);
            provider.GetService<IMediator>().Should().NotBeNull();
            provider.GetService<ICustomerClient>().Should().NotBeNull();
        }

        [Theory]
        [InlineData("Development")]
        [InlineData("Staging")]
        [InlineData("Docker")]
        [InlineData("Production")]
        [InlineData("Other")]
        [InlineData("")]
        public void Configure_ExpectsSuccess(string environment)
        {
            //arrange
            var hostMock = new MockWebApplicationFactory<Startup>(environment);
            var app = new ApplicationBuilder(hostMock.ServiceProvider);

            //act
            var reqDelegate = () => app.Build();
            hostMock.CreateClient();

            //assert
            app.Properties.Count.Should().Be(1);
            reqDelegate.Should().NotThrow();
        }
    }
}
