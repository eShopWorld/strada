using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class GetEventMetadataPayloadBatchEventArgs : EventArgs
    {
        public GetEventMetadataPayloadBatchEventArgs(int numItemsReturned, int numEventsCached)
        {
            NumItemsReturned = numItemsReturned;
            NumEventsCached = numEventsCached;
        }

        public int NumItemsReturned { get; }

        public int NumEventsCached { get; }
    }
}