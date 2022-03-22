using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts;
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

        public async Task<RestResponse> AddDeposit(TransactionRequestModel transactionModel)
        {
            _logger.LogInformation($"Попытка транзакии с аккаунта id = {transactionModel.AccountId}.");
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            _logger.LogInformation($"Отправка запроса на транзакию с аккаунта id = {transactionModel.AccountId}.");
            var response = await _requestHelper.SendRequest<TransactionRequestModel>(_url, UrlTransaction.Deposit, Method.Post, transactionModel);
            _logger.LogInformation($"Получен ответ на транзакию с аккаунта id = {transactionModel.AccountId}.");

            return response;
        }

        public async Task<RestResponse> AddTransfer(TransferRequestModel transactionModel)
        {
            _logger.LogInformation($"Попытка трансфера с аккаунта id = {transactionModel.AccountIdFrom} на аккаунт id = {transactionModel.AccountIdTo}.");
            var entity = _accountRepository.GetById(transactionModel.AccountIdFrom);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdFrom, entity);
            var accountTo = _accountRepository.GetById(transactionModel.AccountIdTo);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdTo, accountTo);
            _logger.LogInformation($"Отправка запроса трансфера с аккаунта id = {transactionModel.AccountIdFrom} на аккаунт id = {transactionModel.AccountIdTo}.");
            var response = await _requestHelper.SendRequest<TransferRequestModel>(_url, UrlTransaction.Transfer, Method.Post, transactionModel);
            _logger.LogInformation($"Получен ответ на трансфер с аккаунта id = {transactionModel.AccountIdFrom} на аккаунт id = {transactionModel.AccountIdTo}.");

            return response;
        }

        public async Task<RestResponse> Withdraw(TransactionRequestModel transactionModel)
        {
            _logger.LogInformation($"Попытка вывода средств с аккаунта id = {transactionModel.AccountId}.");
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            _logger.LogInformation($"Отправка запроса на вывод средств с аккаунта id = {transactionModel.AccountId}.");
            var response = await _requestHelper.SendRequest<TransactionRequestModel>(_url, UrlTransaction.Deposit, Method.Post, transactionModel);
            _logger.LogInformation($"Получен ответ на вывод средств с аккаунта id = {transactionModel.AccountId}.");

            return response;
        }
    }
}
