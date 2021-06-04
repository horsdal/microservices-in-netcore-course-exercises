using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.SpecialOffers;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.AspNetCore.Mvc;

namespace SpecialOffers
{
    [Route("special-offers")]
    public class SpecialOffersController : Controller
    {
        private static IList<SpecialOffer> _offers = new List<SpecialOffer>();
        private readonly IPubSub _bus;

        public SpecialOffersController(IPubSub bus)
        {
            _bus = bus;
        }

        [HttpPost("")]
        public Task CreateSpecialOffer([FromBody] SpecialOffer newOffer)
        {
            _offers.Add(newOffer);
            return _bus.PublishAsync(
                new SpecialOfferCreated
                {
                    SpecialOffer = newOffer,
                    EventId = Guid.NewGuid(),
                    EventCreated = DateTimeOffset.Now
                });
        }
    }
}