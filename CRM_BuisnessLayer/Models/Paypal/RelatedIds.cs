using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class RelatedIds
    {
        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }
    }
}
