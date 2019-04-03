using System;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
{
    public class Agent
    {
        private static readonly Lazy<Agent> Lazy =
            new Lazy<Agent>(() => new Agent());

        public static Agent Instance => Lazy.Value;

        public event EventMetaCache.EventMetaAddedEventHandler EventMetaAdded; // todo: add remaining events

        public void Start(
            CloudServiceCredentials cloudServiceCredentials,
            DataTransmissionClientConfigSettings dataTransmissionClientConfigSettings)
        {
            EventMetaCache.Instance.EventMetaAdded += EventMetaAdded;

            DataTransmissionClient.Instance.InitAsync(
                cloudServiceCredentials,
                dataTransmissionClientConfigSettings
            ).Wait();

            DataUploader.Instance.StartAsync(
                DataTransmissionClient.Instance,
                EventMetaCache.Instance,
                dataTransmissionClientConfigSettings).Wait();
        }
    }
}