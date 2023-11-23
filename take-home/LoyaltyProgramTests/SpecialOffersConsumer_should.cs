using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Contracts.SpecialOffers;
using EasyNetQ;
using InMemLogger;
using LoyaltyProgram;
using LoyaltyProgram.Users;
using LoyaltyProgramServiceTests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PactNet;
using PactNet.Exceptions;
using PactNet.Infrastructure.Outputters;
using PactNet.Matchers;
using PactNet.Output.Xunit;
using Polly;
using Polly.Retry;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LoyaltyProgramTests;

public class SpecialOffersConsumer_should : IDisposable
{
    private readonly ITestOutputHelper _output;

    private static readonly RetryPolicy ExponentialRetryPolicy =
        Policy
            .Handle<XunitException>()
            .WaitAndRetry(5, x => TimeSpan.FromMilliseconds(100 * Math.Pow(2, x)));

    private IPactBuilderV4 _pactBuilder;
    private IHost _host;
    private IBus _bus;
    private HttpClient _sut;

    public SpecialOffersConsumer_should(ITestOutputHelper output) => _output = output;

    private void StartSut(Uri noticationsServiceUri)
    {
        _bus = RabbitHutch.CreateBus("host=localhost");

        _host = new HostBuilder()
            .ConfigureWebHost(x => x
                .ConfigureAppConfiguration(x => x.AddInMemoryCollection(new[] {new KeyValuePair<string, string>("notification_host", noticationsServiceUri.ToString()) } ))
                .ConfigureLogging(builder => builder.AddInMemory())
                .UseStartup<Startup>().UseTestServer())
            .Start();
        _sut = _host.GetTestClient();
    }


    private async Task RegisterNewUser()
    {
        var newuser = new LoyaltyProgramUser(
            Id: 10,
            Name: "Foo",
            LoyaltyPoints: 42,
            Settings: new LoyaltyProgramSettings(new[] { "Cycling" }));
        var actual = await _sut.PostAsync(
            "/users",
            new StringContent(JsonConvert.SerializeObject(newuser), Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
        var actualUser = JsonConvert.DeserializeObject<LoyaltyProgramUser>(await actual.Content.ReadAsStringAsync());
        Assert.Equal(newuser.Name, actualUser.Name);
        Assert.Equal(newuser.LoyaltyPoints, actualUser.LoyaltyPoints);
    }

    [Fact]
    public async Task notify_eligible_user()
    {
        var pact = Pact.V4(nameof(notify_eligible_user), "Notifications", new PactConfig { PactDir = $"{Environment.CurrentDirectory}/../../../pacts", Outputters = new List<IOutput>(){ new XunitOutput(_output) } });
        _pactBuilder = pact.WithHttpInteractions();
        _pactBuilder
            .UponReceiving("Command to send notification")
                .WithRequest(HttpMethod.Post, "/notifications")
                .WithJsonBody(new { body = Match.Type("string"), userId = Match.Integer(0)})
            .WillRespond()
                .WithStatus(HttpStatusCode.OK);

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            StartSut(ctx.MockServerUri);
            var httpClient = new HttpClient();
            httpClient.BaseAddress = ctx.MockServerUri;
            await RegisterNewUser();
            await CreateSpecialOffer();
            AssertNotificationWasSent();
        });
    }

    private void AssertNotificationWasSent()
    {
        InMemoryLogger logs = _host.Services.GetService(typeof(InMemoryLogger)) as InMemoryLogger;
        ExponentialRetryPolicy.Execute(() =>
        {
            Assert.Contains(logs.RecordedInformationLogs, l => l.Message.Contains("Send notification"));
        });
    }

    private async Task CreateSpecialOffer()
    {
        var specialOffer = new SpecialOfferCreated
        {
            EventId = Guid.NewGuid(),
            EventCreated = DateTimeOffset.Now,
            SpecialOffer = new SpecialOffer
            {
                ShortDescription = "Lorum ipsum",
                FullDescription =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididun",
                Start = DateTimeOffset.Now.AddDays(1),
                End = DateTimeOffset.Now.AddDays(10),
                Tags = new[] { "Cycling", "Sports" }
            }
        };
        await _bus.PubSub.PublishAsync(specialOffer);
    }

    public async void Dispose()
    {
        await _host?.StopAsync();
        _host?.Dispose();
        _sut?.Dispose();
        _bus?.Dispose();
    }
}