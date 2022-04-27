using CRM.BusinessLayer.Models;
using Marvelous.Contracts.RequestModels;

namespace CRM.APILayer.Producers
{
    public interface ICRMProducers
    {
        Task NotifyLeadAdded(LeadModel lead);
        Task NotifyLeadAdded(int id);
        Task NotifyAccountAdded(AccountModel account);
        Task NotifyAccountAdded(int id);
        Task NotifyWhithdraw(int leadId, TransactionRequestModel transaction);
    }
}