using Newtonsoft.Json;

namespace Eshopworld.Strada.App
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
    }
}