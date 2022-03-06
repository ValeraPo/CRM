using CRM.DataLayer.Entities;

namespace CRM.DataLayer.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        int AddAccount(Account account);
        void UpdateAccountById(Account account);
        void LockById(int id);
        void UnlockById(int id);
        List<Account> GetByLead(int leadId);
        Account GetById(int id);
    }
}
