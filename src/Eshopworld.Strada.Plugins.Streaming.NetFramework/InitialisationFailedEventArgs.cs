using System;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
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