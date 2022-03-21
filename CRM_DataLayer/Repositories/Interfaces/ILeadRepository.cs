using CRM.DataLayer.Entities;

namespace CRM.DataLayer.Repositories.Interfaces
{
    public interface ILeadRepository
    {
        Task<int> AddLead(Lead lead);
        void UpdateLeadById(Lead lead);
        void ChangeRoleLead(Lead lead);
        void DeleteById(int id);
        void RestoreById(int id);
        void ChangePassword(int id, string hashPassword);
        Task<List<Lead>> GetAll();
        Task<Lead> GetById(int id);
        Task<Lead> GetByEmail(string email);
        Task<List<string>> GetAllEmails();
    }
}
