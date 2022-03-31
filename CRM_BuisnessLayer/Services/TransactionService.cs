using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Logging;
using NLog;
using RestSharp;


namespace CRM.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<TransactionService> _logger;
        private const string _url = "https://piter-education.ru:6060";

        public TransactionService(IAccountRepository accountRepository, IRequestHelper requestHelper, ILogger<TransactionService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        public async Task<RestResponse> AddDeposit(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received a request to add a deposit to an account with ID =  {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendRequest<TransactionRequestModel>(_url, UrlTransaction.Deposit, Method.Post, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        public async Task<RestResponse> AddTransfer(TransferRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Transfer request received from account with ID {transactionModel.AccountIdFrom} to account with ID {transactionModel.AccountIdTo}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountIdFrom);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdFrom, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            var accountTo = await _accountRepository.GetById(transactionModel.AccountIdTo);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdTo, accountTo);
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendRequest<TransferRequestModel>(_url, UrlTransaction.Transfer, Method.Post, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        public async Task<RestResponse> Withdraw(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received withdrawal request from account with ID = {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendRequest<TransactionRequestModel>(_url, UrlTransaction.Withdraw, Method.Post, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        public async Task<decimal> GetBalance(int id)
        {
            _logger.LogInformation($"Received get balance request from account with ID = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendGetRequest(_url, id);
            _logger.LogInformation($"Request successful.");

            return Convert.ToDecimal(response.Content);
        }
    }
}
