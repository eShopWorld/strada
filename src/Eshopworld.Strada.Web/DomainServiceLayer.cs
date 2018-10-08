using System.Data;
using System.Threading.Tasks;
using Eshopworld.Strada.Plugins.Streaming;

namespace Eshopworld.Strada.Web
{
    /// <summary>
    ///     DomainServiceLayer encapsulates common domain functions, such as persisting data to DB.
    /// </summary>
    public class DomainServiceLayer
    {
        private readonly DataAnalyticsMeta _dataAnalyticsMeta;
        private readonly DataTransmissionClient _dataTransmissionClient;
        private readonly OrderRepository _orderRepository;

        public DomainServiceLayer(
            OrderRepository orderRepository,
            DataAnalyticsMeta dataAnalyticsMeta,
            DataTransmissionClient dataTransmissionClient)
        {
            _orderRepository = orderRepository;
            _dataAnalyticsMeta = dataAnalyticsMeta;
            _dataTransmissionClient = dataTransmissionClient;
        }

        /// <summary>
        ///     Used to link related metadata in the downstream data lake.
        /// </summary>
        public string CorrelationId => _dataAnalyticsMeta.CorrelationId;

        /// <summary>
        ///     SaveOrder persists an <see cref="Order" /> instance to DB.
        /// </summary>
        /// <param name="order">The <see cref="Order" /> to persist to DB.</param>
        /// <param name="eventName">The name of the upstream event from which this HTTP request originated.</param>
        /// <returns><c>True</c> if the <see cref="Order" /> has been successfully persisted to DB.</returns>
        public async Task SaveOrder(Order order, string eventName)
        {
            var orderSaved = _orderRepository.Save(order);

            if (orderSaved)
                await _dataTransmissionClient.TransmitAsync(
                    "MAX",
                    eventName,
                    _dataAnalyticsMeta.CorrelationId,
                    order);
            else
                throw new DataException("Something went wrong while saving the order.");
        }
    }
}