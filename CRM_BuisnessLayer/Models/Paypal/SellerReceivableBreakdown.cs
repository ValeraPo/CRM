using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class SellerReceivableBreakdown
    {
        [JsonPropertyName("gross_amount")]
        public UnitAmount GrossAmount { get; set; }
        [JsonPropertyName("paypal_fee")]
        public UnitAmount PaypalFee { get; set; }
        [JsonPropertyName("net_amount")]
        public UnitAmount NetAmount { get; set; }
    }
}
