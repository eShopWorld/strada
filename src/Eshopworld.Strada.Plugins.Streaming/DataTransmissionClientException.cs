using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <inheritdoc />
    /// <summary>
    ///     DataTransmissionClientException wraps underlying Init/Shutdown exceptions to provide more intuitive exception
    ///     metadata.
    /// </summary>
    public class DataTransmissionClientException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="message">An intuitive error message.</param>
        /// <param name="innerException">The exception instance that caused the error.</param>
        public DataTransmissionClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}