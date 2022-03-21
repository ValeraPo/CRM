using CRM.BusinessLayer.Models;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface IAccountService
    {
        Task<int> AddAccount(int role, AccountModel accountModel);
        void UpdateAccount(int leadId, AccountModel accountModel);
        void LockById(int id);
        void UnlockById(int id);
        Task<List<AccountModel>> GetByLead(int leadId);
        Task<AccountModel> GetById(int id, int leadId);
    }
}
