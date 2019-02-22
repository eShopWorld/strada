﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentScheduler;

namespace Eshopworld.Strada.Plugins.Streaming.PerfApp
{
    public class EventMetadataUploadJob : IJob
    {
        private readonly DataTransmissionClient _dataTransmissionClient;
        private readonly List<long> _elapsedTime;
        private readonly EventMetaCache _eventMetadataCache;
        private readonly Stopwatch _stopwatch;

        public EventMetadataUploadJob(
            DataTransmissionClient dataTransmissionClient,
            EventMetaCache eventMetadataCache,
            Stopwatch stopwatch,
            List<long> elapsedTime)
        {
            _dataTransmissionClient = dataTransmissionClient;
            _eventMetadataCache = eventMetadataCache;
            _stopwatch = stopwatch;
            _elapsedTime = elapsedTime;
        }

        public void Execute()
        {
            _stopwatch.Restart();

            var eventMetadataPayloadBatch =
                _eventMetadataCache.GetEventMetadataPayloadBatch();
            _dataTransmissionClient.TransmitAsync(eventMetadataPayloadBatch).Wait();

            _stopwatch.Stop();
            _elapsedTime.Add(_stopwatch.ElapsedMilliseconds); // todo: Send to StackDriver

            Console.Clear();
            Console.WriteLine("No. items in this batch: {0}", eventMetadataPayloadBatch.Count);
            Console.WriteLine("No. items remaining: {0}", _eventMetadataCache.NumItems);
            Console.WriteLine("Elapsed time this run: {0}", _stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Total elapsed time: {0}", _elapsedTime.Sum());
            Console.WriteLine("Avg elapsed time: {0}", _elapsedTime.Average());
            Console.WriteLine("Min elapsed time: {0}", _elapsedTime.Min());
            Console.WriteLine("Max elapsed time: {0}", _elapsedTime.Max());
            Console.WriteLine("No. runs: {0}", _elapsedTime.Count);
        }

        public void Stop(bool immediate)
        {
        }
    }
}