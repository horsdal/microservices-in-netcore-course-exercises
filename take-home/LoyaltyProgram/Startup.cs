using System;
using System.Threading.Tasks;
using EasyNetQ;
using LoyaltyProgram.SpecialOffers;
using LoyaltyProgram.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            app.UseMiddleware<RedirectingMiddleware>();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

    public class RedirectingMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext ctx)
        {
            if (ctx.Request.Path.Value.TrimEnd('/') == "/oldpath")
            {
                ctx.Response.Redirect("/newpath", permanent: true);
                return Task.CompletedTask;
            }

            return _next(ctx);
        }
    }
}

