using CRM_BuisnessLayer.Models;

namespace CRM_BuisnessLayer.Services.Interfaces
{
    public interface ILeadService
    {
        int AddLead(LeadModel leadModel);
        void UpdateLead(LeadModel leadModel);
    }
}
