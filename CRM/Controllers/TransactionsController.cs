using AutoMapper;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
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

        // api/transaction/
        [HttpPost("deposit")]
        [SwaggerOperation(Summary = "Add deposit")]
        [SwaggerResponse(201, "Deposit added")]
        public ActionResult AddDeposit([FromBody] TransactionRequest transaction)
        {
            _logger.Info($"Получен запрос на добавление депозита в аккаунт id = {transaction.AccountId}.");
            var transactionModel = _mapper.Map<TransactionModel>(transaction);
            var transactionId = _transactionService.AddDeposit(transactionModel);
            _logger.Info($"Депозит с id = {transactionId} успешно добавлен в аккаунт id = {transaction.AccountId}.");

            return StatusCode(201, transactionId);
        }

        // api/transaction/
        [HttpPost("transfer-to-{accountId}-in-{currencyTo}")]
        [SwaggerOperation(Summary = "Add transfer")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        public ActionResult AddTransfer([FromBody] TransactionRequest transaction, int accountId, int currencyTo)
        {
            _logger.Info($"Получен запрос на добавление трансфера с аккаунта id = {transaction.AccountId} на аккаунт id = {accountId}.");
            var transactionModel = _mapper.Map<TransactionModel>(transaction);
            var transactionId = _transactionService.AddTransfer(transactionModel, accountId, currencyTo);
            _logger.Info($"Трансфер с id = {transactionId} с аккаунта id = {transaction.AccountId} на аккаунт id = {accountId} прошел успешно.");

            return StatusCode(201, transactionId);
        }

        [HttpPost("withdraw")]
        [SwaggerOperation(Summary = "Withdraw")]
        [SwaggerResponse(201, "Withdraw successful")]
        public ActionResult Withdraw([FromBody] TransactionRequest transaction)
        {
            _logger.Info($"Получен запрос на вывод средств с аккаунта id = {transaction.AccountId}.");
            var transactionModel = _mapper.Map<TransactionModel>(transaction);
            var transactionId = _transactionService.Withdraw(transactionModel);
            _logger.Info($"Вывод средств с id = {transactionId} с аккаунта id = {transaction.AccountId} прошел успешно.");

            return StatusCode(201, transactionId);
        }
    }
}
