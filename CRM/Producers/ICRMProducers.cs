using CRM.BusinessLayer.Models;
using Marvelous.Contracts.RequestModels;


namespace CRM.APILayer.Producers
{
    public interface ICRMProducers
    {
        Task AmmountCommissionForTransactionAdded(ComissionTransactionModel comissiontransaction);
        Task NotifyAccountAdded(AccountModel account);
        Task NotifyAccountAdded(int id);
        Task NotifyLeadAdded(int id);
        Task NotifyLeadAdded(LeadModel lead);
    }
}