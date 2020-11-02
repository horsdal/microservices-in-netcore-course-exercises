using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiGatewayMock;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using static System.Console;
using static System.Environment;

IAsyncPolicy<HttpResponseMessage> ExponentialRetryPolicy =
    Policy<HttpResponseMessage>
        .Handle<HttpRequestException>()
        .OrTransientHttpStatusCode()
        .WaitAndRetryAsync(
            3,
            attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));

IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy =
    Policy<HttpResponseMessage>
        .Handle<HttpRequestException>()
        .OrTransientHttpStatusCode()
        .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));


var host = args.Length > 0 ? args[0] : "http://localhost:6000";

var services = new ServiceCollection();
services.AddHttpClient<LoyaltyProgramClient>()
    .AddPolicyHandler(request =>
        request.Method == HttpMethod.Get
            ? CircuitBreakerPolicy
            : ExponentialRetryPolicy)
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(host));
var serviceProvider = services.BuildServiceProvider();

var client = serviceProvider.GetService<LoyaltyProgramClient>() ?? throw new NullReferenceException($"Must register {nameof(LoyaltyProgramClient)} in service collection");
var processCommand =
    new Dictionary<char, (string description, Func<string, Task<(bool, HttpResponseMessage)>> handler)>
    {
        {
            'r',
            ("r <user name> - to register a user with name <user name> with the Loyalty Program Microservice.",
                async c => (true, await client.RegisterUser(c.Substring(1))))
        },
        {
            'q',
            ("q <userid> - to query the Loyalty Program Microservice for a user with id <userid>.",
                async c => (true, await client.QueryUser(c.Substring(1))))
        },
        {
            'u',
            ("u <userid> <interests> - to update a user with new interests",
                HandleUpdateInterestsCommand)
        },
        {
            'x',
            ("x - to exit",
                _ => Task.FromResult((false, new HttpResponseMessage(0))))
        },
    };

WriteLine("Welcome to the API Gateway Mock.");

var cont = true;
while (cont)
{
    WriteLine();
    WriteLine();
    WriteLine("********************");
    WriteLine("Choose one of:");
    foreach (var c in processCommand.Values)
        WriteLine(c.description);
    WriteLine("********************");
    var cmd = ReadLine() ?? "";
    if (processCommand.TryGetValue(cmd[0], out var command))
    {
        var (@continue, response) = await command.handler(cmd);
        await PrettyPrint(response);
        cont = @continue;
    }
}

async Task<(bool, HttpResponseMessage)> HandleUpdateInterestsCommand(string cmd)
{
    var response = await client.QueryUser(cmd.Split(' ').Skip(1).First());
    if (!response.IsSuccessStatusCode)
        return (true, response);

    var user = JsonSerializer.Deserialize<LoyaltyProgramUser>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    var newInterests = cmd.Substring(cmd.IndexOf(' ', 2)).Split(',').Select(i => i.Trim());
    var res = await client.UpdateUser(new
    {
        user.Name,
        user.Id,
        user.LoyaltyPoints,
        settings = new {interests = user.Settings.Interests.Concat(newInterests).ToArray()}
    });
    return (true, res);

}


static async Task PrettyPrint(HttpResponseMessage response)
{
    if (response.StatusCode == 0) return;
    WriteLine("********** Response **********");
    WriteLine($"status code: {response.StatusCode}");
    WriteLine("Headers: " + response.Headers.Aggregate("",
        (acc, h) => $"{acc}{NewLine}\t{h.Key}: {h.Value.Aggregate((hAcc, hVal)=> $"{hAcc}{hVal}, ")}") ?? "");
    WriteLine($"Body:{NewLine}{JsonSerializer.Serialize(JsonDocument.Parse(await response.Content.ReadAsStringAsync()), new JsonSerializerOptions { WriteIndented = true })}");
}

public record LoyaltyProgramSettings(string[] Interests);

public record LoyaltyProgramUser(int Id, string Name, int LoyaltyPoints, LoyaltyProgramSettings Settings);