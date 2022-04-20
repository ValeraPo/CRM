using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Collections;

namespace CRM.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<TransactionService> _logger;
        private readonly IConfiguration _config;


        public TransactionService(IAccountRepository accountRepository, IRequestHelper requestHelper, ILogger<TransactionService> logger, IConfiguration config)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _requestHelper = requestHelper;
            _config = config;
        }

        public async Task<int> AddDeposit(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received a request to add a deposit to an account with ID =  {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            transactionModel.Amount -= GetAmountCommission("Deposit", transactionModel.Amount, entity.Lead.Role);
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Deposit, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        public async Task<int> AddTransfer(TransferRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Transfer request received from account with ID {transactionModel.AccountIdFrom} to account with ID {transactionModel.AccountIdTo}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountIdFrom);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdFrom, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            var accountTo = await _accountRepository.GetById(transactionModel.AccountIdTo);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdTo, accountTo);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(accountTo.Lead.Id, leadId);
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Transfer, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        public async Task<int> Withdraw(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received withdraw request from account with ID = {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            transactionModel.Amount -= GetAmountCommission("Withdraw", transactionModel.Amount, entity.Lead.Role);
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Withdraw,  transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        public async Task<string> GetTransactionsByAccountId(int id, int leadId)
        {
            _logger.LogInformation($"Popytka polucheniia transakcii accounta id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            _logger.LogInformation($"Otpravka zaprosa na poluchenie transakcii accounta id = {id}.");
            var response = await _requestHelper.GetTransactions(id);
            _logger.LogInformation($"Poluchen otvet na poluchenie transakcii accounta id = {id}.");

            return response;
        }

        private decimal GetAmountCommission(string typeTransaction, decimal amount, Role role)
        {
            int commission = Convert.ToInt32(_config[$"{typeTransaction}{Enum.GetName(typeof(Role), (int)role)}"]);
            decimal amountCommission = amount * (decimal)(commission * 0.01);
            _logger.LogInformation($"Before ammount:{amount} Discard commission {amountCommission}, Ammount={amount - amountCommission}");
            return amountCommission;
        }
    }
}
