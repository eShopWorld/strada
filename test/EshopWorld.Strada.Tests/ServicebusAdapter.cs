using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace EshopWorld.Strada.Tests
{
    internal class ServicebusAdapter
    {
        private readonly ManualResetEvent _manualResetEvent;
        private ISubscriptionClient _subscriptionClient;

        public ServicebusAdapter(ManualResetEvent manualResetEvent)
        {
            _manualResetEvent = manualResetEvent;
        }

        public int MessageCount { get; set; }

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

        public async Task Unsubscribe()
        {
            if (_subscriptionClient != null && _subscriptionClient.IsClosedOrClosing)
                await _subscriptionClient.CloseAsync();
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            if (++MessageCount == 10) _manualResetEvent.Set();
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }
    }
}