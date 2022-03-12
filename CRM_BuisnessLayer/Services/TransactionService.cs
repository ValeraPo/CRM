using CRM.BusinessLayer.Models;
using CRM.DataLayer.Repositories.Interfaces;
using NLog;
using RestSharp;


namespace CRM.BusinessLayer.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private static Logger _logger;


        public TransactionService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public Task<RestResponse> AddDeposit(TransactionModel transactionModel)
        {
            _logger.Info($"Попытка транзакии с аккаунта id = {transactionModel.AccountId}.");
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            var client = new RestClient("https://api.marvelous.com");
            var request = new RestRequest("/transaction/", Method.Post);
            _logger.Info($"Отправка запроса на транзакию с аккаунта id = {transactionModel.AccountId}.");
            request.AddJsonBody(transactionModel);
            var response = client.ExecuteAsync(request);
            _logger.Info($"Получен ответ на транзакию с аккаунта id = {transactionModel.AccountId}.");

            return response;
        }

        public Task<RestResponse> AddTransfer(TransactionModel transactionModel, int accountIdTo, int currencyTo)
        {
            _logger.Info($"Попытка трансфера с аккаунта id = {transactionModel.AccountId} на аккаунт id = {accountIdTo}.");
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            var accountTo = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, accountTo);

            var client = new RestClient("https://api.marvelous.com");
            var request = new RestRequest($"/transfer-to-{accountIdTo}-in-{currencyTo}/", Method.Post);
            request.AddJsonBody(transactionModel);
            _logger.Info($"Отправка запроса трансфера с аккаунта id = {transactionModel.AccountId} на аккаунт id = {accountIdTo}.");
            var response = client.ExecuteAsync(request);
            _logger.Info($"Получен ответ на трансфер с аккаунта id = {transactionModel.AccountId} на аккаунт id = {accountIdTo}.");

            return response;
        }

        public Task<RestResponse> Withdraw(TransactionModel transactionModel)
        {
            _logger.Info($"Попытка вывода средств с аккаунта id = {transactionModel.AccountId}.");
            var entity = _accountRepository.GetById(transactionModel.AccountId);
            ExceptionsHelper.ThrowIfEntityNotFound(transactionModel.AccountId, entity);
            var client = new RestClient("https://api.marvelous.com");
            var request = new RestRequest("/withdraw/", Method.Post);
            _logger.Info($"Отправка запроса на вывод средств с аккаунта id = {transactionModel.AccountId}.");
            request.AddJsonBody(transactionModel);
            var response = client.ExecuteAsync(request);
            _logger.Info($"Получен ответ на вывод средств с аккаунта id = {transactionModel.AccountId}.");

            return response;
        }
    }
}
