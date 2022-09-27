using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Contracts.SpecialOffers;
using EasyNetQ;
using LoyaltyProgram;
using LoyaltyProgram.Users;
using LoyaltyProgramServiceTests.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Xunit;
using Xunit.Sdk;

namespace LoyaltyProgramTests
{
    public class SpecialOffersConsumer_should : IDisposable
    {
        private static readonly RetryPolicy ExponentialRetryPolicy =
            Policy
                .Handle<XunitException>()
                .WaitAndRetry(5, x => TimeSpan.FromMilliseconds(100 * Math.Pow(2, x)));

        private readonly MocksHost _serviceMock;
        private readonly IHost _host;
        private readonly IBus _bus;
        private readonly HttpClient _sut;

        public SpecialOffersConsumer_should()
        {
            _serviceMock = new MocksHost(6001);
            _host = new HostBuilder()
                .ConfigureWebHost(x => x
                    .ConfigureAppConfiguration(x => x.AddInMemoryCollection(new[] {new KeyValuePair<string, string>("notification_host",  "http://localhost:6001") } ))
                    .UseStartup<Startup>().UseTestServer())
                .Start();
            _bus = RabbitHutch.CreateBus("host=localhost");
            _sut = _host.GetTestClient();
        }


        private async Task RegisterNewUser()
        {
            var newuser = new LoyaltyProgramUser(
                Id: 0,
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
            await RegisterNewUser();
            await CreateSpecialOffer();
            AssertNotificationWasSent();
        }

        private void AssertNotificationWasSent()
        {
            ExponentialRetryPolicy.Execute(() =>
                Assert.True(NotificationsMock.ReceivedNotification));
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
            await (_serviceMock?.DisposeAsync() ?? ValueTask.CompletedTask);
            await _host?.StopAsync();
            _host?.Dispose();
            _sut?.Dispose();
            _bus?.Dispose();
        }
    }
}
