namespace CRM_APILayer.Models
{ 
    public class AccountRequest
    {
        public string Name { get; set; }
        public string CurrencyType { get; set; }
        public int LeadId { get; set; }
    }
}
