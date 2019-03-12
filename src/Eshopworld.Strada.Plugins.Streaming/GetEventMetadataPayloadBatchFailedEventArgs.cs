using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class GetEventMetadataPayloadBatchFailedEventArgs : EventArgs
    {
        public GetEventMetadataPayloadBatchFailedEventArgs(Exception exception, int numEventsCached)
        {
            Exception = exception;
            NumEventsCached = numEventsCached;
        }

        public Exception Exception { get; }

        public int NumEventsCached { get; }
    }
}