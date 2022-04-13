using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class Transaction
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }
        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; }
        [JsonPropertyName("payment_date")]
        public string PaymentDate { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("amount")]
        public UnitAmount Amount { get; set; }
    }
}
