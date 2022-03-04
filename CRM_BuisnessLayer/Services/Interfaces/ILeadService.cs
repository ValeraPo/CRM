using CRM.BusinessLayer.Models;

namespace CRM.BusinessLayer.Services.Interfaces
{
    public interface ILeadService
    {
        int AddLead(LeadModel leadModel);
        void UpdateLead(LeadModel leadModel);
    }
}
