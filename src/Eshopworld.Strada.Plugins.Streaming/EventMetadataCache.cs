using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class EventMetadataCache
    {
        private ConcurrentQueue<string> _cache;

        public EventMetadataCache()
        {
            _cache = new ConcurrentQueue<string>();
        }

        public long NumItems => _cache.Count;

        public void Add(string eventMetadataPayload)
        {
            if (string.IsNullOrEmpty(eventMetadataPayload))
                throw new ArgumentNullException(nameof(eventMetadataPayload));

            if (_cache == null) _cache = new ConcurrentQueue<string>();
            _cache.Enqueue(eventMetadataPayload);
        }

        public List<string>
            GetEventMetadataPayloadBatch(
                int maxItemsToRemove = 1000)
        {
            if (maxItemsToRemove > 1000)
                throw new IndexOutOfRangeException(
                    $"Value {maxItemsToRemove} should be in range [1, 1000].");
            if (_cache == null || _cache.IsEmpty) return new List<string>();

            var eventMetadataPayloadBatch = new List<string>();
            var counter = 0;
            bool canDequeue;

            do
            {
                canDequeue = _cache.TryDequeue(out var eventMetadataPayload);
                if (canDequeue) eventMetadataPayloadBatch.Add(eventMetadataPayload);
                counter++;
            } while (counter < maxItemsToRemove && canDequeue);

            return eventMetadataPayloadBatch;
        }
    }
}