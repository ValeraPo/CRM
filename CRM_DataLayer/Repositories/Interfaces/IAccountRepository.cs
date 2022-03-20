using CRM.DataLayer.Entities;

namespace CRM.DataLayer.Repositories.Interfaces
{
    public interface IAccountRepository
    {
         Task<int> AddAccount(Account account);
         void UpdateAccountById(Account account);
         void LockById(int id);
         void UnlockById(int id);
         Task<List<Account>> GetByLead(int leadId);
         Task<Account> GetById(int id);
    }
}
