using System;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace ShoppingCart
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                });
            Log.Logger = loggerConfig.CreateLogger();

            Log.Information("Starting web host");
            new WebHostBuilder()
                .UseSerilog()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
