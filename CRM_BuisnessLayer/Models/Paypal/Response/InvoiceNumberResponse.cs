using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class InvoiceNumberResponse
    {
        [JsonPropertyName("invoice_number")]
        public string InvoiceNumber { get; set; }
    }
}
