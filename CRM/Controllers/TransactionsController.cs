using AutoMapper;
using CRM.APILayer.Extensions;
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
            _logger.LogInformation($"Poluchen zapros na dobavlenye depozita v account id = {transaction.AccountId}.");

            var leadId = this.GetLeadFromToken().Id;

            var transactionId = await _transactionService.AddDeposit(transaction);
            _logger.LogInformation($"Depozit c id = {transactionId} uspeshno dobavlen v account id = {transaction.AccountId}.");

            return StatusCode(201, transactionId);
        }

        // api/trsnsfer/
        [HttpPost(UrlTransaction.Transfer)]
        [SwaggerOperation(Summary = "Add transfer")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        public async Task<ActionResult> AddTransfer([FromBody] TransferRequestModel transaction)
        {
            _logger.LogInformation($"Poluchen zapros na dobavlenie transfera c accounta id = {transaction.AccountIdFrom} na account id = {transaction.AccountIdTo}.");
            var leadId = this.GetLeadFromToken().Id;
            var transactionId = await _transactionService.AddTransfer(transaction);
            _logger.LogInformation($"Transfer c id = {transactionId} c accounta id = {transaction.AccountIdFrom} na account id = {transaction.AccountIdTo} proshel uspeshno.");
            return StatusCode(201, transactionId);
        }

        // api/withdraw/
        [HttpPost(UrlTransaction.Withdraw)]
        [SwaggerOperation(Summary = "Withdraw")]
        [SwaggerResponse(201, "Withdraw successful")]
        public async Task<ActionResult> Withdraw([FromBody] TransactionRequestModel transaction)
        {
            _logger.LogInformation($"Poluchen zapros na vyvod sredstv c accounta id = {transaction.AccountId}.");
            var leadId = this.GetLeadFromToken().Id;
            var transactionId = await _transactionService.Withdraw(transaction);
            _logger.LogInformation($"Vyvod sredstv c id = {transactionId} c accounta id = {transaction.AccountId} proshel uspeshno.");

            return StatusCode(201, transactionId);
        }
    }
}
