﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class EventMetaCache
    {
        public delegate void AddEventMetaFailedEventHandler(object sender, AddEventMetaFailedEventArgs e);

        public delegate void ClearCacheFailedEventHandler(object sender, ClearCacheFailedEventArgs e);

        public delegate void EventMetaAddedEventHandler(object sender, EventMetaAddedEventArgs e);

        public delegate void GetEventMetadataPayloadBatchEventHandler(
            object sender,
            GetEventMetadataPayloadBatchEventArgs e);

        public delegate void GetEventMetadataPayloadBatchFailedEventHandler(
            object sender,
            GetEventMetadataPayloadBatchFailedEventArgs e);

        private static readonly Lazy<EventMetaCache> Lazy =
            new Lazy<EventMetaCache>(() => new EventMetaCache());

        private ConcurrentQueue<string> _cache;

        public EventMetaCache()
        {
            _cache = new ConcurrentQueue<string>();
        }

        public long NumItems => _cache.Count;

        public static EventMetaCache Instance => Lazy.Value;

        public event EventMetaAddedEventHandler EventMetaAdded;
        public event AddEventMetaFailedEventHandler AddEventMetaFailed;
        public event GetEventMetadataPayloadBatchFailedEventHandler GetEventMetadataPayloadBatchFailed;
        public event GetEventMetadataPayloadBatchEventHandler GotEventMetadataPayloadBatch;
        public event ClearCacheFailedEventHandler ClearCacheFailed;

        public void Add<T>(T eventMetadataPayload,
            string brandCode,
            string eventName,
            string fingerprint,
            string queryString,
            Dictionary<string, string> httpHeaders)
        {
            if (eventMetadataPayload == null)
                throw new ArgumentNullException(nameof(eventMetadataPayload));
            if (string.IsNullOrEmpty(brandCode)) throw new ArgumentNullException(nameof(brandCode));
            if (string.IsNullOrEmpty(eventName)) throw new ArgumentNullException(nameof(eventName));
            if (string.IsNullOrEmpty(fingerprint)) throw new ArgumentNullException(nameof(fingerprint));
            if (string.IsNullOrEmpty(queryString)) throw new ArgumentNullException(nameof(queryString));
            if (httpHeaders == null)
                throw new ArgumentNullException(nameof(httpHeaders));

            if (_cache == null) _cache = new ConcurrentQueue<string>();

            try
            {
                var serialisedEventMetadataPayload = JsonConvert.SerializeObject(eventMetadataPayload);
                var eventTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                var cachePayload = Functions.AddTrackingMetadataToJson(
                    serialisedEventMetadataPayload,
                    brandCode,
                    eventName,
                    fingerprint,
                    queryString,
                    httpHeaders,
                    eventTimestamp.ToString()
                );

                _cache.Enqueue(cachePayload);

                OnEventMetaAdded(new EventMetaAddedEventArgs(cachePayload));
            }
            catch (Exception exception)
            {
                const string errorMessage = "An error occurred while adding event-meta to cache.";
                OnAddEventMetaFailed(new AddEventMetaFailedEventArgs(new Exception(errorMessage, exception)));
            }
        }

        public List<string> GetEventMetadataPayloadBatch(
            int maxItemsToRemove = 1000)
        {
            if (maxItemsToRemove > 1000)
                throw new IndexOutOfRangeException(
                    $"Value {maxItemsToRemove} should be in range [1, 1000].");
            if (_cache == null || _cache.IsEmpty) return new List<string>();

            var eventMetadataPayloadBatch = new List<string>();
            var counter = 0;

            try
            {
                bool canDequeue;
                do
                {
                    canDequeue = _cache.TryDequeue(out var eventMetadataPayload);
                    if (canDequeue) eventMetadataPayloadBatch.Add(eventMetadataPayload);
                    counter++;
                } while (counter < maxItemsToRemove && canDequeue);

                OnGotEventMetadataPayloadBatch(new GetEventMetadataPayloadBatchEventArgs(
                    eventMetadataPayloadBatch.Count,
                    _cache.Count));
                return eventMetadataPayloadBatch;
            }
            catch (Exception exception)
            {
                const string errorMessage = "An error occurred while getting the event-meta payload batch.";
                OnGetEventMetadataPayloadBatchFailed(new GetEventMetadataPayloadBatchFailedEventArgs(
                    new Exception(errorMessage, exception), _cache.Count));
            }

            return null;
        }

        public void Clear()
        {
            if (_cache == null) return;

            try
            {
                bool canDequeue;
                do
                {
                    canDequeue = _cache.TryDequeue(out _);
                } while (canDequeue);
            }
            catch (Exception exception)
            {
                const string errorMessage = "An error occurred while clearing the cache.";
                OnClearCacheFailed(new ClearCacheFailedEventArgs(new Exception(errorMessage, exception)));
            }
        } // todo: Clear cache if it becomes too big

        protected virtual void OnEventMetaAdded(EventMetaAddedEventArgs e)
        {
            EventMetaAdded?.Invoke(this, e);
        }

        protected virtual void OnAddEventMetaFailed(AddEventMetaFailedEventArgs e)
        {
            AddEventMetaFailed?.Invoke(this, e);
        }

        protected virtual void OnGetEventMetadataPayloadBatchFailed(GetEventMetadataPayloadBatchFailedEventArgs e)
        {
            GetEventMetadataPayloadBatchFailed?.Invoke(this, e);
        }

        protected virtual void OnGotEventMetadataPayloadBatch(GetEventMetadataPayloadBatchEventArgs e)
        {
            GotEventMetadataPayloadBatch?.Invoke(this, e);
        }

        protected virtual void OnClearCacheFailed(ClearCacheFailedEventArgs e)
        {
            ClearCacheFailed?.Invoke(this, e);
        }
    }
}