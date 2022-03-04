
namespace CRM_BuisnessLayer.Models
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CurrencyType { get; set; }
        public LeadModel Lead { get; set; }
        public decimal Balance { get; set; }
    }
}
