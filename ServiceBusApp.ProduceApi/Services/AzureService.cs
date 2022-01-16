using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using ServiceBusApp.Common;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusApp.ProduceApi.Services
{
    public class AzureService
    {
        private readonly ManagementClient ManagementClient;

        public AzureService(ManagementClient managementClient)
        {
            ManagementClient = managementClient;
        }

        #region Queue Methods
        public async Task SendMessageToQueue(string queueName, object messageContent, string messageType = null)
        {
            IQueueClient client = new QueueClient(Constants.ConnectionString, queueName);

            await SendMessage(client, messageContent, messageType);
        }

        public async Task CreateQueueIfNoExist(string queueName)
        {
            if (!await ManagementClient.QueueExistsAsync(queueName))
                ManagementClient.CreateQueueAsync(queueName).Wait();
        }
        #endregion

        #region Topic Methods
        public async Task CreateTopicIfNoExist(string topicName)
        {
            if (!await ManagementClient.TopicExistsAsync(topicName))
                ManagementClient.CreateTopicAsync(topicName).Wait();
        }

        public async Task SendMessageToTopic(string topicName, object messageContent, string messageType = null)
        {
            ITopicClient client = new TopicClient(Constants.ConnectionString, topicName);

            await SendMessage(client, messageContent, messageType);
        }
        #endregion

        public async Task CreateSubscriptionIfNotExists(string topicName, string subscriptionName, string messageType = null, string ruleName = null)
        {
            if (!await ManagementClient.SubscriptionExistsAsync(topicName, subscriptionName))
                return;

            if (messageType != null)
            {
                SubscriptionDescription subscriptionDescription = new(topicName, subscriptionName);

                CorrelationFilter filter = new();
                filter.Properties["MessageType"] = messageType;

                RuleDescription ruleDescription = new(ruleName ?? messageType + "Rule", filter);

                await ManagementClient.CreateSubscriptionAsync(subscriptionDescription, ruleDescription);
            }
            else
                await ManagementClient.CreateSubscriptionAsync(topicName, subscriptionName);
        }

        private static async Task SendMessage(ISenderClient client, object messageContent, string messageType = null)
        {
            var byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageContent));

            var message = new Message(byteArray);
            message.UserProperties["MessageType"] = messageType;

            await client.SendAsync(message);
        }
    }
}
