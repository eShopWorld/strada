using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class ClearCacheFailedEventArgs : EventArgs
    {
        public ClearCacheFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}