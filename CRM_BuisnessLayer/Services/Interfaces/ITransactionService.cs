using Marvelous.Contracts.ExchangeModels;
using Marvelous.Contracts.RequestModels;

namespace CRM.BusinessLayer.Services
{
    public interface ITransactionService
    {
        Task<ComissionTransactionExchangeModel> AddDeposit(TransactionRequestModel transactionModel, int leadId);
        Task<int> AddTransfer(TransferRequestModel transactionModel, int leadId);
        Task<string> GetTransactionsByAccountId(int id, int leadId);
        Task<string> Withdraw(TransactionRequestModel transactionRequestModel);
        Task<Tuple<ComissionTransactionExchangeModel, TransactionRequestModel>> WithdrawApproved(int tmpId, int leadId, int pin2FA);
    }
}