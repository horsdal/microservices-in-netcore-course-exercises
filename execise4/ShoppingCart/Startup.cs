using System.Xml.Linq;
using Marten;
using Microsoft.AspNetCore.Builder;
using Nancy;
using Nancy.Configuration;
using Nancy.Owin;
using Nancy.TinyIoc;
using StatsdClient;

namespace ShoppingCart
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            Metrics.Configure(new MetricsConfig
            {
                StatsdServerName = "localhost",
                StatsdServerPort = 8125,
                Prefix = "ShoppingCart"
            });

            app.Use(next => async ctx =>     
            {
                using (Metrics.StartTimer(ctx.Request.Method))
                {
                    await next(ctx);
                }
            });
            app.UseOwin().UseNancy();
        }
    }

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            var documentStore = DocumentStore.For(x =>
            {
                x.Connection("host=localhost;database=shoppingcart;password=mysecretpassword;username=postgres");
            });
            container.Register<IDocumentStore>(documentStore);
            base.ConfigureApplicationContainer(container);
        }

        public override void Configure(INancyEnvironment environment)
        {
            environment.Tracing(enabled: false, displayErrorTraces: true);
            base.Configure(environment);
        }
    }
}