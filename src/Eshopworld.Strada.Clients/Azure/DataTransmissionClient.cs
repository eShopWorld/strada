using System;
using System.Text;
using System.Threading.Tasks;
using Eshopworld.Strada.Clients.Core;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Clients.Azure
{
    /// <summary>
    ///     DataTransmissionClient is a static Event Hubs client, providing connectivity and transmission functionality.
    /// </summary>
    public class DataTransmissionClient
    {
        private static readonly Lazy<DataTransmissionClient> InnerDataTransmissionClient =
            new Lazy<DataTransmissionClient>(() => new DataTransmissionClient());

        private string _connectionString;
        private EventHubClient _eventHubClient;
        private string _eventHubPath;

        /// <summary>
        ///     Instance is a static instance of <see cref="DataTransmissionClient" />.
        /// </summary>
        public static DataTransmissionClient Instance => InnerDataTransmissionClient.Value;

        /// <summary>
        ///     Init instantiates Event Hub connectivity metadata.
        /// </summary>
        /// <param name="connectionString">The Event Hub connection-string.</param>
        /// <param name="eventHubPath">The Event Hub name, or URI path.</param>
        public void Init(string connectionString, string eventHubPath)
        {
            _connectionString = connectionString;
            _eventHubPath = eventHubPath;
        }

        /// <summary>
        ///     Connect establishes an AMQP connection to an Event Hub.
        /// </summary>
        public void Connect() // Todo: Add retry metadata as parameter.
        {
            var builder = new ServiceBusConnectionStringBuilder(_connectionString)
            {
                TransportType = TransportType.Amqp
            };

            var messagingFactory = MessagingFactory.CreateFromConnectionString(builder.ToString());

            _eventHubClient = messagingFactory.CreateEventHubClient(_eventHubPath);

            _eventHubClient.RetryPolicy = new RetryExponential(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(10), 3);
        }

        /// <summary>
        ///     Transmit persists <see cref="metadata" /> with associated <see cref="brand" /> metadata
        ///     to the connected Event Hub.
        /// </summary>
        /// <param name="metadata">The metadata to transmit to the Event Hub.</param>
        /// <param name="brand">The brand associated with <see cref="metadata" />.</param>
        public async Task Transmit(object metadata, string brand)
        {
            // Todo: Use ESW Brand package Enum.
            // Todo: Error-handling (to App Insights).
            // Todo: Introduce an error event so that apps can catch, but won't require try-catch

            var serialisedPayload = JsonConvert.SerializeObject(metadata);
            var payloadSizeInKilobytes = serialisedPayload.GetSizeInKilobytes();

            var streamToSend = payloadSizeInKilobytes >= 256
                ? Encoding.UTF8.GetBytes(serialisedPayload).Compress()
                : Encoding.UTF8.GetBytes(serialisedPayload);

            if (_eventHubClient == null || _eventHubClient.IsClosed) Connect();
            await _eventHubClient.SendAsync(new EventData(streamToSend));
        }
    }
}