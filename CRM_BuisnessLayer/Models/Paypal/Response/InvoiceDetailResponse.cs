using System.Text.Json.Serialization;

namespace CRM.BusinessLayer.Models.Paypal
{
    public class InvoiceDetailResponse : InvoiceDetail
    {
        [JsonPropertyName("invoice_date")]
        public string InvoiceDate { get; set; }
        [JsonPropertyName("viewed_by_recipient")]
        public bool? ViewedByRecipient { get; set; }
        [JsonPropertyName("group_draft")]
        public bool? GroupDraft { get; set; }
        [JsonPropertyName("metadata")]
        public MetadataResponse Metadata { get; set; }
        [JsonPropertyName("archived")]
        public bool? Archived { get; set; }
    }
}
