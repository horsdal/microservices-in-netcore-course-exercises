using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SpecialOffers
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var bus = new ServiceBusClient("Endpoint=sb://streamingbus.servicebus.windows.net/;SharedAccessKeyName=SpecialOfferMicroservice;SharedAccessKey=WfN3DSuxi5hip6Qah9pxMd9In69SHj7uEhwhWlLeQks=;EntityPath=specialoffercreated");
            services
                .AddSingleton(bus)
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
