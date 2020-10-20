using System;

namespace Contracts.SpecialOffers
{
    public class SpecialOfferCreated
    {
        public Guid EventId { get; set; }
        public DateTimeOffset EventCreated { get; set; }
        public SpecialOffer SpecialOffer { get; set; }
    }
}