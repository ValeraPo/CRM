using CRM.BusinessLayer.Models;

namespace CRM.APILayer.Producers
{
    public interface ICRMProducers
    {
        Task NotifyLeadAdded(LeadModel lead);
        Task NotifyAccountAdded(AccountModel account);
    }
}