using MarvelousContracts;

namespace CRM.BusinessLayer.Models
{
    public  class TransactionModel
    {
        public decimal Amount { get; set; }
        public int AccountId { get; set; }
        public Currency Currency { get; set; }
    }
}
