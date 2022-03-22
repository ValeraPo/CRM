using CRM.DataLayer.Entities;

namespace CRM.DataLayer.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<int> AddAccount(Account account);
        Task UpdateAccountById(Account account);
        Task LockById(int id);
        Task UnlockById(int id);
        Task<List<Account>> GetByLead(int leadId);
        Task<Account> GetById(int id);
    }
}
