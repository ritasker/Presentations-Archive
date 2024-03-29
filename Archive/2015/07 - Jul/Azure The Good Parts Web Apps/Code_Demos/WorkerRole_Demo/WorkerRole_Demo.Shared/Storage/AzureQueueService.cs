﻿using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using WorkerRole_Demo.Shared.Messages;

namespace WorkerRole_Demo.Shared.Storage
{
    public class AzureQueueService
    {
        private readonly CloudQueue _queue;

        public AzureQueueService(CloudQueueClient queueClient, string queueName)
        {
            _queue = queueClient.GetQueueReference(queueName);
            _queue.CreateIfNotExists();
        }

        public void AddMessage(ResizeImageMessage message)
        {
            var serializeMessage = JsonConvert.SerializeObject(message);
            _queue.AddMessage(new CloudQueueMessage(serializeMessage));
        }

        public CloudQueueMessage GetMessage()
        {
            return _queue.GetMessage();
        }

        public void DeleteMessage(CloudQueueMessage message)
        {
            _queue.DeleteMessage(message);
        }

        public bool MessagesAvailable()
        {
            return _queue.PeekMessage() != null;
        }
    }
}