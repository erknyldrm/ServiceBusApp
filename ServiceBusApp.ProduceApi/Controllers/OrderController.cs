using Microsoft.AspNetCore.Mvc;
using ServiceBusApp.Common;
using ServiceBusApp.Common.Dto;
using ServiceBusApp.Common.Events;
using ServiceBusApp.ProduceApi.Services;
using System;
using System.Threading.Tasks;

namespace ServiceBusApp.ProduceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AzureService AzureService;

        public OrderController(AzureService azureService)
        {
            AzureService = azureService;
        }

        [HttpPost]
        public async Task CreateOrder(OrderDto order)
        {
            //...

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                Id = order.Id,
                ProductName = order.ProductName,
                CreatedOn = DateTime.Now
            };

            //await AzureService.CreateQueueIfNoExist(Constants.OrderCreatedQueueName);
            //await AzureService.SendMessageToQueue(Constants.OrderCreatedQueueName, orderCreatedEvent);

            await AzureService.CreateTopicIfNoExist(Constants.OrderTopic);
            await AzureService.CreateSubscriptionIfNotExists(Constants.OrderTopic, Constants.OrderCreatedSubName, "OrderCreated", "OrderCreatedOnly");

            await AzureService.SendMessageToQueue(Constants.OrderTopic, orderCreatedEvent, "OrderCreated");
        }

        [HttpDelete("{id}")]
        public async Task DeleteOrder(int id)
        {
            //...

            var orderDeletedEvent = new OrderDeletedEvent()
            {
                Id = id,
                CreatedOn = DateTime.Now
            };

            //await AzureService.CreateQueueIfNoExist(Constants.OrderDeletedQueueName);
            //await AzureService.SendMessageToQueue(Constants.OrderCreatedQueueName, orderDeletedEvent);

            await AzureService.CreateTopicIfNoExist(Constants.OrderTopic);
            await AzureService.CreateSubscriptionIfNotExists(Constants.OrderTopic, Constants.OrderDeletedSubName, "OrderDeleted", "OrderDeletedOnly");

            await AzureService.SendMessageToTopic(Constants.OrderTopic, orderDeletedEvent,"OrderDeleted");
        }
    }
}
