using CRM.BusinessLayer.Models;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface IAccountService
    {
        int AddVipAccount(AccountModel accountModel);
        int AddRegularAccount(AccountModel accountModel);
        void UpdateAccount(int id, AccountModel accountModel);
        void LockById(int id);
        void UnlockById(int id);
        List<AccountModel> GetByLead(int leadId);
        AccountModel GetById(int id, int leadId);
    }
}
