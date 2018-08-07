using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    // todo: Document this and other items of interest.
    public class TransmissionFailedEventArgs : EventArgs
    {
        public TransmissionFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}