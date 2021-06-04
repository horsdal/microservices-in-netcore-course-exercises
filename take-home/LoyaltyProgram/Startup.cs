using System;
using EasyNetQ;
using LoyaltyProgram.SpecialOffers;
using LoyaltyProgram.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LoyaltyProgram
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            services
                .AddSingleton(bus)
                .AddSingleton(bus.PubSub)
                .AddSingleton(new UserDb())
                .AddHostedService<SpecialOffersConsumer>()
                .AddMvcCore();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}

