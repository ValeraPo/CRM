using AutoMapper;
using CRM.APILayer.Attribites;
using CRM.APILayer.Extensions;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts.Enums;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/accounts")]

    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _autoMapper;
        private readonly ILogger<AccountsController> _logger;
        private readonly ITransactionService _transactionService;
        private readonly ICRMProducers _crmProducers;

        public AccountsController(IAccountService accountService,
            IMapper autoMapper,
            ILogger<AccountsController> logger,
            ITransactionService transactionService,
            ICRMProducers crmProducers)
        {
            _accountService = accountService;
            _autoMapper = autoMapper;
            _logger = logger;
            _transactionService = transactionService;
            _crmProducers = crmProducers;
        }

        //api/accounts
        [HttpPost]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [SwaggerOperation("Add new account. Roles: Vip, Regular")]
        public async Task<ActionResult<int>> AddAccount([FromBody] AccountInsertRequest accountInsertRequest)
        {
            var leadIdentity = this.GetLeadFromToken();
            _logger.LogInformation($"A request was received to add an account as a lead with ID = {leadIdentity.Id}.");

            var accountModel = _autoMapper.Map<AccountModel>(accountInsertRequest);
            accountModel.Lead = new LeadModel();
            accountModel.Lead.Id = leadIdentity.Id;
            Role role = leadIdentity.Role;
            var id = await _accountService.AddAccount((int)role, accountModel);
            _logger.LogInformation($"Account with ID {id} successfully added");
            await _crmProducers.NotifyAccountAdded(accountModel);
            return StatusCode(StatusCodes.Status201Created, id);
        }

        //api/accounts/42
        [HttpPut("{id}")]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Update account by id. Roles: Vip, Regular")]
        public async Task<ActionResult> UpdateAccount(int id, [FromBody] AccountUpdateRequest accountUpdateRequest)
        {
            var leadIdentity = this.GetLeadFromToken();
            _logger.LogInformation($"A request was received to update an account with ID {id} as a lead with ID = {leadIdentity.Id}.");
            var accountModel = _autoMapper.Map<AccountModel>(accountUpdateRequest);
            var leadId = leadIdentity.Id;
            accountModel.Id = id;
            await _accountService.UpdateAccount(leadId, accountModel);
            _logger.LogInformation($"Account with ID {id} successfully updated.");
            await _crmProducers.NotifyAccountAdded(id);
            return Ok($"Account with id = {id} successfully updated.");
        }

        //api/accounts/42
        [HttpDelete("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Lock account by id. Roles: Admin")]
        public async Task<ActionResult> LockById(int id)
        {
            _logger.LogInformation($"A request was received to lock an account with ID {id} as a lead with ID = {this.GetLeadFromToken().Id}.");
            await _accountService.LockById(id);
            _logger.LogInformation($"Account with ID {id} successfully locked.");
            await _crmProducers.NotifyAccountAdded(id);
            return Ok($"Account with ID {id} successfully updated.");
        }

        //api/accounts/42
        [HttpPatch("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Unlock account by id. Roles: Admin")]
        public async Task<ActionResult> UnlockById(int id)
        {
            _logger.LogInformation($"A request was received to unlock an account with ID {id} as a lead with ID = {this.GetLeadFromToken().Id}.");
            await _accountService.UnlockById(id);
            _logger.LogInformation($"Account with ID {id} successfully unlocked.");
            await _crmProducers.NotifyAccountAdded(id);
            return Ok($"Account with ID {id} successfully unlocked.");
        }

        //api/accounts
        [HttpGet()]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [ProducesResponseType(typeof(List<AccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Get accounts by lead. Roles: Vip, Regular")]
        public async Task<ActionResult<List<AccountResponse>>> GetByLead()
        {
            var id = this.GetLeadFromToken().Id;
            _logger.LogInformation($"Request received to get all accounts by lead with ID = {id}");
            var accountModels = await _accountService.GetByLead(id);
            var outputs = _autoMapper.Map<List<AccountResponse>>(accountModels);
            foreach (var account in outputs)
                account.Balance = await _transactionService.GetBalance(account.Id);
            _logger.LogInformation($"All lead accounts with ID {id} have been successfully received");
            return Ok(outputs);
        }

        //api/accounts/42
        [HttpGet("{id}")]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("Get account by id. Roles: Vip, Regular")]
        public async Task<ActionResult<AccountResponse>> GetById(int id)
        {
            _logger.LogInformation($"A request was received to get an account with an ID {id} lead with an ID {this.GetLeadFromToken().Id}");
            var leadId =  this.GetLeadFromToken().Id;
            var accountModel = await _accountService.GetById(id, leadId);
            var output = _autoMapper.Map<AccountResponse>(accountModel);
            output.Balance = await _transactionService.GetBalance(id);
            _logger.LogInformation($"Account with ID = {id} successfully received");
            return Ok(output);
        }

        //api/transaction/42
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [HttpGet("transaction/{accountId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successful", typeof(ArrayList))]
        [SwaggerOperation("Get transactions by accountId. Roles: Vip, Regular")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ArrayList>> GetTransactionsByAccountId(int accountId)
        {
            _logger.LogInformation($"Poluchen zapros na poluchenie transakcii c accounta id = {accountId}");
            var leadId =  this.GetLeadFromToken().Id;
            var transactionModel = await _transactionService.GetTransactionsByAccountId(accountId, leadId);
            _logger.LogInformation($"Poluchenie transakcii c accounta id = {accountId} proshel uspeshno");

            return Ok(transactionModel.Content);
        }

        //api/accounts/42
        [HttpGet("balance/{CurrencyType}")]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [SwaggerResponse(StatusCodes.Status200OK, "Successful", typeof(decimal))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Get balance. Roles: Vip, Regular")]
        public async Task<ActionResult> GetBalance(int CurrencyType)
        {
            var leadIdentity = this.GetLeadFromToken();
            var leadId = leadIdentity.Id;
            _logger.LogInformation($"Poluchen zapros na polucheniie balance leada c id = {leadId}");
            var balance = await _accountService.GetBalance(leadId, (Currency)CurrencyType);
            _logger.LogInformation($"Balance dlya leada c id = {leadId} uspeshno poluchen.");
            return Ok(balance);
        }
    }
}
