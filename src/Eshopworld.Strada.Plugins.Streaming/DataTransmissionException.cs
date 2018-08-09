using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <inheritdoc />
    /// <summary>
    ///     DataTransmissionException wraps underlying data transmission exceptions to provide more intuitive exception
    ///     metadata.
    /// </summary>
    public class DataTransmissionException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="message">An intuitive error message.</param>
        /// <param name="brandName">The customer name or reference code.</param>
        /// <param name="correlationId">Used to link related metadata in the downstream data lake.</param>
        /// <param name="innerException">The exception instance that caused the error.</param>
        public DataTransmissionException(
            string message,
            string brandName,
            string correlationId,
            Exception innerException) : base(message, innerException)
        {
            BrandName = brandName;
            CorrelationId = correlationId;
        }

        /// <summary>
        ///     BrandName is the customer name or reference code.
        /// </summary>
        public string BrandName { get; }

        /// <summary>
        ///     CorrelationId is used to link related metadata in the downstream data lake.
        /// </summary>
        public string CorrelationId { get; }
    }
}