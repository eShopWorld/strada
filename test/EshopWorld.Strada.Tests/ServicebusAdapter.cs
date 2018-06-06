using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace EshopWorld.Strada.Tests
{
    /// <summary>
    ///     ServicebusAdapter provides thread-synchronised Azure Service Bus Topic-subscription functionality.
    /// </summary>
    internal class ServicebusAdapter
    {
        private readonly ManualResetEvent _manualResetEvent;
        private ISubscriptionClient _subscriptionClient;

        /// <summary>
        ///     ServicebusAdapter initialises a new <see cref="ServicebusAdapter" /> instance.
        /// </summary>
        /// <param name="manualResetEvent">
        ///     The <see cref="ManualResetEvent" /> that synchronises this instance' running process context
        ///     with that of calling processes - in this case, Unit Tests.
        /// </param>
        public ServicebusAdapter(ManualResetEvent manualResetEvent)
        {
            _manualResetEvent = manualResetEvent;
        }

        /// <summary>
        ///     MessageCount is the total number of messages returned from the Service Bus Topic.
        /// </summary>
        public int MessageCount { get; private set; }

        /// <summary>
        ///     Subscribe subscribes to an Azure Service Bus Topic in order to receive associated messages.
        /// </summary>
        public void Subscribe()
        {
            _subscriptionClient = new SubscriptionClient(
                Resources.ServiceBusConnectionString,
                Resources.TopicName,
                Resources.SubscriptionName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        /// <summary>
        ///     Unsubscribe unsubscribes from an Azure Service Bus Topic.
        /// </summary>
        /// <returns>An empty <see cref="Task" />.</returns>
        public async Task Unsubscribe()
        {
            if (_subscriptionClient != null && _subscriptionClient.IsClosedOrClosing)
                await _subscriptionClient.CloseAsync();
        }

        /// <summary>
        ///     ProcessMessagesAsync is executed for each message downloaded from an Azure Service Bus Topic.
        /// </summary>
        /// <param name="message">The inbound Azure Service Bus message.</param>
        /// <param name="token">The inbound Azure Service Bus message lock token.</param>
        /// <returns>An empty <see cref="Task" />.</returns>
        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            if (++MessageCount == 10) _manualResetEvent.Set();
        }

        /// <summary>
        ///     ExceptionReceivedHandler is executed when errors occur within the context of this instance.
        /// </summary>
        /// <param name="exceptionReceivedEventArgs"></param>
        /// <returns>An empty <see cref="Task" />.</returns>
        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }
    }
}