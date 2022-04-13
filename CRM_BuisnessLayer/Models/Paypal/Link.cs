using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class Link
    {
        [JsonPropertyName("rel")]
        public string Rel { get; set; }
        [JsonPropertyName("href")]
        public string Href { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; }
    }
}
