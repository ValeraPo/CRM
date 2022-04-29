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
        Task<bool> CheckPin2FA(int pin, int leadId);
        Task<int> SetChacheTransactionModel(TransactionRequestModel transactionModel);
        Task<TransactionRequestModel> GetChacheTransactionModel(int tmpId);
    }

}