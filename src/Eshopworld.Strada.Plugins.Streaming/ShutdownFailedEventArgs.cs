using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <inheritdoc />
    /// <summary>
    ///     ShutdownFailedEventArgs encapsulates exceptions thrown
    ///     during <see cref="DataTransmissionClient" /> shutdown.
    /// </summary>
    public class ShutdownFailedEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="exception">The exception thrown during shutdown.</param>
        public ShutdownFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        ///     Exception is the exception thrown during shutdown.
        /// </summary>
        public Exception Exception { get; }
    }
}