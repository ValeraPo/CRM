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
        private readonly ILogger<TransactionsController> _logger;


        public TransactionsController(ITransactionService transactionService, IMapper autoMapper, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _mapper = autoMapper;
            _logger = logger;
        }

        // api/deposit/
        [HttpPost(UrlTransaction.Deposit)]
        [SwaggerOperation(Summary = "Add deposit")]
        [SwaggerResponse(201, "Deposit added")]
        public async Task<ActionResult> AddDeposit([FromBody] TransactionRequestModel transaction)
        {
            _logger.LogInformation($"Получен запрос на добавление депозита в аккаунт id = {transaction.AccountId}.");
            var transactionId = _transactionService.AddDeposit(transaction);
            _logger.LogInformation($"Депозит с id = {transactionId} успешно добавлен в аккаунт id = {transaction.AccountId}.");

            return StatusCode(201, transactionId);
        }

        // api/trsnsfer/
        [HttpPost(UrlTransaction.Transfer)]
        [SwaggerOperation(Summary = "Add transfer")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        public async Task<ActionResult> AddTransfer([FromBody] TransferRequestModel transaction)
        {
            _logger.LogInformation($"Получен запрос на добавление трансфера с аккаунта id = {transaction.AccountIdFrom} на аккаунт id = {transaction.AccountIdTo}.");
            var transactionId = _transactionService.AddTransfer(transaction);
            _logger.LogInformation($"Трансфер с id = {transactionId} с аккаунта id = {transaction.AccountIdFrom} на аккаунт id = {transaction.AccountIdTo} прошел успешно.");
            return StatusCode(201, transactionId);
        }

        // api/withdraw/
        [HttpPost(UrlTransaction.Withdraw)]
        [SwaggerOperation(Summary = "Withdraw")]
        [SwaggerResponse(201, "Withdraw successful")]
        public async Task<ActionResult> Withdraw([FromBody] TransactionRequestModel transaction)
        {
            _logger.LogInformation($"Получен запрос на вывод средств с аккаунта id = {transaction.AccountId}.");
            var transactionId = _transactionService.Withdraw(transaction);
            _logger.LogInformation($"Вывод средств с id = {transactionId} с аккаунта id = {transaction.AccountId} прошел успешно.");

            return StatusCode(201, transactionId);
        }
    }
}
