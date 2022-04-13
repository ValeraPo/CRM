using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class Invoicer
    {
        [JsonPropertyName("email_address")]
        public string Email { get; set; }
        [JsonPropertyName("website")]
        public string Website { get; set; }
    }
}
