using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataTransmissionException : Exception
    {
        public DataTransmissionException(
            string message,
            string brandCode,
            string correlationId,
            Exception innerException) : base(message, innerException)
        {
            BrandCode = brandCode;
            CorrelationId = correlationId;
        }

        public DataTransmissionException(
            string message,
            Exception innerException) : base(message, innerException)
        {
        }

        public string BrandCode { get; }

        public string CorrelationId { get; }
    }
}