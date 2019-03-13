using System;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataUploaderStartFailedEventArgs : EventArgs
    {
        public DataUploaderStartFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}