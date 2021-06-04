using System;

namespace Contracts.SpecialOffers
{
    public class SpecialOffer
    {
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }

        public string[] Tags { get; set; }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}