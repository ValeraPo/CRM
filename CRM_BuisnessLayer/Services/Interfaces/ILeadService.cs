using CRM_BuisnessLayer.Models;

namespace CRM_BuisnessLayer.Services.Interfaces
{
    public interface ILeadService
    {
        void AddLead(LeadModel leadModel);
        void UpdateLead(LeadModel leadModel);
    }
}
