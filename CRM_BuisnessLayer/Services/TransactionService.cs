using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public async Task<ComissionTransactionModel> AddDeposit(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received a request to add a deposit to an account with ID =  {transactionModel.AccountId}.");
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
            var ammountComission = GetAmountCommission(TransactionType.Deposit.ToString(), transactionModel.Amount, entity.Lead.Role);
            transactionModel.Amount -= ammountComission;
            _logger.LogInformation($"Send request.");
            var response = await _requestHelper.SendTransactionPostRequest(TransactionEndpoints.Deposit, transactionModel);
            _logger.LogInformation($"Request successful.");
            
            ComissionTransactionModel comissionTransaction = new ComissionTransactionModel();
            comissionTransaction.IdTransaction = response;
            comissionTransaction.AmountComission = ammountComission;
            return comissionTransaction;
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

        public async Task<ComissionTransactionModel> Withdraw(TransactionRequestModel transactionModel, int leadId)
        {
            _logger.LogInformation($"Received withdraw request from account with ID = {transactionModel.AccountId}.");
            var comissionTransaction = new ComissionTransactionModel();
            var entity = await _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId);
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
