using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Contracts.SpecialOffers;
using LoyaltyProgram.Users;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LoyaltyProgram.SpecialOffers
{
    public class SpecialOffersConsumer : IHostedService
    {
        private readonly ServiceBusClient _bus;
        private readonly UserDb _db;
        private readonly ILogger<SpecialOffersConsumer> _logger;
        private ServiceBusProcessor _messageProcessor;
        private readonly HttpClient _client;

        public SpecialOffersConsumer(UserDb db, ServiceBusClient bus, ILogger<SpecialOffersConsumer> logger)
        {
            _db = db;
            _bus = bus;
            _logger = logger;
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5001/")
            };
        }

        public async Task StartAsync(CancellationToken ct)
        {
            _messageProcessor = _bus.CreateProcessor(nameof(SpecialOfferCreated), "LoyaltyProgramConsumer");
            _messageProcessor.ProcessMessageAsync += args =>
            {
                _logger.LogInformation("Received event: {@event}", args.Message);
                return HandleSpecialOfferCreated(JsonSerializer.Deserialize<SpecialOfferCreated>(args.Message.Body));
            };
            _messageProcessor.ProcessErrorAsync += args =>
            {
                _logger.LogError(args.Exception, "Event processing failed", args);
                return Task.CompletedTask;
            };
            await _messageProcessor.StartProcessingAsync(ct);
        }

        private async Task HandleSpecialOfferCreated(SpecialOfferCreated offer)
        {
            _logger.LogInformation("Received offer: {@SpecialOfferCreated}", offer);
            var notifications = offer.SpecialOffer.Tags
                .SelectMany(w => _db.LookUpByTag(w))
                .Select(u =>
                {
                    _logger.LogInformation("Send notification to {id}", u.Id);
                    return _client.PostAsync("/notifications",
                        new StringContent(
                            JsonConvert.SerializeObject(new { body = "Great Offer!", userId = u.Id.ToString() }),
                            Encoding.UTF8, "application/json"));
                });
            var r = await Task.WhenAll(notifications);
        }

        public async Task StopAsync(CancellationToken ct)
        {
            await _messageProcessor.StopProcessingAsync(ct);
            await _messageProcessor.DisposeAsync();
        }
    }
}