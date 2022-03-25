using CRM.BusinessLayer.Models;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface IAccountService
    {
        Task<int> AddAccount(int role, AccountModel accountModel);
        Task UpdateAccount(int leadId, AccountModel accountModel);
        Task LockById(int id);
        Task UnlockById(int id);
        Task<List<AccountModel>> GetByLead(int leadId);
        Task<AccountModel> GetById(int id, int leadId);
        Task<AccountModel> GetById(int id);
    }
}
