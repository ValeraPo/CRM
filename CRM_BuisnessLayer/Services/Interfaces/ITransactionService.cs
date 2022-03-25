using Marvelous.Contracts;
using Marvelous.Contracts.RequestModels;
using RestSharp;

namespace CRM.BusinessLayer.Services
{
    public interface ITransactionService
    {
        Task<RestResponse> AddDeposit(TransactionRequestModel transactionModel);
        Task<RestResponse> AddTransfer(TransferRequestModel transactionModel);
        Task<RestResponse> Withdraw(TransactionRequestModel transactionModel);
        Task<decimal> GetBalance(int id);
    }
}