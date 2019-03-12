using System;

namespace Eshopworld.Strada.Plugins.Streaming
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