using System;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
{
    public class AddEventMetaFailedEventArgs : EventArgs
    {
        public AddEventMetaFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}