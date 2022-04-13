using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class InvoiceRequest
    {
        [JsonPropertyName("detail")]
        public InvoiceDetail InvoiceDetail { get; set; }
        [JsonPropertyName("invoicer")]
        public Invoicer Invoicer { get; set; }
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }
    }
}
