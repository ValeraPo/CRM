using AutoMapper;
using CRM.APILayer.Extensions;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using FluentValidation;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/accounts")]

    public class AccountsController : AdvancedController
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _autoMapper;
        private readonly ILogger<AccountsController> _logger;
        private readonly ICRMProducers _crmProducers;
        private readonly IValidator<AccountInsertRequest> _validatorAccountInsertRequest;
        private readonly IValidator<AccountUpdateRequest> _validatorAccountUpdateRequest;

        public AccountsController(IAccountService accountService,
            IMapper autoMapper,
            ILogger<AccountsController> logger,
            ICRMProducers crmProducers,
            IRequestHelper requestHelper,
            IValidator<AccountInsertRequest> validatorAccountInsertRequest,
            IValidator<AccountUpdateRequest> validatorAccountUpdateRequest) : base(requestHelper, logger)
        {
            _accountService = accountService;
            _autoMapper = autoMapper;
            _logger = logger;
            _crmProducers = crmProducers;
            _validatorAccountInsertRequest = validatorAccountInsertRequest;
            _validatorAccountUpdateRequest = validatorAccountUpdateRequest;
        }

        // Adding new account
        //api/accounts
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation("Add new account. Roles: Vip, Regular")]
        public async Task<ActionResult<int>> AddAccount([FromBody] AccountInsertRequest accountInsertRequest)
        {
            var leadIdentity = GetIdentity(); // Checking token
            Validate(accountInsertRequest, _validatorAccountInsertRequest); //Validating data
            _logger.LogInformation($"A request was received to add an account as a lead with ID = {leadIdentity.Id}.");

            var accountModel = _autoMapper.Map<AccountModel>(accountInsertRequest);
            accountModel.Lead = new LeadModel();
            accountModel.Lead.Id = (int)leadIdentity.Id; //substituting Id from token
            var role = (Role)Enum.Parse(typeof(Role), leadIdentity.Role); // Parsing role
            var id = await _accountService.AddAccount(role, accountModel);
            _logger.LogInformation($"Account with ID {id} successfully added");
            accountModel.Id = id;
            await _crmProducers.NotifyAccountAdded(accountModel); // Senting new account to reporting service
            return StatusCode(StatusCodes.Status201Created, id);
        }

        // Updating account
        //api/accounts/42
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation("Update account by id. Roles: Vip, Regular")]
        public async Task<ActionResult> UpdateAccount(int id, [FromBody] AccountUpdateRequest accountUpdateRequest)
        {
            var leadIdentity = GetIdentity(); // Checking token
            CheckRole(leadIdentity, Role.Vip, Role.Regular); //Checking role
            Validate(accountUpdateRequest, _validatorAccountUpdateRequest); //Validating data
            _logger.LogInformation($"A request was received to update an account with ID {id} as a lead with ID = {leadIdentity.Id}.");
            var accountModel = _autoMapper.Map<AccountModel>(accountUpdateRequest);
            var leadId = (int)leadIdentity.Id;
            accountModel.Id = id;
            await _accountService.UpdateAccount(leadId, accountModel);
            _logger.LogInformation($"Account with ID {id} successfully updated.");
            await _crmProducers.NotifyAccountAdded(id); // Senting changed account to reporting service
            return Ok($"Account with id = {id} successfully updated.");
        }

        //Locking account
        //api/accounts/42
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Lock account by id. Roles: Admin")]
        public async Task<ActionResult> LockById(int id)
        {
            var leadIdentity = GetIdentity(); // Checking token
            CheckRole(leadIdentity, Role.Admin); //Checking role
            _logger.LogInformation($"A request was received to lock an account with ID {id} as a lead with ID = {leadIdentity.Id}.");
            await _accountService.LockById(id);
            _logger.LogInformation($"Account with ID {id} successfully locked.");
            await _crmProducers.NotifyAccountAdded(id); // Senting changed account to reporting service
            return Ok($"Account with ID {id} successfully updated.");
        }

        //Unlocking account
        //api/accounts/42
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Unlock account by id. Roles: Admin")]
        public async Task<ActionResult> UnlockById(int id)
        {
            var leadIdentity = GetIdentity(); // Checking token
            CheckRole(leadIdentity, Role.Admin); //Checking role
            _logger.LogInformation($"A request was received to unlock an account with ID {id} as a lead with ID = {leadIdentity.Id}.");
            await _accountService.UnlockById(id);
            _logger.LogInformation($"Account with ID {id} successfully unlocked.");
            await _crmProducers.NotifyAccountAdded(id); // Senting changed account to reporting service
            return Ok($"Account with ID {id} successfully unlocked.");
        }

        //Getting all lead's accounts
        //api/accounts
        [HttpGet()]
        [ProducesResponseType(typeof(List<AccountShortResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Get accounts by lead. Roles: Vip, Regular")]
        public async Task<ActionResult<List<AccountResponse>>> GetByLead()
        {
            var leadIdentity = GetIdentity(); // Checking token
            CheckRole(leadIdentity, Role.Vip, Role.Regular); //Checking role
            var id = (int)leadIdentity.Id;
            _logger.LogInformation($"Request received to get all accounts by lead with ID = {id}");
            var accountModels = await _accountService.GetByLead(id);
            var outputs = _autoMapper.Map<List<AccountShortResponse>>(accountModels);

            _logger.LogInformation($"All lead accounts with ID {id} have been successfully received");
            return Ok(outputs);
        }

        //Getting an account by id
        //api/accounts/42
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation("Get account by id. Roles: Vip, Regular")]
        public async Task<ActionResult<AccountResponse>> GetById(int id)
        {
            var leadIdentity = GetIdentity(); // Checking token
            CheckRole(leadIdentity, Role.Vip, Role.Regular); //Checking role
            _logger.LogInformation($"A request was received to get an account with an ID {id} lead with an ID {leadIdentity.Id}");
            var leadId = (int)leadIdentity.Id;
            var accountModel = await _accountService.GetById(id, leadId);
            var output = _autoMapper.Map<AccountResponse>(accountModel);

            _logger.LogInformation($"Account with ID = {id} successfully received");
            return Ok(output);
        }

        //Getting account's balance
        //api/accounts/42
        [HttpGet("balance/{CurrencyType}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successful", typeof(decimal))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Get balance. Roles: Vip, Regular")]
        public async Task<ActionResult> GetBalance(Currency CurrencyType)
        {
            var leadIdentity = GetIdentity(); // Checking token
            CheckRole(leadIdentity, Role.Vip, Role.Regular); //Checking role
            var leadId = (int)leadIdentity.Id;
            _logger.LogInformation($"Poluchen zapros na polucheniie balance leada c id = {leadId}");
            var balance = await _accountService.GetBalance(leadId, CurrencyType);
            _logger.LogInformation($"Balance dlya leada c id = {leadId} uspeshno poluchen.");
            return Ok(balance);
        }
    }
}
