﻿using System;
using Xunit;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Unit
{
    public class EventMetadataCacheTests
    {
        [Fact]
        public void AllEventMetadataPayloadsAreDequeued()
        {
            var eventMetadataCache = new EventMetadataCache();
            for (var i = 0; i < 100; i++) eventMetadataCache.Add("EVENTMETADATAPAYLOAD");

            var eventMetadataPayloadBatch = eventMetadataCache.GetEventMetadataPayloadBatch();
            Assert.Equal(100, eventMetadataPayloadBatch.Count);
        }

        [Fact]
        public void CantDequeueMoreThan1000EventsPerRun()
        {
            var eventMetadataCache = new EventMetadataCache();
            Assert.Throws<IndexOutOfRangeException>(() => eventMetadataCache.GetEventMetadataPayloadBatch(1001));
        }

        [Fact]
        public void Top50EventMetadataPayloadsAreDequeued()
        {
            var eventMetadataCache = new EventMetadataCache();
            for (var i = 0; i < 100; i++) eventMetadataCache.Add("EVENTMETADATAPAYLOAD");

            var eventMetadataPayloadBatch = eventMetadataCache.GetEventMetadataPayloadBatch(50);
            Assert.Equal(50, eventMetadataPayloadBatch.Count);
        }        
    }
}