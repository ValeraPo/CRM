using Marvelous.Contracts.Enums;

namespace CRM.BusinessLayer.Models
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Currency CurrencyType { get; set; }
        public LeadModel Lead { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? LockDate { get; set; }
        public decimal Balance { get; set; }
    }
}
