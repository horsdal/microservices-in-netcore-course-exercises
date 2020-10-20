using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Notifications;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Notificatons
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvcCore();
            services.AddSwaggerGen();
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

    [Route("/notifications")]
    public class NotificationsController : Controller
    {
        private static readonly IList<SendNotification> _notifications = new List<SendNotification>();

        [HttpPost("")]
        public void SendNotification([FromBody] SendNotification notification) => _notifications.Add(notification);

        [HttpGet("")]
        public SendNotification[] GetAll() => _notifications.ToArray();
    }
}
