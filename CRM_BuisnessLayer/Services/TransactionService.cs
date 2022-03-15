using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts;
using NLog;
using RestSharp;


namespace CRM.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private static Logger _logger;
        private const string _url = "https://piter-education.ru:6060";

        public TransactionService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public Task<RestResponse> AddDeposit(TransactionRequestModel transactionModel)
        {
            _logger.Info($"Попытка транзакии с аккаунта id = {transactionModel.AccountId}.");
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            _logger.Info($"Отправка запроса на транзакию с аккаунта id = {transactionModel.AccountId}.");
            var request = new RequestHelper<TransactionRequestModel>();
            var response = request.GenerateRequest(_url, UrlTransaction.Deposit, Method.Post, transactionModel);
            _logger.Info($"Получен ответ на транзакию с аккаунта id = {transactionModel.AccountId}.");

            return response;
        }

        public Task<RestResponse> AddTransfer(TransferRequestModel transactionModel)
        {
            _logger.Info($"Попытка трансфера с аккаунта id = {transactionModel.AccountIdFrom} на аккаунт id = {transactionModel.AccountIdTo}.");
            var entity = _accountRepository.GetById(transactionModel.AccountIdFrom);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdFrom, entity);
            var accountTo = _accountRepository.GetById(transactionModel.AccountIdTo);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountIdTo, accountTo);
            _logger.Info($"Отправка запроса трансфера с аккаунта id = {transactionModel.AccountIdFrom} на аккаунт id = {transactionModel.AccountIdTo}.");
            var request = new RequestHelper<TransferRequestModel>();
            var response = request.GenerateRequest(_url, UrlTransaction.Transfer, Method.Post, transactionModel);
            _logger.Info($"Получен ответ на трансфер с аккаунта id = {transactionModel.AccountIdFrom} на аккаунт id = {transactionModel.AccountIdTo}.");

            return response;
        }

        public Task<RestResponse> Withdraw(TransactionRequestModel transactionModel)
        {
            _logger.Info($"Попытка вывода средств с аккаунта id = {transactionModel.AccountId}.");
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            _logger.Info($"Отправка запроса на вывод средств с аккаунта id = {transactionModel.AccountId}.");
            var request = new RequestHelper<TransactionRequestModel>();
            var response = request.GenerateRequest(_url, UrlTransaction.Deposit, Method.Post, transactionModel);
            _logger.Info($"Получен ответ на вывод средств с аккаунта id = {transactionModel.AccountId}.");

            return response;
        }
    }
}
