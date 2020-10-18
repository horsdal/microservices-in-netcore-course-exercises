using Microsoft.AspNetCore.Mvc;

namespace HelloMicorservices
{
    public class HelloController : Controller
    {
        [HttpGet("/")]
        public string SimpleGreet() => "Hello!";

        [HttpGet("/greetme")]
        public string GreetMeFrom([FromQuery] string name) => $"Helle {name}";

        [HttpGet("/greetme/{name}")]
        public string GreetMePath(string name) => $"Helle {name}";
       }
}