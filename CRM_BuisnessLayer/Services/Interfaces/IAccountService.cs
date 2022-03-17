using CRM.BusinessLayer.Models;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface IAccountService
    {
        int AddAccount(int role, AccountModel accountModel);
        void UpdateAccount(int leadId, int id, AccountModel accountModel);
        void LockById(int id);
        void UnlockById(int id);
        List<AccountModel> GetByLead(int leadId);
        AccountModel GetById(int id, int leadId);
    }
}
