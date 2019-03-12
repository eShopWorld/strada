using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataTransmissionClientException : Exception
    {
        public DataTransmissionClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}