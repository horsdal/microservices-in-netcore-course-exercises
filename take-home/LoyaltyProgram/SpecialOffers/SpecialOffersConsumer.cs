using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts.SpecialOffers;
using EasyNetQ;
using LoyaltyProgram.Users;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LoyaltyProgram.SpecialOffers
{
    public class SpecialOffersConsumer : IHostedService
    {
        private readonly IPubSub _bus;
        private readonly UserDb _db;
        private readonly ILogger<SpecialOffersConsumer> _logger;
        private ISubscriptionResult _subscription;
        private HttpClient _client;

        public SpecialOffersConsumer(UserDb db, IPubSub bus, ILogger<SpecialOffersConsumer> logger)
        {
            _db = db;
            _bus = bus;
            _logger = logger;
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5001/")
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _subscription = await
                _bus.SubscribeAsync<SpecialOfferCreated>(nameof(SpecialOffersConsumer),
                     async offer =>
                    {
                        _logger.LogInformation("Received offer: {@SpecialOfferCreated}", offer);
                        var notifications = offer.SpecialOffer.Tags
                            .SelectMany(w => _db.LookUpByTag(w))
                            .Select(u =>
                            {
                                _logger.LogInformation("Send notification to {id}", u.Id);
                                return _client.PostAsync(
                                    "/notifications",
                                    new StringContent(JsonConvert.SerializeObject(new
                                        {
                                            body = "Great Offer!", userId = u.Id.ToString()
                                        }),
                                        Encoding.UTF8,
                                        "application/json"));
                            });
                        var r = await Task.WhenAll(notifications);
                    });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription.Dispose();
            return Task.CompletedTask;
        }
    }
}