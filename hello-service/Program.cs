using System;
using Microsoft.AspNetCore.Hosting;

namespace hello_service
{
    class Program
    {
        static void Main(string[] args)
        {
            new WebHostBuilder()
            .UseKestrel()
            .UseStartup<Startup>()
            .Build()
            .Run();
        }
    }
}
