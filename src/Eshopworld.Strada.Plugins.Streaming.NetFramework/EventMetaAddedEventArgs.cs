using System;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
{
    public class EventMetaAddedEventArgs : EventArgs
    {
        public EventMetaAddedEventArgs(object eventMeta)
        {
            EventMeta = eventMeta;
        }

        public object EventMeta { get; }
    }
}