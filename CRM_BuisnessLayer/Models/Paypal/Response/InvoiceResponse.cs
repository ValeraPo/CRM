using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class InvoiceResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("detail")]
        public InvoiceDetailResponse InvoiceDetail { get; set; }
        [JsonPropertyName("invoicer")]
        public Invoicer Invoicer { get; set; }

        [JsonPropertyName("amount")]
        public UnitAmountWithBreakdown Amount { get; set; }
        [JsonPropertyName("due_amount")]
        public UnitAmount DueAmount { get; set; }
        [JsonPropertyName("links")]
        public List<Link> Links { get; set; }
        [JsonPropertyName("unilateral")]
        public bool? Unilateral { get; set; }
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }

        [JsonPropertyName("configuration")]
        public Configuration Configuration { get; set; }
        [JsonPropertyName("payments")]
        public InvoicePayments Payments { get; set; }
    }
}
