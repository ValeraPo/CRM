using Marvelous.Contracts.RequestModels;

namespace CRM.BusinessLayer.Services
{
    public interface ITransactionService
    {
        Task<ComissionTransactionModel> AddDeposit(TransactionRequestModel transactionModel, int leadId);
        Task<int> AddTransfer(TransferRequestModel transactionModel, int leadId);
        Task<string> GetTransactionsByAccountId(int id, int leadId);
        Task<ComissionTransactionModel> Withdraw(TransactionRequestModel transactionModel, int leadId);
    }
}