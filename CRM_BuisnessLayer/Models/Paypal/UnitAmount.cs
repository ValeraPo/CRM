using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class UnitAmount
    {
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
