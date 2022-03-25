using CRM.BusinessLayer.Models;
using Marvelous.Contracts;
using RestSharp;

namespace CRM.BusinessLayer.Services
{
    public interface ITransactionService
    {
        Task<RestResponse> AddDeposit(TransactionRequestModel transactionModel, int leadId);
        Task<RestResponse> AddTransfer(TransferRequestModel transactionModel, int leadId);
        Task<RestResponse> Withdraw(TransactionRequestModel transactionModel, int leadId);
    }
}