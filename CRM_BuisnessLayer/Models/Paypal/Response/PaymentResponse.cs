using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class PaymentResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("amount")]
        public UnitAmount Amount { get; set; }
        [JsonPropertyName("final_capture")]
        public bool FinalCapture { get; set; }
        [JsonPropertyName("seller_protection")]
        public SellerProtection SellerProtection { get; set; }
        [JsonPropertyName("seller_receivable_breakdown")]
        public SellerReceivableBreakdown SellerReceivableBreakdown { get; set; }
        [JsonPropertyName("invoice_id")]
        public string InvoiceId { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("supplementary_data")]
        public SupplementaryData SupplementaryData { get; set; }
        [JsonPropertyName("create_time")]
        public string CreateTime { get; set; }
        [JsonPropertyName("update_time")]
        public string UpdateTime { get; set; }
        [JsonPropertyName("links")]
        public List<Link> Links { get; set; }

    }
}
