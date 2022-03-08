
using CRM.DataLayer.Entities;
using MarvelousContracts;


namespace CRM.BusinessLayer.Models
{
    public class AccountModel : Account
    {
        public decimal Balance { get; set; }
    }
}
