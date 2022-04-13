using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class Discount
    {
        [JsonPropertyName("invoice_discount")]
        public InvoiceDiscount InvoiceDiscount { get; set; }
        [JsonPropertyName("item_discount")]
        public UnitAmount ItemDiscount { get; set; }
    }
}
