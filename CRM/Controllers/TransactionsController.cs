﻿using AutoMapper;
using CRM.APILayer.Extensions;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
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
        private readonly ICRMProducers _crmProducers;
        private readonly IMapper _autoMapper;


        public TransactionsController(ITransactionService transactionService, 
            ILogger<TransactionsController> logger,
            IMapper autoMapper,
            IRequestHelper requestHelper,
            ICRMProducers crmProducers) : base(requestHelper, logger)
        {
            _transactionService = transactionService;
            _logger = logger;
            _autoMapper = autoMapper;
            _crmProducers = crmProducers;
        }

        // api/transactions/deposit/
        [HttpPost("deposit")]
        [SwaggerOperation("Add deposit Roles: Vip, Regular")]
        [SwaggerResponse(201, "Deposit added")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddDeposit([FromBody] TransactionShortRequest transaction)
        {
            var leadIdentity = GetIdentity();
            CheckRole(leadIdentity, Role.Vip, Role.Regular);
            _logger.LogInformation($"Received a request to add a deposit to an account with ID = {transaction.AccountId}.");
            var leadId = (int)leadIdentity.Id;
            var transactionModel = _autoMapper.Map<TransactionRequestModel>(transaction);
            var response = await _transactionService.AddDeposit(transactionModel, leadId);
            _logger.LogInformation($"Successfully added deposit to account with ID = {transaction.AccountId}. Deposit ID = {response}.");
            await _crmProducers.AmmountCommissionForTransactionAdded(response);
            return StatusCode(201, response);
        }

        // api/transactions/trsnsfer/
        [HttpPost("transfer")]
        [SwaggerOperation("Add transfer Roles: Vip, Regular")]
        [SwaggerResponse(201, "List transactions by accountId ")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddTransfer([FromBody] TransferShortRequest transaction)
        {
            var leadIdentity = GetIdentity();
            CheckRole(leadIdentity, Role.Vip, Role.Regular);
            _logger.LogInformation($"Transfer request received from account with ID {transaction.AccountIdFrom} to account with ID {transaction.AccountIdTo}.");
            var leadId = (int)leadIdentity.Id;
            var transactionModel = _autoMapper.Map<TransferRequestModel>(transaction);
            var response = await _transactionService.AddTransfer(transactionModel, leadId);
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
        public async Task<ActionResult> Withdraw([FromBody] TransactionShortRequest transaction)
        {
            var leadIdentity = GetIdentity();
            CheckRole(leadIdentity,Role.Vip, Role.Regular);
            _logger.LogInformation($"Received withdrawal request from account with ID = {transaction.AccountId}.");
            var transactionModel = _autoMapper.Map<TransactionRequestModel>(transaction);
            var response = _transactionService.Withdraw(transactionModel);
            _logger.LogInformation($"Withdraw accepted to account ID = {transaction.AccountId}. Redirect to WithdrawApprove {response.Result}");
            return StatusCode(201, $"Redirect to Post {response.Result} AccessTime 10 minutes");
            
        }

        // api/transactions/withdraw/{tmpId}/approve
        [HttpPost("withdraw/{tmpId}/approve")]
        [SwaggerOperation("Withdraw Roles: Vip, Regular")]
        [SwaggerResponse(201, "Withdraw Approved successful")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> WithdrawApprove(int tmpId,[FromBody] int pin2FA)
        {
            var leadIdentity= GetIdentity();
            var leadId = (int)leadIdentity.Id;
            CheckRole(leadIdentity, Role.Vip, Role.Regular);
            var response = await _transactionService.WithdrawApproved(tmpId, leadId, pin2FA);
            _logger.LogInformation($"Successfully passed the request for withdrawal of funds from the ID transaction = {response.Item1.IdTransaction}.");
            await _crmProducers.AmmountCommissionForTransactionAdded(response.Item1);
            await _crmProducers.NotifyWhithdraw(leadId, response.Item2);
            return StatusCode(201, $"Id transaction {response.Item1.IdTransaction}, commission for her {response.Item1.AmountComission}");
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
