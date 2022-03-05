using CRM.DataLayer.Entities;

namespace CRM.DataLayer.Repositories.Interfaces
{
    public interface ILeadRepository
    {
        int AddLead(Lead lead);
        void UpdateLeadById(Lead lead);
        void DeleteById(int id);
        void RestoreById(int id);
        List<Lead> GetAll();
        Lead GetById(int id);
    }
}
