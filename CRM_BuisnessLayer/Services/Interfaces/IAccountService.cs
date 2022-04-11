using CRM.BusinessLayer.Models;
using Marvelous.Contracts.Enums;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface IAccountService
    {
        Task<int> AddAccount(Role role, AccountModel accountModel);
        Task UpdateAccount(int leadId, AccountModel accountModel);
        Task LockById(int id);
        Task UnlockById(int id);
        Task<List<AccountModel>> GetByLead(int leadId);
        Task<AccountModel> GetById(int id, int leadId);
        Task<AccountModel> GetById(int id);
        Task<decimal> GetBalance(int leadId, Currency currencyType);
    }
}
