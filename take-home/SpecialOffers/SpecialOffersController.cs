using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Contracts.SpecialOffers;
using Microsoft.AspNetCore.Mvc;

namespace SpecialOffers
{
    [Route("special-offers")]
    public class SpecialOffersController : Controller
    {
        private static IList<SpecialOffer> _offers = new List<SpecialOffer>();
        private readonly ServiceBusSender _busMessageSender;

        public SpecialOffersController(ServiceBusClient bus)
        {
            _busMessageSender = bus.CreateSender(nameof(SpecialOfferCreated));
        }

        [HttpPost("")]
        public Task CreateSpecialOffer([FromBody] SpecialOffer newOffer)
        {
            _offers.Add(newOffer);
            var @event = new SpecialOfferCreated
            {
                EventCreated = DateTimeOffset.Now,
                EventId = Guid.NewGuid(),
                SpecialOffer = newOffer
            };
            return _busMessageSender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(@event)));
        }
    }
}