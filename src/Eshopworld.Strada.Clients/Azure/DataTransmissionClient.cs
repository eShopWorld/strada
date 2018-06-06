using System;
using System.Text;
using Eshopworld.Strada.Clients.Core;
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
        private EventHubClient _eventHubClient;
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

        public void Connect()
        {

            var builder = new ServiceBusConnectionStringBuilder(_connectionString)
            {
                TransportType = TransportType.Amqp
            };

            var messagingFactory = MessagingFactory.CreateFromConnectionString(builder.ToString());

            _eventHubClient = messagingFactory.CreateEventHubClient(_eventHubPath);

            _eventHubClient.RetryPolicy = new RetryExponential(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(10), 3);
        }

        // Todo: Use ESW Brand package Enum.
        public void Transmit(object metadata, string brand)
        {
            // Todo: Error-handling (to App Insights).

            var serialisedPayload = JsonConvert.SerializeObject(metadata);
            var payloadSizeInKilobytes = serialisedPayload.GetSizeInKilobytes();

            var streamToSend = payloadSizeInKilobytes >= 256
                ? Encoding.UTF8.GetBytes(serialisedPayload).Compress()
                : Encoding.UTF8.GetBytes(serialisedPayload);

            if (_eventHubClient == null || _eventHubClient.IsClosed)
            {
                Init();
                Connect();
            }

            _eventHubClient.SendAsync(new EventData(streamToSend));
        }
    }
}