using AutoMapper;
using CRM.APILayer.Extensions;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using FluentValidation;
using Marvelous.Contracts.Endpoints;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ExchangeModels;
using Marvelous.Contracts.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route(CrmEndpoints.LeadApi)]

    public class LeadsController : AdvancedController
    {
        private readonly ILeadService _leadService;
        private readonly IMapper _autoMapper;
        private readonly ILogger<LeadsController> _logger;
        private readonly ICRMProducers _crmProducers;
        private readonly IValidator<LeadInsertRequest> _validatorLeadInsertRequest;
        private readonly IValidator<LeadUpdateRequest> _validatorLeadUpdateRequest;
        private readonly IValidator<LeadChangePasswordRequest> _validatorLeadChangePasswordRequest;

        public LeadsController(ILeadService leadService,
            IMapper autoMapper,
            ILogger<LeadsController> logger,
            ICRMProducers crmProducers,
            IRequestHelper requestHelper,
            IValidator<LeadInsertRequest> validatorLeadInsertRequest,
            IValidator<LeadUpdateRequest> validatorLeadUpdateRequest,
            IValidator<LeadChangePasswordRequest> validatorLeadChangePasswordRequest) : base(requestHelper, logger)
        {
            _leadService = leadService;
            _autoMapper = autoMapper;
            _logger = logger;
            _crmProducers = crmProducers;
            _validatorLeadInsertRequest = validatorLeadInsertRequest;
            _validatorLeadUpdateRequest = validatorLeadUpdateRequest;
            _validatorLeadChangePasswordRequest = validatorLeadChangePasswordRequest;

        }

        //Adding lead
        //api/Leads
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation("Create lead")]
        public async Task<ActionResult<int>> AddLead([FromBody] LeadInsertRequest leadInsertRequest)
        {
            Validate(leadInsertRequest, _validatorLeadInsertRequest); //Validating data
            _logger.LogInformation($"Received a request to create a new lead.");
            var leadModel = _autoMapper.Map<LeadModel>(leadInsertRequest);
            var ids = await _leadService.AddLead(leadModel); // (leadId, accountId)
            var leadId = ids.Item1;
            var accountId = ids.Item2;
            _logger.LogInformation($"Lead successfully created. Received ID = {leadId}");
            await _crmProducers.NotifyLeadAdded(leadModel); // Senting new lead to reporting service
            await _crmProducers.NotifyAccountAdded(accountId); // Senting new account to reporting service
            return StatusCode(StatusCodes.Status201Created, leadId);
        }

        //Updating lead
        //api/Leads/42
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation("Update lead by id. Roles: All")]
        public async Task<ActionResult> UpdateLead(int id, [FromBody] LeadUpdateRequest leadUpdateRequest)
        {
            CheckRole(GetIdentity(), Role.Admin, Role.Vip, Role.Regular); //Checking role
            Validate(leadUpdateRequest, _validatorLeadUpdateRequest); //Validating data
            _logger.LogInformation($"Received a request to update lead with ID = {id}.");
            var leadModel = _autoMapper.Map<LeadModel>(leadUpdateRequest);
            leadModel.Id = id;
            await _leadService.UpdateLead(id, leadModel);
            _logger.LogInformation($"Lead successfully updated with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id); // Senting changed lead to reporting service
            return Ok($"Lead successfully updated with ID = {id}.");
        }

        // Changing lead's role
        //api/Leads/42/2
        [HttpPut("{id}/role/{role}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Change lead's role by id. Roles: Admin")]
        public async Task<ActionResult> ChangeRoleLead(int id, Role role)
        {
            CheckRole(GetIdentity(), Role.Admin); //Checking role
            _logger.LogInformation($"Received a request to update the role of the lead with ID = {id}.");
            await _leadService.ChangeRoleLead(id, role);
            _logger.LogInformation($"Successfully updated lead role with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id); // Senting changed lead to reporting service
            return Ok($"Successfully updated lead role with ID = {id}.");
        }

        // Deleting lead
        //api/Leads/42
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Delete lead by id. Roles: Admin")]
        public async Task<ActionResult> DeleteById(int id)
        {
            CheckRole(GetIdentity(), Role.Admin);
            _logger.LogInformation($"Received a request to delete lead with ID = {id}.");
            await _leadService.DeleteById(id);
            _logger.LogInformation($"Lead successfully deleted with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id); // Senting changed lead to reporting service
            return Ok($"Lead successfully deleted with ID = {id}.");
        }

        //Restoring lead
        //api/Leads/42
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Restore lead by id. Roles: Admin")]
        public async Task<ActionResult> RestoreById(int id)
        {
            CheckRole(GetIdentity(), Role.Admin); //Checking role
            _logger.LogInformation($"Received a request to restore lead with ID = {id}.");
            await _leadService.RestoreById(id);
            _logger.LogInformation($"Lead successfully deleted with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id); // Senting changed lead to reporting service
            return Ok($"Lead successfully restored with ID = {id}.");
        }

        // Getting all leads
        //api/Leads/
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(List<LeadResponse>), StatusCodes.Status200OK)]
        [SwaggerOperation("Get all lead. Roles: Admin")]
        public async Task<ActionResult<List<LeadResponse>>> GetAll()
        {
            CheckRole(GetIdentity(), Role.Admin); //Checking role
            _logger.LogInformation($"Received a request to receive all leads.");
            var leadModels = await _leadService.GetAll(); // List<LeadModel>
            var outputs = _autoMapper.Map<List<LeadResponse>>(leadModels);
            _logger.LogInformation($"All leads have been successfully received.");
            return Ok(outputs);
        }

        // Getting all real leads
        //api/Leads/auth/
        [HttpGet(CrmEndpoints.Auth)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(List<LeadAuthExchangeModel>), StatusCodes.Status200OK)]
        [SwaggerOperation("Get all lead. Roles: all")]
        public async Task<ActionResult<List<LeadAuthExchangeModel>>> GetAllToAuth()
        {
            var leadIdentity = GetIdentity(); // Checking token
            //Checking authorization service ir role 
            if (!CheckMicroservice(leadIdentity, Microservice.MarvelousAuth))
                CheckRole(leadIdentity, Role.Admin);
            _logger.LogInformation($"Received a request to receive all leads.");
            var leadModels = await _leadService.GetAllToAuth(); // List<LeadAuthExchangeModel>
            _logger.LogInformation($"All leads have been successfully received.");
            return Ok(leadModels);
        }

        // Getting a lead by Id
        //api/Leads/42
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Get lead by id. Roles: All")]
        public async Task<ActionResult<LeadResponse>> GetById(int id)
        {
            var leadIdentity = GetIdentity(); // Checking token
            CheckRole(leadIdentity, Role.Admin, Role.Vip, Role.Regular); //Checking role
            _logger.LogInformation($"Received to get an lead with an ID {id}.");
            var leadModel = await _leadService.GetById(id, leadIdentity); // LeadModel
            var output = _autoMapper.Map<LeadResponse>(leadModel);
            _logger.LogInformation($"Successfully received a lead with ID = {id}.");
            return Ok(output);
        }

        // Changing password
        //api/Leads/password
        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation("Change lead password. Roles: All")]
        public async Task<ActionResult> ChangePassword([FromBody] LeadChangePasswordRequest changePasswordRequest)
        {
            var leadIdentity = GetIdentity(); // Checking token
            CheckRole(leadIdentity, Role.Admin, Role.Vip, Role.Regular); //Checking role
            Validate(changePasswordRequest, _validatorLeadChangePasswordRequest); //Validating data
            var id = (int)leadIdentity.Id;
            _logger.LogInformation($"Received a request to change the password of a lead with an ID = {id}.");
            await _leadService.ChangePassword(id, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            _logger.LogInformation($"Successfully changed the password of the lead with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id); // Senting changed lead to reporting service
            return Ok();
        }

    }
}
