using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class InvoiceDiscount
    {
        [JsonPropertyName("amount")]
        public UnitAmount Amount { get; set; }
    }
}
