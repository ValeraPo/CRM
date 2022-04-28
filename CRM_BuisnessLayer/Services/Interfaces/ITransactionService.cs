using Marvelous.Contracts.RequestModels;
using System.Collections;

namespace CRM.BusinessLayer.Services
{
    public interface ITransactionService
    {
        Task<int> AddDeposit(TransactionRequestModel transactionModel, int leadId);
        Task<int> AddTransfer(TransferRequestModel transactionModel, int leadId);
        Task<int> Withdraw(TransactionRequestModel transactionModel, int leadId);
        Task<string> GetTransactionsByAccountId(int id, int leadId);
    }

}