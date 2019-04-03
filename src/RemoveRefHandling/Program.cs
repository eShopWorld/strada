using System;
using System.IO;
using System.Linq;
using Eshopworld.Strada.Plugins.Streaming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RemoveRefHandling
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cloudServiceCredentials =
                JsonConvert.DeserializeObject<CloudServiceCredentials>(Resources.CloudServiceCredentials);

            var dataTransmissionClientConfigSettings =
                JsonConvert.DeserializeObject<DataTransmissionClientConfigSettings>(Resources
                    .DataTransmissionClientConfigSettings);

            DataTransmissionClient
                .Instance
                .InitAsync(cloudServiceCredentials, dataTransmissionClientConfigSettings)
                .Wait();

            DataTransmissionClient.Instance.TransmissionFailed += Instance_TransmissionFailed;
            EventMetaCache.Instance.AddEventMetaFailed += Instance_AddEventMetaFailed;
            EventMetaCache.Instance.GotEventMetadataPayloadBatch += Instance_GotEventMetadataPayloadBatch;

            DataUploader
                .Instance
                .StartAsync(
                    DataTransmissionClient.Instance,
                    EventMetaCache.Instance,
                    dataTransmissionClientConfigSettings)
                .Wait();

            var files = Directory.GetFiles(@"c:\users\pmooney\desktop\json\broken");

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);

                using (var jsonReader = new JsonTextReader(new StringReader(json)))
                {
                    jsonReader.SupportMultipleContent = true;

                    var jsonSerializer = new JsonSerializer();

                    while (jsonReader.Read())
                    {
                        dynamic d = jsonSerializer.Deserialize(jsonReader);
                        var fixedJson = FixJson(d.ToString());
                        var result = fixedJson.ToString();
                        EventMetaCache.Instance.Add(result);
                    }
                }
            }

            Console.ReadLine();
        }

        private static void Instance_GotEventMetadataPayloadBatch(object sender,
            GetEventMetadataPayloadBatchEventArgs e)
        {
        }

        private static void Instance_AddEventMetaFailed(object sender, AddEventMetaFailedEventArgs e)
        {
        }

        private static void Instance_TransmissionFailed(object sender, TransmissionFailedEventArgs e)
        {
        }

        private static string FixJson(string input)
        {
            var temp = JObject.Parse(input);
            temp.Descendants()
                .OfType<JProperty>()
                .Where(attr => attr.Name.StartsWith("$id"))
                .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                .ForEach(attr => attr.Remove()); // removing unwanted attributes

            temp.Descendants()
                .OfType<JProperty>()
                .Where(attr => attr.Name.StartsWith("MetadataItems"))
                .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                .ForEach(attr => attr.Remove()); // removing unwanted attributes

            temp.Descendants()
                .OfType<JProperty>()
                .Where(attr => attr.Name.StartsWith("$values"))
                .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                .ForEach(attr =>
                {
                    var root = (JObject) attr.Parent.Parent.Parent;
                    var parent = attr.Parent.Parent.ToObject<JProperty>();

                    var property = new JProperty(parent.Name, attr.Values());
                    root.Remove(parent.Name);

                    root.Add(property);
                });

            temp.Descendants()
                .OfType<JProperty>()
                .Where(attr => attr.Name.StartsWith("$values"))
                .ToList() // you should call ToList because you're about to changing the result, which is not possible if it is IEnumerable
                .ForEach(attr =>
                {
                    var root = (JObject) attr.Parent.Parent.Parent;
                    var parent = attr.Parent.Parent.ToObject<JProperty>();

                    var property = new JProperty(parent.Name, attr.Values());
                    root.Remove(parent.Name);

                    root.Add(property);
                });

            return temp.ToString();
        }
    }
}