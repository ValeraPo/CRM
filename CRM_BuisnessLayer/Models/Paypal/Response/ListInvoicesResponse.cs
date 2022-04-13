using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class ListInvoicesResponse
    {

        [JsonPropertyName("items")]
        public List<InvoiceResponse> Items { get; set; }
        [JsonPropertyName("links")]
        public List<Link> Links { get; set; }
    }
}
