using Marvelous.Contracts.Enums;

namespace CRM.APILayer.Models
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Currency CurrencyType { get; set; }
        public decimal Balance { get; set; }
    }
}
