
using CRM.BusinessLayer.Models;
using RestSharp;

namespace CRM.BusinessLayer.Services
{
    public interface ITransactionServise
    {
        Task<RestResponse> AddDeposit(TransactionModel transactionModel);
        Task<RestResponse> AddTransfer(TransactionModel transactionModel, int accountIdTo, int currencyTo);
        Task<RestResponse> Withdraw(TransactionModel transactionModel);
    }
}