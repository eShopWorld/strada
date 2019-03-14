using System;
using System.Collections.Generic;
using Xunit;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Unit
{
    public class EventMetadataCacheTests
    {
        [Fact]
        public void AllEventMetadataPayloadsAreDequeued()
        {
            var eventMetadataCache = new EventMetaCache();
            var httpHeaders = new Dictionary<string, string>
                {{"User-Agent", "USERAGENT"}, {"Content-Type", "CONTENT"}};

            for (var i = 0; i < 100; i++)
                eventMetadataCache.Add(
                    new SimpleObject {Name = "TEST"},
                    "BRAND",
                    "TEST",
                    Guid.NewGuid().ToString(),
                    "QUERY",
                    httpHeaders);

            var eventMetadataPayloadBatch = eventMetadataCache.GetEventMetadataPayloadBatch();
            Assert.Equal(100, eventMetadataPayloadBatch.Count);
        }

        [Fact]
        public void CantDequeueMoreThan1000EventsPerRun()
        {
            var eventMetadataCache = new EventMetaCache();
            Assert.Throws<IndexOutOfRangeException>(() => eventMetadataCache.GetEventMetadataPayloadBatch(1001));
        }

        [Fact]
        public void Top50EventMetadataPayloadsAreDequeued()
        {
            var eventMetadataCache = new EventMetaCache();
            var httpHeaders = new Dictionary<string, string>
                {{"User-Agent", "USERAGENT"}, {"Content-Type", "CONTENT"}};

            for (var i = 0; i < 100; i++)
                eventMetadataCache.Add(
                    new SimpleObject {Name = "TEST"},
                    "BRAND",
                    "TEST",
                    Guid.NewGuid().ToString(),
                    "QUERY",
                    httpHeaders);

            var eventMetadataPayloadBatch = eventMetadataCache.GetEventMetadataPayloadBatch(50);
            Assert.Equal(50, eventMetadataPayloadBatch.Count);
        }
    }
}