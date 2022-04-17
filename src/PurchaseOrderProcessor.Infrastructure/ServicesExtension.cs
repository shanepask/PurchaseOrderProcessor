using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PurchaseOrderProcessor.Domain.Clients;
using PurchaseOrderProcessor.Domain.Mediation;
using PurchaseOrderProcessor.Infrastructure.Clients;

namespace PurchaseOrderProcessor.Infrastructure
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddPurchaseOrderProcessor(this IServiceCollection services, IOptions<CustomerApiClient.Settings> options)
        {
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<ICustomerClient, CustomerApiClient>();
            services.AddHttpClient<ICustomerClient, CustomerApiClient>().ConfigureHttpClient(c=>c.BaseAddress = new Uri(options.Value.BaseUrl));
            return services;
        }
        public static IServiceCollection AddHandler<THandler>(this IServiceCollection services) where THandler : class, IHandler
        {
            services.AddTransient<IHandler, THandler>();
            return services;
        }
    }
}
