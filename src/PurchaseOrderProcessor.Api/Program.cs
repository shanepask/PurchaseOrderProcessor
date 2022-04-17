using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PurchaseOrderProcessor.Api
{
    /// <summary>
    /// App entry point
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Creates a web host
        /// </summary>
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            if (!args.Contains("skipHost"))
                host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
