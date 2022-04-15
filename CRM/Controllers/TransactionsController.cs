using CRM.APILayer.Extensions;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Services;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("/api/transactions")]

    public class TransactionsController : AdvancedController
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;


        public TransactionsController(ITransactionService transactionService, 
            ILogger<TransactionsController> logger,
            IRequestHelper requestHelper) : base(requestHelper, logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        // api/transactions/deposit/
        [HttpPost("deposit")]
        [SwaggerOperation("Add deposit Roles: Vip, Regular")]
        [SwaggerResponse(201, "Deposit added")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddDeposit([FromBody] TransactionRequestModel transaction)
        {
            var leadIdentity = GetIdentity();
            CheckRole(leadIdentity, Role.Vip, Role.Regular);
            _logger.LogInformation($"Received a request to add a deposit to an account with ID = {transaction.AccountId}.");
            var leadId = (int)leadIdentity.Id;
            var response = await _transactionService.AddDeposit(transaction, leadId);
            _logger.LogInformation($"Successfully added deposit to account with ID = {transaction.AccountId}. Deposit ID = {response}.");

            return StatusCode(201, response);
        }

        // api/transactions/trsnsfer/
        [HttpPost("transfer")]
        [SwaggerOperation("Add transfer Roles: Vip, Regular")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddTransfer([FromBody] TransferRequestModel transaction)
        {
            var leadIdentity = GetIdentity();
            CheckRole(leadIdentity, Role.Vip, Role.Regular);
            _logger.LogInformation($"Transfer request received from account with ID {transaction.AccountIdFrom} to account with ID {transaction.AccountIdTo}.");
            var leadId = (int)leadIdentity.Id;
            var response = await _transactionService.AddTransfer(transaction, leadId);
            _logger.LogInformation($"Successfully added transfer from account with ID {transaction.AccountIdFrom} to account with ID {transaction.AccountIdTo}. Transfer ID = {response}.");
            return StatusCode(201, response);
        }

        // api/transactions/withdraw/
        [HttpPost("withdraw")]
        [SwaggerOperation("Withdraw Roles: Vip, Regular")]
        [SwaggerResponse(201, "Withdraw successful")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Withdraw([FromBody] TransactionRequestModel transaction)
        {
            var leadIdentity = GetIdentity();
            CheckRole(leadIdentity,Role.Vip, Role.Regular);
            _logger.LogInformation($"Received withdrawal request from account with ID = {transaction.AccountId}.");
            var leadId = (int)leadIdentity.Id;
            var response = await _transactionService.Withdraw(transaction, leadId);
            _logger.LogInformation($"Successfully passed the request for withdrawal of funds from the account with the ID {transaction.AccountId}. Withdraw ID = {response}.");

            return StatusCode(201, response);
        }

        //api/transactions/42
        [HttpGet("{accountId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successful", typeof(string))]
        [SwaggerOperation("Get transactions by accountId. Roles: Vip, Regular")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetTransactionsByAccountId(int accountId)
        {
            var leadIdentity = GetIdentity();
            CheckRole(leadIdentity, Role.Vip, Role.Regular);
            _logger.LogInformation($"Poluchen zapros na poluchenie transakcii c accounta id = {accountId}");
            var leadId = (int)leadIdentity.Id;
            var transactionModel = await _transactionService.GetTransactionsByAccountId(accountId, leadId);
            _logger.LogInformation($"Poluchenie transakcii c accounta id = {accountId} proshel uspeshno");

            return Ok(transactionModel);
        }
    }
}
