using CRM.DataLayer.Entities;

namespace CRM.DataLayer.Repositories.Interfaces
{
    public interface ILeadRepository
    {
        int AddLead(Lead lead);
        void UpdateLeadById(Lead lead);
        void ChangeRoleLead(Lead lead);
        void DeleteById(int id);
        void RestoreById(int id);
        void ChangePassword(int id, string hashPassword);
        List<Lead> GetAll();
        Lead GetById(int id);
        Lead GetByEmail(string email);
        List<string> GetAllEmails();
    }
}
