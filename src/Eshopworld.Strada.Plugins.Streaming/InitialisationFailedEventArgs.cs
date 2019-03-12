using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class InitialisationFailedEventArgs : EventArgs
    {
        public InitialisationFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}