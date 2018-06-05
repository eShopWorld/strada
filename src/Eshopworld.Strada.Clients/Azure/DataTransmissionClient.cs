using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Clients.Azure
{
    public class DataTransmissionClient
    {
        private static readonly Lazy<DataTransmissionClient> InnerDataTransmissionClient =
            new Lazy<DataTransmissionClient>(() => new DataTransmissionClient());

        private string _connectionString;
        private string _eventHubPath;

        public static DataTransmissionClient Instance => InnerDataTransmissionClient.Value;

        public void Init()
        {
            _connectionString = "";
            _eventHubPath = "";
        }

        public void Init(string connectionString, string eventHubPath)
        {
            _connectionString = connectionString;
            _eventHubPath = eventHubPath;
        }

        public void Transmit(object data)
        {
            // Todo: 2. Size
            // Todo: 3. Compress
            // Todo: 4. Send
            // Todo: 5. Error-handling (to App Insights).

            var serialised = JsonConvert.SerializeObject(data);

            var messagingFactory = MessagingFactory.CreateFromConnectionString(_connectionString);
            var eventHubClient = messagingFactory.CreateEventHubClient(_eventHubPath);

            eventHubClient.RetryPolicy = new RetryExponential(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(10), 3);

            var streamToSend = new byte[0];
            eventHubClient.SendAsync(new EventData(streamToSend));
        }
    }
}