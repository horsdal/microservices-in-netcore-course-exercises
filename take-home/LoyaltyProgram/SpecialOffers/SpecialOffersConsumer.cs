using System.Threading;
using System.Threading.Tasks;
using Contracts.SpecialOffers;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LoyaltyProgram.SpecialOffers
{
    public class SpecialOffersConsumer : IHostedService
    {
        private readonly IPubSub _bus;
        private readonly ILogger<SpecialOffersConsumer> _logger;
        private ISubscriptionResult _subscription;

        public SpecialOffersConsumer(IPubSub bus, ILogger<SpecialOffersConsumer> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}