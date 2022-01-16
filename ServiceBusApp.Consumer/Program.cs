using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using ServiceBusApp.Common;
using ServiceBusApp.Common.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusApp.Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsumeSub<OrderCreatedEvent>(Constants.OrderTopic, Constants.OrderCreatedSubName, i =>
            {
                Console.WriteLine($"OrderCreatedEvent ReceivedMessage with:{i.Id}, Name:{i.ProductName}");
            }).Wait();


            ConsumeSub<OrderDeletedEvent>(Constants.OrderTopic, Constants.OrderDeletedSubName, i =>
            {
                Console.WriteLine($"OrderCreatedEvent ReceivedMessage with:{i.Id}");
            }).Wait();

            Console.ReadLine();
          
        }

        private static async Task ConsumeSub<T>(string topicName,string subName, Action<T> receivedAction)
        {
            // IQueueClient client = new QueueClient(Constants.ConnectionString, queueName);
            ISubscriptionClient client = new SubscriptionClient(Constants.ConnectionString, topicName, subName);

            client.RegisterMessageHandler(async (message, ct) => 
            {
                var model = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));

                receivedAction(model);

                await Task.CompletedTask;

            }, new MessageHandlerOptions(i => Task.CompletedTask));

            Console.WriteLine($"{typeof(T).Name} is listening...");
        }
    }
}
