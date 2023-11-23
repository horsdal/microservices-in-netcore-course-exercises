using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Json;

namespace LoyaltyProgram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, logger) =>
                {
                    logger
                        .Enrich.FromLogContext()
                        .Enrich.WithSpan();
                    if (context.HostingEnvironment.IsDevelopment())
                        logger.WriteTo.ColoredConsole(
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {TraceId} {Level:u3} {Message}{NewLine}{Exception}");
                    else
                        logger.WriteTo.Console(new JsonFormatter());
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5002");
                });
    }
}