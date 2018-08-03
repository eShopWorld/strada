using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class TransmissionFailedEventArgs : EventArgs
    {        
        public TransmissionFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }
        
        public Exception Exception { get; private set; }
    }

}