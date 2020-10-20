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
            services
                .AddSingleton(RabbitHutch.CreateBus("host=localhost"))
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

