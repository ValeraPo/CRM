using Marvelous.Contracts.Enums;

namespace CRM.DataLayer.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Currency CurrencyType { get; set; }
        public Lead Lead { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? LockDate { get; set; }
    }
}