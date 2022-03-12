using Marvelous.Contracts;

namespace CRM.APILayer.Models
{
    public class TransactionRequest
    {
        public decimal Amount { get; set; }
        public int AccountId { get; set; }
        public Currency Currency { get; set; }
    }
}
