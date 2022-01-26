using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
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
            var bus = new ServiceBusClient("Endpoint=sb://streamingbus.servicebus.windows.net/;SharedAccessKeyName=LoyaltyProgrmaMicroservice;SharedAccessKey=pte9MidNO9jrd64D+w0/XOqRcXz6igThB0qD+jLqANg=;EntityPath=specialoffercreated");
            services
                .AddSingleton(bus)
                .AddSingleton(new UserDb())
                .AddHostedService<SpecialOffersConsumer>()
                .AddMvcCore();
            services.AddControllers();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<RedirectingMiddleware>()
                .UseSwagger()
                .UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "Special Offer v1");
                    x.RoutePrefix = string.Empty;
                })
                .UseRouting()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });
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

