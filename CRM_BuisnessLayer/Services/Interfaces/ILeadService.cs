using CRM.BusinessLayer.Models;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface ILeadService
    {
        int AddLead(LeadModel leadModel);
        void UpdateLead(LeadModel leadModel);
        void DeleteById(int id);
        void RestoreById(int id);
        List<LeadModel> GetAll();
        LeadModel GetById(int id);
    }
}
