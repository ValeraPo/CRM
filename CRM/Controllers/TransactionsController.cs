using AutoMapper;
using CRM.APILayer.Extensions;
using CRM.BusinessLayer.Services;
using Marvelous.Contracts;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.Urls;
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
        [HttpPost(TransactionUrls.Deposit)]
        [SwaggerOperation(Summary = "Add deposit")]
        [SwaggerResponse(201, "Deposit added")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddDeposit([FromBody] TransactionRequestModel transaction)
        {
            _logger.LogInformation($"Received a request to add a deposit to an account with ID = {transaction.AccountId}.");
            var leadId = this.GetLeadFromToken().Id;
            var response = await _transactionService.AddDeposit(transaction, leadId);
            _logger.LogInformation($"Successfully added deposit to account with ID = {transaction.AccountId}. Deposit ID = {response.Content}.");

            return StatusCode(201, response.Content);
        }

        // api/trsnsfer/
        [HttpPost(TransactionUrls.Transfer)]
        [SwaggerOperation(Summary = "Add transfer")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddTransfer([FromBody] TransferRequestModel transaction)
        {
            _logger.LogInformation($"Transfer request received from account with ID {transaction.AccountIdFrom} to account with ID {transaction.AccountIdTo}.");
            var leadId = this.GetLeadFromToken().Id;
            var response = await _transactionService.AddTransfer(transaction, leadId);
            _logger.LogInformation($"Successfully added transfer from account with ID {transaction.AccountIdFrom} to account with ID {transaction.AccountIdTo}. Transfer ID = {response.Content}.");
            return StatusCode(201, response.Content);
        }

        // api/withdraw/
        [HttpPost(TransactionUrls.Withdraw)]
        [SwaggerOperation(Summary = "Withdraw")]
        [SwaggerResponse(201, "Withdraw successful")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Withdraw([FromBody] TransactionRequestModel transaction)
        {
            _logger.LogInformation($"Received withdrawal request from account with ID = {transaction.AccountId}.");
            var leadId = this.GetLeadFromToken().Id;
            var response = await _transactionService.Withdraw(transaction, leadId);
            _logger.LogInformation($"Successfully passed the request for withdrawal of funds from the account with the ID {transaction.AccountId}. Withdraw ID = {response.Content}.");

            return StatusCode(201, response.Content);
        }


    }
}
