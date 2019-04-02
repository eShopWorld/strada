using System;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
{
    public class DataTransmittedEventArgs : EventArgs
    {
        public DataTransmittedEventArgs(int numItemsTransmitted)
        {
            NumItemsTransferred = numItemsTransmitted;
        }

        public int NumItemsTransferred { get; }
    }
}