using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class UnitAmountWithBreakdown : UnitAmount
    {
        [JsonPropertyName("breakdown")]
        public Breakdown Breakdown { get; set; }
    }
}
