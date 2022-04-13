using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal.Request
{
    public class SendInvoiceRequest
    {
        [JsonPropertyName("send_to_invoicer")]
        public bool? SendToInvoicer { get; set; }
        [JsonPropertyName("send_to_recipient")]
        public bool? SendToRecipient { get; set; }
    }
}
