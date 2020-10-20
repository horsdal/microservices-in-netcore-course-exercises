using EasyNetQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SpecialOffers
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
// Use this line when running outside docker:                .AddSingleton(RabbitHutch.CreateBus("host=localhost"))
                .AddSingleton(RabbitHutch.CreateBus("host=rabbitmq"))
                .AddMvcCore();
            services.AddControllers();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
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
}
