using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart
{
    public class EventStore
    {
        public void Raise(Event ev)
        {

        }

        public IEnumerable<Event> GetEvents(long start, long end)
        {
            return Enumerable.Empty<Event>();
        }
    }

    public class Event
    {
        public long SequenceNumber { get; set; }
        public object data;

    }
}
