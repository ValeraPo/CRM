using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class SupplementaryData
    {
        [JsonPropertyName("related_ids")]
        public RelatedIds RelatedIds { get; set; }

    }
}
