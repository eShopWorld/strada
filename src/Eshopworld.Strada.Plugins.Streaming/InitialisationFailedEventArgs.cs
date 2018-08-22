using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <inheritdoc />
    /// <summary>
    ///     InitialisationFailedEventArgs encapsulates exceptions thrown
    ///     during <see cref="DataTransmissionClient" /> initialisation.
    /// </summary>
    public class InitialisationFailedEventArgs : EventArgs
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="exception">The exception thrown during initialisation.</param>
        public InitialisationFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        ///     Exception is the exception thrown during initialisation.
        /// </summary>
        public Exception Exception { get; }
    }
}