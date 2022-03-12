using CRM.BusinessLayer.Models;
using CRM.DataLayer.Repositories.Interfaces;
using RestSharp;

namespace CRM.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;


        public TransactionService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Task<RestResponse> AddDeposit(TransactionModel transactionModel)
        {
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            var client = new RestClient("https://api.marvelous.com");
            var request = new RestRequest("/transaction/", Method.Post);
            request.AddJsonBody(transactionModel);
            var response = client.ExecuteAsync(request);

            return response;
        }

        public Task<RestResponse> AddTransfer(TransactionModel transactionModel, int accountIdTo, int currencyTo)
        {
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            var accountTo = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, accountTo);

            var client = new RestClient("https://api.marvelous.com");
            var request = new RestRequest($"/transfer-to-{accountIdTo}-in-{currencyTo}/", Method.Post);
            request.AddJsonBody(transactionModel);
            var response = client.ExecuteAsync(request);

            return response;
        }

        public Task<RestResponse> Withdraw(TransactionModel transactionModel)
        {
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            var client = new RestClient("https://api.marvelous.com");
            var request = new RestRequest("/withdraw/", Method.Post);
            request.AddJsonBody(transactionModel);
            var response = client.ExecuteAsync(request);
            return response;
        }
    }
}
