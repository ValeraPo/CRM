using AutoMapper;
using CRM.APILayer.Attribites;
using CRM.APILayer.Extensions;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Services;
using Marvelous.Contracts;
using Marvelous.Contracts.Enums;
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
        private readonly IRequestHelper _requestHelper;


        public TransactionsController(ITransactionService transactionService, 
            IMapper autoMapper, 
            ILogger<TransactionsController> logger,
            IRequestHelper requestHelper)
        {
            _transactionService = transactionService;
            _mapper = autoMapper;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        // api/deposit/
        [HttpPost(TransactionUrls.Deposit)]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [SwaggerOperation("Add deposit Roles: Vip, Regular")]
        [SwaggerResponse(201, "Deposit added")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddDeposit([FromBody] TransactionRequestModel transaction)
        {
            this.CheckToken(_requestHelper);
            _logger.LogInformation($"Received a request to add a deposit to an account with ID = {transaction.AccountId}.");
            var leadId = this.GetLeadFromToken().Id;
            var response = await _transactionService.AddDeposit(transaction, leadId);
            _logger.LogInformation($"Successfully added deposit to account with ID = {transaction.AccountId}. Deposit ID = {response.Content}.");

            return StatusCode(201, response.Content);
        }

        // api/trsnsfer/
        [HttpPost(TransactionUrls.Transfer)]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [SwaggerOperation("Add transfer Roles: Vip, Regular")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddTransfer([FromBody] TransferRequestModel transaction)
        {
            this.CheckToken(_requestHelper);
            _logger.LogInformation($"Transfer request received from account with ID {transaction.AccountIdFrom} to account with ID {transaction.AccountIdTo}.");
            var leadId = this.GetLeadFromToken().Id;
            var response = await _transactionService.AddTransfer(transaction, leadId);
            _logger.LogInformation($"Successfully added transfer from account with ID {transaction.AccountIdFrom} to account with ID {transaction.AccountIdTo}. Transfer ID = {response.Content}.");
            return StatusCode(201, response.Content);
        }

        // api/withdraw/
        [HttpPost(TransactionUrls.Withdraw)]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [SwaggerOperation("Withdraw Roles: Vip, Regular")]
        [SwaggerResponse(201, "Withdraw successful")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Withdraw([FromBody] TransactionRequestModel transaction)
        {
            this.CheckToken(_requestHelper);
            _logger.LogInformation($"Received withdrawal request from account with ID = {transaction.AccountId}.");
            var leadId = this.GetLeadFromToken().Id;
            var response = await _transactionService.Withdraw(transaction, leadId);
            _logger.LogInformation($"Successfully passed the request for withdrawal of funds from the account with the ID {transaction.AccountId}. Withdraw ID = {response.Content}.");

            return StatusCode(201, response.Content);
        }


    }
}
