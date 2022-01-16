using System;

namespace ServiceBusApp.Common.Events
{
    public class EventBase
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
