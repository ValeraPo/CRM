using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class InvoiceDetail
    {
        [JsonPropertyName("invoice_number")]
        public string InvoiceNumber { get; set; }
        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; }
        [JsonPropertyName("payment_term")]
        public PaymentTerm PaymentTerm { get; set; }
    }
}
