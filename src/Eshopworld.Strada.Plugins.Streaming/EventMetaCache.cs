using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class EventMetaCache
    {
        private static readonly Lazy<EventMetaCache> Lazy =
            new Lazy<EventMetaCache>(() => new EventMetaCache());

        private ConcurrentQueue<string> _cache;

        public EventMetaCache()
        {
            _cache = new ConcurrentQueue<string>();
        }

        public long NumItems => _cache.Count;

        public static EventMetaCache Instance => Lazy.Value;

        public void Add<T>(T eventMetadataPayload,
            string brandCode = null,
            string eventName = null,
            string fingerprint = null,
            string userAgent = null,
            string queryString = null)
        {
            if (eventMetadataPayload == null)
                throw new ArgumentNullException(nameof(eventMetadataPayload));
            if (_cache == null) _cache = new ConcurrentQueue<string>();

            var serialisedEventMetadataPayload = JsonConvert.SerializeObject(eventMetadataPayload);
            var eventTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var cachePayload = Functions.AddTrackingMetadataToJson(
                serialisedEventMetadataPayload,
                brandCode,
                eventName,
                fingerprint,
                userAgent,
                queryString,
                eventTimestamp.ToString()
            );

            _cache.Enqueue(cachePayload);
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