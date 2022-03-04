using CRM.DataLayer.Entities;

namespace CRM.DataLayer.Repositories.Interfaces
{
    public interface ILeadRepository
    {
        int AddLead(Lead lead);
        void UpdateLeadById(Lead lead);
    }
}
