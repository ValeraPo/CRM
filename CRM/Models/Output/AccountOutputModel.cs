namespace CRM.APILayer.Models
{ 
    public class AccountOutputModel
    {
        public string Name { get; set; }
        public string CurrencyType { get; set; }
        public decimal Balance { get; set; }
    }
}
