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
            _logger.LogInformation($"Poluchen zapros na dobavlenie accounta leadom c id = {leadIdentity.Id}.");

            var accountModel = _autoMapper.Map<AccountModel>(accountInsertRequest);
            accountModel.Lead = new LeadModel();
            accountModel.Lead.Id = leadIdentity.Id;
            Role role = leadIdentity.Role;
            var id = await _accountService.AddAccount((int)role, accountModel);
            _logger.LogInformation($"Account с id = {id} uspeshno dobavlen.");
            accountModel.Id = id;
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
            _logger.LogInformation($"Poluchen zapros na obnovlenie accounta id = {id} leadom c id = {leadIdentity.Id}.");
            var accountModel = _autoMapper.Map<AccountModel>(accountUpdateRequest);
            var leadId = leadIdentity.Id;
            accountModel.Id = id;
            await _accountService.UpdateAccount(leadId, accountModel);
            _logger.LogInformation($"Account c id = {id} uspeshno obnovlen.");
            await _crmProducers.NotifyAccountAdded(id);
            return Ok($"Account with id = {id} was updated");
        }

        //api/accounts/42
        [HttpDelete("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Lock account by id. Roles: Admin")]
        public async Task<ActionResult> LockById(int id)
        {
            _logger.LogInformation($"Poluchen zapros na blokirovku accounta id = {id} leadom c id = {this.GetLeadFromToken().Id}.");
            await _accountService.LockById(id);
            _logger.LogInformation($"Account с id = {id} uspeshno zablokirovan.");
            await _crmProducers.NotifyAccountAdded(id);
            return Ok($"Account with id = {id} was locked");
        }

        //api/accounts/42
        [HttpPatch("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Unlock account by id. Roles: Admin")]
        public async Task<ActionResult> UnlockById(int id)
        {
            _logger.LogInformation($"Poluchen zapros na  razblokirovku accounta id = {id} leadom c id = {this.GetLeadFromToken().Id}.");
            await _accountService.UnlockById(id);
            _logger.LogInformation($"Account c id = {id} uspeshno razblokirovan.");
            await _crmProducers.NotifyAccountAdded(id);
            return Ok($"Account with id = {id} was unlocked");
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
            _logger.LogInformation($"Poluchen zapros na  poluchenie vseh accountov leadom c id = {id}.");
            var accountModels = await _accountService.GetByLead(id);
            var outputs = _autoMapper.Map<List<AccountResponse>>(accountModels);
            foreach (var account in outputs)
                account.Balance = await _transactionService.GetBalance(account.Id);
            _logger.LogInformation($"Vse accounty leada c id = {id} uspeshno polucheny.");
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
            _logger.LogInformation($"Poluchen zapros na poluchenie accounta c id = {id} leadom c id = {id}.");
            var leadId = this.GetLeadFromToken().Id;
            var accountModel = await _accountService.GetById(id, leadId);
            var output = _autoMapper.Map<AccountResponse>(accountModel);
            output.Balance = await _transactionService.GetBalance(id);
            _logger.LogInformation($"Account c id = {id} uspeshno poluchen.");
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
    }
}
