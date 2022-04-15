using AutoMapper;
using CRM.APILayer.Extensions;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Services;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.APILayer.Controllers
{
    public class TransactionsController : AdvancedController
    {
        private readonly ITransactionService _transactionService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<TransactionsController> _logger;
        private readonly IRequestHelper _requestHelper;
        private readonly IConfiguration _config;


        public TransactionsController(ITransactionService transactionService,
            IInvoiceService invoiceService,
            ILogger<TransactionsController> logger,
            IRequestHelper requestHelper,
            IConfiguration configuration) : base(configuration, requestHelper)
        {
            _transactionService = transactionService;
            _invoiceService = invoiceService;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        // api/deposit/
        [HttpPost("deposit")]
        [SwaggerOperation("Creates invoice on paypal to make deposit. Returns link on paypal site. Roles: Vip, Regular")]
        [SwaggerResponse(201, "Deposit added")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddDeposit([FromBody] TransactionRequestModel transaction)
        {
            await CheckRole(Role.Vip, Role.Regular);
            _logger.LogInformation($"Received a request to add a deposit to an account with ID = {transaction.AccountId}.");
            var leadId = (int)(await GetIdentity()).Id;
            var linkToPay = await _invoiceService.GetNewInvoiceUrlToPay(transaction, leadId);
            _logger.LogInformation($"Successfully created invoice on paypal to deposit on account with ID = {transaction.AccountId}. Link to make payment = {linkToPay}");

            return StatusCode(201, linkToPay);
        }

        // api/transfer/
        [HttpPost("transfer")]
        [SwaggerOperation("Add transfer Roles: Vip, Regular")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddTransfer([FromBody] TransferRequestModel transaction)
        {
            await CheckRole(Role.Vip, Role.Regular);
            _logger.LogInformation($"Transfer request received from account with ID {transaction.AccountIdFrom} to account with ID {transaction.AccountIdTo}.");
            var leadId = (int)(await GetIdentity()).Id;
            var response = await _transactionService.AddTransfer(transaction, leadId);
            _logger.LogInformation($"Successfully added transfer from account with ID {transaction.AccountIdFrom} to account with ID {transaction.AccountIdTo}. Transfer ID = {response.Content}.");
            return StatusCode(201, response.Content);
        }

        // api/withdraw/
        [HttpPost("withdraw")]
        [SwaggerOperation("Withdraw Roles: Vip, Regular")]
        [SwaggerResponse(201, "Withdraw successful")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Withdraw([FromBody] TransactionRequestModel transaction)
        {
            await CheckRole(Role.Vip, Role.Regular);
            _logger.LogInformation($"Received withdrawal request from account with ID = {transaction.AccountId}.");
            var leadId = (int)(await GetIdentity()).Id;
            var response = await _transactionService.Withdraw(transaction, leadId);
            _logger.LogInformation($"Successfully passed the request for withdrawal of funds from the account with the ID {transaction.AccountId}. Withdraw ID = {response.Content}.");

            return StatusCode(201, response.Content);
        }


    }
}
