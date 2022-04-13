using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class Breakdown
    {
        [JsonPropertyName("item_total")]
        public UnitAmount ItemTotal { get; set; }
        [JsonPropertyName("discount")]
        public Discount Discount { get; set; }
        [JsonPropertyName("tax_total")]
        public UnitAmount TaxTotal { get; set; }
    }
}
