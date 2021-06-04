using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;

namespace HelloMicroservices
{
    public class HelloController : Controller
    {
        private readonly IPubSub _bus;

        public HelloController(IPubSub bus)
        {
            _bus = bus;
        }

        [HttpGet("/")]
        public string SimpleGreet() => "Hello!";

        [HttpGet("/greetme")]
        public string GreetMeFrom([FromQuery] string name) => $"Helle {name}";

        [HttpGet("/greetme/{name}")]
        public string GreetMePath(string name) => $"Helle {name}";

        [HttpPost("/publish")]
        public Task PublishGreeting([FromBody] string name)
            => _bus.PublishAsync(new GreeterMessage(greeting: $"Hello {name}"));
    }

    public class GreeterMessage
    {
        public GreeterMessage(string greeting)
        {
            Greeting = greeting;
        }

        public string Greeting { get; private set; }
    }
}