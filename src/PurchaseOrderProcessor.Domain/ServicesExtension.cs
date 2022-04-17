using System;
using Microsoft.Extensions.DependencyInjection;
using PurchaseOrderProcessor.Domain.Mediation;

namespace PurchaseOrderProcessor.Domain
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddPurchaseOrderProcessor(this IServiceCollection services)
        {
            throw new NotImplementedException();
        }
        public static IServiceCollection AddHandler<THandler>(this IServiceCollection services) where THandler : class, IHandler, new()
        {
            throw new NotImplementedException();
        }
    }
}
