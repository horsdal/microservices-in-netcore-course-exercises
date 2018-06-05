using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace hello_service
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin().UseNancy();
        }
    }
}