using AutoMapper;
using CRM.BusinessLayer.Services;
using Marvelous.Contracts;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.APILayer.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        private static Logger _logger;

        public TransactionsController(ITransactionService transactionService, IMapper autoMapper)
        {
            _transactionService = transactionService;
            _mapper = autoMapper;
            _logger = LogManager.GetCurrentClassLogger();
        }

        // api/deposit/
        [HttpPost(UrlTransaction.Deposit)]
        [SwaggerOperation(Summary = "Add deposit")]
        [SwaggerResponse(201, "Deposit added")]
        public ActionResult AddDeposit([FromBody] TransactionRequestModel transaction)
        {
            _logger.Info($"Получен запрос на добавление депозита в аккаунт id = {transaction.AccountId}.");
            var transactionId = _transactionService.AddDeposit(transaction);
            _logger.Info($"Депозит с id = {transactionId} успешно добавлен в аккаунт id = {transaction.AccountId}.");

            return StatusCode(201, transactionId);
        }

        // api/trsnsfer/
        [HttpPost(UrlTransaction.Transfer)]
        [SwaggerOperation(Summary = "Add transfer")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        public ActionResult AddTransfer([FromBody] TransferRequestModel transaction)
        {
            _logger.Info($"Получен запрос на добавление трансфера с аккаунта id = {transaction.AccountIdFrom} на аккаунт id = {transaction.AccountIdTo}.");
            var transactionId = _transactionService.AddTransfer(transaction);
            _logger.Info($"Трансфер с id = {transactionId} с аккаунта id = {transaction.AccountIdFrom} на аккаунт id = {transaction.AccountIdTo} прошел успешно.");
            return StatusCode(201, transactionId);
        }

        // api/withdraw/
        [HttpPost(UrlTransaction.Withdraw)]
        [SwaggerOperation(Summary = "Withdraw")]
        [SwaggerResponse(201, "Withdraw successful")]
        public ActionResult Withdraw([FromBody] TransactionRequestModel transaction)
        {
            _logger.Info($"Получен запрос на вывод средств с аккаунта id = {transaction.AccountId}.");
            var transactionId = _transactionService.Withdraw(transaction);
            _logger.Info($"Вывод средств с id = {transactionId} с аккаунта id = {transaction.AccountId} прошел успешно.");

            return StatusCode(201, transactionId);
        }
    }
}
