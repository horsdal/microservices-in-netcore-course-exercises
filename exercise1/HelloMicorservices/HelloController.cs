using Microsoft.AspNetCore.Mvc;

namespace HelloMicorservices
{
    public class HelloController : Controller
    {
        [HttpGet("/")]
        public string SimpleGreet() => "Hello!";

        [HttpGet("/greetme")]
        public string GreetMeFrom([FromQuery] string name) => $"Hello {name}";

        [HttpGet("/greetme/{name}")]
        public string GreetMePath(string name) => $"Hello {name}";
       }
}