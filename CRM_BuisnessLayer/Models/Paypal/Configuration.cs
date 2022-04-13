using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class Configuration
    {
        [JsonPropertyName("tax_calculated_after_discount")]
        public bool? TaxCalculatedAfterDiscount { get; set; }
        [JsonPropertyName("tax_inclusive")]
        public bool? TaxInclusive { get; set; }
        [JsonPropertyName("allow_tip")]
        public bool? AllowTip { get; set; }
        [JsonPropertyName("template_id")]
        public string TemplateId { get; set; }
    }
}
