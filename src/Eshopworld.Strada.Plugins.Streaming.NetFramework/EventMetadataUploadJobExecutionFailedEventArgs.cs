using System;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
{
    public class EventMetadataUploadJobExecutionFailedEventArgs : EventArgs
    {
        public EventMetadataUploadJobExecutionFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}