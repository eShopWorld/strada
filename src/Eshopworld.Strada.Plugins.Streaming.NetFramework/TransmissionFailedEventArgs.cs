using System;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
{
    public class TransmissionFailedEventArgs : EventArgs
    {
        public TransmissionFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}