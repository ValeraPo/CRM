using CRM.BusinessLayer.Models;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface IAccountService
    {
        int AddAccount(AccountModel accountModel);
        void UpdateAccount(AccountModel accountModel);
        void LockById(int id);
        void UnlockById(int id);
        List<AccountModel> GetByLead(int leadId);
        AccountModel GetById(int id);
    }
}
