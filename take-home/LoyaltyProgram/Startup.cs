using System;
using System.Threading.Tasks;
using EasyNetQ;
using LoyaltyProgram.SpecialOffers;
using LoyaltyProgram.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoyaltyProgram;

public class Startup
{
    private readonly IConfiguration _config;

    public Startup(IConfiguration config)
    {
        _config = config;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        var notificationHost = _config["notification_host"] ?? "http://localhost:5001/";

        var bus = RabbitHutch.CreateBus("host=localhost");
        services
            .AddSingleton(bus)
            .AddSingleton(bus.PubSub)
            .AddSingleton(new UserDb())
            .AddHostedService<SpecialOffersConsumer>()
            .AddHttpClient("NotificationsClient", httpClient => httpClient.BaseAddress = new Uri(notificationHost)).Services
            .AddControllers().Services
            .AddMvcCore().Services
            .AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseSwagger()
            .UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "Notifications v1");
                x.RoutePrefix = string.Empty;
            })
            .UseRouting()
            .UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}