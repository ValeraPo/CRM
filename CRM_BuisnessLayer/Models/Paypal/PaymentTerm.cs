using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class PaymentTerm
    {
        //Valid Values: ["DUE_ON_RECEIPT", "DUE_ON_DATE_SPECIFIED", "NET_10", "NET_15", "NET_30", "NET_45", "NET_60", "NET_90", "NO_DUE_DATE"]
        [JsonPropertyName("term_type")]
        public string TermType { get; set; }
        [JsonPropertyName("due_date")]
        public string DueDate { get; set; }
    }
}
