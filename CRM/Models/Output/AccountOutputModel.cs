namespace CRM.APILayer.Models
{ 
    public class AccountOutputModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CurrencyType { get; set; }
        public decimal Balance { get; set; }
    }
}
