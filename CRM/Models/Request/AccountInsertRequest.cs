namespace CRM.APILayer.Models
{
    public class AccountInsertRequest : AccountUpdateRequest
    {
        public int CurrencyType { get; set; }
    }
}
