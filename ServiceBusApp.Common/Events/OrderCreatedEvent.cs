namespace ServiceBusApp.Common.Events
{
    public class OrderCreatedEvent : EventBase
    {
        public string ProductName { get; set; }
    }
}
