using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataTransmittedEventArgs : EventArgs
    {
        public DataTransmittedEventArgs(int numItemsTransferred)
        {
            NumItemsTransferred = numItemsTransferred;
        }

        public int NumItemsTransferred { get; }
    }
}