using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <inheritdoc />
    /// <summary>
    ///     TransmissionFailedEventArgs provides metadata specific to a transmission failure.
    /// </summary>
    public class TransmissionFailedEventArgs : EventArgs
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="brandName">The customer name or reference code.</param>
        /// <param name="correlationId">Used to link related metadata in the downstream data lake.</param>
        /// <param name="exception">The <see cref="Exception" /> instance that describes the transmission failure.</param>
        public TransmissionFailedEventArgs(string brandName, string correlationId, Exception exception)
        {
            BrandName = brandName;
            CorrelationId = correlationId;
            Exception = exception;
        }

        /// <summary>
        ///     BrandName is the customer name or reference code.
        /// </summary>
        public string BrandName { get; }

        /// <summary>
        ///     CorrelationId is used to link related metadata in the downstream data lake.
        /// </summary>
        public string CorrelationId { get; }

        /// <summary>
        ///     Exception is the <see cref="Exception" /> instance that describes the transmission failure.
        /// </summary>
        public Exception Exception { get; }
    }
}