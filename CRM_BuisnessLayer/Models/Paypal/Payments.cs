using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class Payments
    {
        [JsonPropertyName("paid_amount")]
        public UnitAmount PaidAmount { get; set; }
        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; }
    }
}
