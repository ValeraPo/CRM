using Marvelous.Contracts.RequestModels;

namespace CRM.BusinessLayer.Services
{
    public interface IInvoiceService
    {
        Task<string> GetNewInvoiceUrlToPay(TransactionRequestModel transactionModel, int leadId);
    }
}