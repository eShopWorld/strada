using System.Threading.Tasks;
using Eshopworld.Strada.Plugins.Streaming.NetCore;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.WebApp
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
        public string CorrelationId => _dataAnalyticsMeta.Fingerprint;

        /// <summary>
        ///     SaveOrder persists an <see cref="Order" /> instance to DB.
        /// </summary>
        /// <param name="order">The <see cref="Order" /> to persist to DB.</param>
        /// <param name="brandCode">The brand code to which the <see cref="Order" /> belongs. E.g., 'MAX'.</param>
        /// <param name="eventName">The name of the upstream event from which this HTTP request originated.</param>
        /// <param name="userAgent">The HTTP request User Agent header value.</param>
        /// <param name="queryString">The HTTP request Query string.</param>
        /// <returns><c>True</c> if the <see cref="Order" /> has been successfully persisted to DB.</returns>
        public async Task SaveOrder(
            Order order,
            string brandCode,
            string eventName,
            string userAgent,
            string queryString)
        {
            _orderRepository.Save(order);
            await _dataTransmissionClient.TransmitAsync(
                brandCode,
                eventName,
                _dataAnalyticsMeta.Fingerprint,
                order,
                userAgent,
                queryString);
        }
    }
}