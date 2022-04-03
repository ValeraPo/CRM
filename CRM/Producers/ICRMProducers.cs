using CRM.BusinessLayer.Models;

namespace CRM.APILayer.Producers
{
    public interface ICRMProducers
    {
        Task NotifyLeadAdded(LeadModel lead);
        Task NotifyLeadAdded(int id);
        Task NotifyAccountAdded(AccountModel account);
        Task NotifyAccountAdded(int id);
    }
}