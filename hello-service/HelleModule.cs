using Nancy;

namespace hello_service
{
    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get("/", _ => "Hello");
        }
    }
}