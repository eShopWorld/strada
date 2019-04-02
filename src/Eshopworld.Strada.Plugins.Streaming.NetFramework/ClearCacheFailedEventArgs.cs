using System;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
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