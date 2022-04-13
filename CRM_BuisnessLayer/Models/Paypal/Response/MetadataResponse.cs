using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class MetadataResponse
    {
        [JsonPropertyName("create_time")]
        public string CreateTime { get; set; }
        [JsonPropertyName("last_update_time")]
        public string LatUpdateTime { get; set; }
        [JsonPropertyName("created_by_flow")]
        public string CreatedByFlow { get; set; }
        [JsonPropertyName("recipient_view_url")]
        public string RecipientViewUrl { get; set; }
        [JsonPropertyName("invoicer_view_url")]
        public string InvoicerViewUrl { get; set; }
        [JsonPropertyName("caller_type")]
        public string CallerType { get; set; }
    }
}
