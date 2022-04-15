using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class SellerProtection
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("dispute_categories")]
        public List<string> DisputeCategories { get; set; }
    }
}
