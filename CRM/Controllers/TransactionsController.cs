using AutoMapper;
using CRM.BusinessLayer.Services;
using Marvelous.Contracts;
using Marvelous.Contracts.RequestModels;
using Microsoft.AspNetCore.Mvc;
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddDeposit([FromBody] TransactionRequestModel transaction)
        {
            _logger.LogInformation($"Poluchen zapros na dobavlenye depozita v account id = {transaction.AccountId}.");
            var response = await _transactionService.AddDeposit(transaction);
            _logger.LogInformation($"Depozit c id = {response.Content} uspeshno dobavlen v account id = {transaction.AccountId}.");

            return StatusCode(201, response.Content);
        }

        // api/trsnsfer/
        [HttpPost(UrlTransaction.Transfer)]
        [SwaggerOperation(Summary = "Add transfer")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddTransfer([FromBody] TransferRequestModel transaction)
        {
            _logger.LogInformation($"Poluchen zapros na dobavlenie transfera c accounta id = {transaction.AccountIdFrom} na account id = {transaction.AccountIdTo}.");
            var response = await _transactionService.AddTransfer(transaction);
            _logger.LogInformation($"Transfer c id = {response.Content} c accounta id = {transaction.AccountIdFrom} na account id = {transaction.AccountIdTo} proshel uspeshno.");
            return StatusCode(201, response.Content);
        }

        // api/withdraw/
        [HttpPost(UrlTransaction.Withdraw)]
        [SwaggerOperation(Summary = "Withdraw")]
        [SwaggerResponse(201, "Withdraw successful")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Withdraw([FromBody] TransactionRequestModel transaction)
        {
            _logger.LogInformation($"Poluchen zapros na vyvod sredstv c accounta id = {transaction.AccountId}.");
            var response = await _transactionService.Withdraw(transaction);
            _logger.LogInformation($"Vyvod sredstv c id = {response.Content} c accounta id = {transaction.AccountId} proshel uspeshno.");

            return StatusCode(201, response.Content);
        }
    }
}
