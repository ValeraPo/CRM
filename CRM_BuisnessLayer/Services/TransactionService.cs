using CRM.BusinessLayer.Helpers;
using CRM.BusinessLayer.Models;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

namespace CRM.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<TransactionService> _logger;
        private readonly ILeadRepository _leadRepository;
        private readonly IMemoryCache _memoryCache;

        public TransactionService(IAccountRepository accountRepository, IRequestHelper requestHelper, ILogger<TransactionService> logger, ILeadRepository leadRepository, IMemoryCache memoryCache)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _requestHelper = requestHelper;
            _leadRepository= leadRepository;
            _memoryCache = memoryCache;
        }

        public async Task<int> AddDeposit(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received a request to add a deposit to an account with ID =  {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            transactionModel.Currency = entity.CurrencyType;
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Deposit, transactionModel);
            _logger.LogInformation($"Request successful.");

            return response;
        }

        public async Task<int> AddTransfer(TransferRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Transfer request received from account with ID {transactionModel.AccountIdFrom} to account with ID {transactionModel.AccountIdTo}.");
            var accountFrom = await _accountRepository.GetById(transactionModel.AccountIdFrom);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdFrom, accountFrom);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(accountFrom.Lead.Id, leadId);
            var accountTo = await _accountRepository.GetById(transactionModel.AccountIdTo);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdTo, accountTo);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(accountTo.Lead.Id, leadId);
            transactionModel.CurrencyTo = accountTo.CurrencyType;
            transactionModel.CurrencyFrom = accountFrom.CurrencyType;
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
            transactionModel.Currency = entity.CurrencyType;
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

        public async Task<bool> CheckPin2FA(int pin, int leadId)
        {
            var entity = await _leadRepository.GetById(leadId);
            ExceptionsHelper.ThrowIfPin2FAIsIncorrected(pin, leadId, entity.Password);
            return true;
        }

        public async Task<int> SetChacheTransactionModel(TransactionRequestModel transactionModel)
        {
            int key =transactionModel.AccountId+Convert.ToInt32(DateTime.Now.ToString());
            _memoryCache.Set(key, transactionModel, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            return key;
        }

        public async Task<TransactionRequestModel> GetChacheTransactionModel(int tmpId)
        {
            return (TransactionRequestModel)_memoryCache.Get(tmpId);
        }
}
