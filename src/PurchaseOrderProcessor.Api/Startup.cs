using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PurchaseOrderProcessor.Api.SwaggerFilters;
using PurchaseOrderProcessor.Domain.Handlers;
using PurchaseOrderProcessor.Infrastructure;
using PurchaseOrderProcessor.Infrastructure.Clients;

namespace PurchaseOrderProcessor.Api
{
    /// <summary>
    /// Startup process for this app.
    /// </summary>
    public class Startup
    {
        internal static string GeneratedContentLocation { get; } = Assembly.GetEntryAssembly()?.Location.Replace(".dll", ".xml");

        private readonly IConfiguration _configuration;

        /// <summary>
        /// Create a new startup class for the environment specified.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddConfiguration(configuration)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
            _configuration = builder.Build();
        }

        /// <summary>
        /// Configure services
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PurchaseOrderProcessor.Api", Version = "v1", Description = "An API to handle customer purchase orders."});
                c.TagActionsBy(a => new List<string> { a.GroupName });
                c.DocInclusionPredicate((_, _) => true);
                c.IncludeXmlComments(GeneratedContentLocation);
                c.OperationFilter<SwaggerExamplesFilter>();
            });

            var customerApiOptions = Options.Create(_configuration.GetRequiredSection("CustomerApi").Get<CustomerApiClient.Settings>());
            services.AddPurchaseOrderProcessor(customerApiOptions)
                .AddHandler<MembershipHandler>()
                .AddHandler<PhysicalProductHandler>()
                .AddHandler<ShippingSlipHandler>();
        }

        /// <summary>
        /// Configure app host
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PurchaseOrderProcessor.Api v1"));
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
