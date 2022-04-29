using CRM.BusinessLayer.Helpers;
using CRM.BusinessLayer.Models;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ExchangeModels;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _config;
        private readonly IMemoryCache _memoryCache;

        public TransactionService(IAccountRepository accountRepository, IRequestHelper requestHelper, ILogger<TransactionService> logger, ILeadRepository leadRepository, IMemoryCache memoryCache)

        public TransactionService(IAccountRepository accountRepository, IRequestHelper requestHelper, ILogger<TransactionService> logger, IConfiguration config)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _requestHelper = requestHelper;
            _leadRepository= leadRepository;
            _memoryCache = memoryCache;
            _config = config;
        }

        public async Task<ComissionTransactionExchangeModel> AddDeposit(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received a request to add a deposit to an account with ID =  {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            transactionModel.Currency = entity.CurrencyType;
            var ammountComission = GetAmountCommission(TransactionType.Deposit.ToString(), transactionModel.Amount, entity.Lead.Role);
            transactionModel.Amount -= ammountComission;
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Deposit, transactionModel);
            _logger.LogInformation($"Request successful.");

            ComissionTransactionExchangeModel comissionTransaction = new ComissionTransactionExchangeModel();
            comissionTransaction.IdTransaction = response;
            comissionTransaction.AmountComission = ammountComission;
            return comissionTransaction;
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

        public async Task<ComissionTransactionExchangeModel> Withdraw(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received withdraw request from account with ID = {transactionModel.AccountId}.");
            var comissionTransaction = new ComissionTransactionExchangeModel();
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            transactionModel.Currency = entity.CurrencyType;
            var ammountComission = GetAmountCommission(TransactionType.Withdraw.ToString(), transactionModel.Amount, entity.Lead.Role);
            transactionModel.Amount -= ammountComission;
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Withdraw, transactionModel);
            _logger.LogInformation($"Request successful.");
            comissionTransaction.IdTransaction = response;
            comissionTransaction.AmountComission = ammountComission;
            return comissionTransaction;
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

        private decimal GetAmountCommission(string typeTransaction, decimal amount, Role role)
        {
            var nameKeyConfig = $"{typeTransaction}{role}";
            var commission = Convert.ToInt32(_config[nameKeyConfig]);
            var amountCommission = amount * (decimal)(commission * 0.01);
            _logger.LogInformation($"Before ammount:{amount} Discard commission {amountCommission}, Ammount={amount - amountCommission}");

            return amountCommission;
        }
    }
}
