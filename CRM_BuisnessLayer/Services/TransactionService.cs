using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Logging;

namespace CRM.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(IAccountRepository accountRepository, IRequestHelper requestHelper, ILogger<TransactionService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        //Adding deposit
        public async Task<int> AddDeposit(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received a request to add a deposit to an account with ID =  {transactionModel.AccountId}.");
            //Check if account doesn't exist
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            //Check if lead's if from account and lead's id from token aren't same
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            transactionModel.Currency = entity.CurrencyType;
            _logger.LogInformation($"Send request.");
            // Sent request by RestSharp
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Deposit, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        //Adding transfer
        public async Task<int> AddTransfer(TransferRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Transfer request received from account with ID {transactionModel.AccountIdFrom} to account with ID {transactionModel.AccountIdTo}.");
            //Check if accounts doesn't exist
            //Check if lead's if from account and lead's id from token aren't same
            var accountFrom = await _accountRepository.GetById(transactionModel.AccountIdFrom);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdFrom, accountFrom);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(accountFrom.Lead.Id, leadId);
            var accountTo = await _accountRepository.GetById(transactionModel.AccountIdTo);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdTo, accountTo);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(accountTo.Lead.Id, leadId);

            transactionModel.CurrencyTo = accountTo.CurrencyType;
            transactionModel.CurrencyFrom = accountFrom.CurrencyType;
            _logger.LogInformation($"Send request.");
            // Sent request by RestSharp
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Transfer, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        //Withdraw
        public async Task<int> Withdraw(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received withdraw request from account with ID = {transactionModel.AccountId}.");
            //Check if accounts doesn't exist
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            //Check if lead's if from account and lead's id from token aren't same
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            transactionModel.Currency = entity.CurrencyType;
            _logger.LogInformation($"Send request.");
            // Sent request by RestSharp
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Withdraw,  transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        //Getting all account's transactions
        public async Task<string> GetTransactionsByAccountId(int id, int leadId)
        {
            _logger.LogInformation($"Popytka polucheniia transakcii accounta id = {id}.");
            //Check if accounts doesn't exist
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            //Check if lead's if from account and lead's id from token aren't same
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            _logger.LogInformation($"Otpravka zaprosa na poluchenie transakcii accounta id = {id}.");
            // Sent request by RestSharp
            var response = await _requestHelper.GetTransactions(id);
            _logger.LogInformation($"Poluchen otvet na poluchenie transakcii accounta id = {id}.");

            return response;
        }
    }
}
