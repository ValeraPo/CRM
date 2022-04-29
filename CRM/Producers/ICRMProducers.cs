using CRM.BusinessLayer.Models;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.ExchangeModels;


namespace CRM.APILayer.Producers
{
    public interface ICRMProducers
    {
        Task AmmountCommissionForTransactionAdded(ComissionTransactionExchangeModel comissiontransaction);
        Task NotifyAccountAdded(AccountModel account);
        Task NotifyAccountAdded(int id);
        Task NotifyWhithdraw(int leadId, TransactionRequestModel transaction);
        Task NotifyLeadAdded(int id);
        Task NotifyLeadAdded(LeadModel lead);
    }
}