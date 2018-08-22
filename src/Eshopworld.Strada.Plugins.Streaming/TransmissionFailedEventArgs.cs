using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <inheritdoc />
    /// <summary>
    ///     TransmissionFailedEventArgs encapsulates exceptions thrown during data-transmission.
    /// </summary>
    public class TransmissionFailedEventArgs : EventArgs
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="exception">The exception thrown during data-transmission.</param>
        public TransmissionFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        ///     Exception is the exception thrown during data-transmission.
        /// </summary>
        public Exception Exception { get; }
    }
}