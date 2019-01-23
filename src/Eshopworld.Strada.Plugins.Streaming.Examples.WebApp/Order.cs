using Newtonsoft.Json;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.WebApp
{
    /// <summary>
    ///     Order is a simulated ESW Order.
    /// </summary>
    public class Order
    {
        [JsonProperty("number")] public string Number { get; set; }

        [JsonProperty("value")] public decimal Value { get; set; }

        [JsonProperty("emailAddress")] public string EmailAddress { get; set; }

        [JsonProperty("complete")] public bool Complete { get; set; }

        [JsonProperty("country")] public string Country { get; set; }

        [JsonProperty("unitsPerOrder")] public int UnitsPerOrder { get; set; }
    }
}