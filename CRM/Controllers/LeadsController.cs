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

        //api/Leads
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponseModel), StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation("Create lead")]
        public async Task<ActionResult<int>> AddLead([FromBody] LeadInsertRequest leadInsertRequest)
        {
            Validate(leadInsertRequest, _validatorLeadInsertRequest);
            _logger.LogInformation($"Received a request to create a new lead.");
            var leadModel = _autoMapper.Map<LeadModel>(leadInsertRequest);
            var ids = await _leadService.AddLead(leadModel);
            var leadId = ids.Item1;
            var accountId = ids.Item2;
            leadModel.Id = leadId;
            _logger.LogInformation($"Lead successfully created. Received ID = {leadId}");
            await _crmProducers.NotifyLeadAdded(leadModel);
            await _crmProducers.NotifyAccountAdded(accountId);
            var data2FA =_leadService.GetData2FA( await _leadService.GetById(leadId));
            return StatusCode(StatusCodes.Status201Created, $"{leadId}, Data to 2FA: {data2FA}");
        }

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
            CheckRole(GetIdentity(), Role.Admin, Role.Vip, Role.Regular);
            Validate(leadUpdateRequest, _validatorLeadUpdateRequest);
            _logger.LogInformation($"Received a request to update lead with ID = {id}.");
            var leadModel = _autoMapper.Map<LeadModel>(leadUpdateRequest);
            leadModel.Id = id;
            await _leadService.UpdateLead(id, leadModel);
            _logger.LogInformation($"Lead successfully updated with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Lead successfully updated with ID = {id}.");
        }

        //api/Leads/42/2
        [HttpPut("{id}/role/{role}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Change lead's role by id. Roles: Admin")]
        public async Task<ActionResult> ChangeRoleLead(int id, Role role)
        {
            CheckRole(GetIdentity(), Role.Admin);
            _logger.LogInformation($"Received a request to update the role of the lead with ID = {id}.");
            await _leadService.ChangeRoleLead(id, role);
            _logger.LogInformation($"Successfully updated lead role with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Successfully updated lead role with ID = {id}.");
        }

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
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Lead successfully deleted with ID = {id}.");
        }

        //api/Leads/42
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Restore lead by id. Roles: Admin")]
        public async Task<ActionResult> RestoreById(int id)
        {
            CheckRole(GetIdentity(), Role.Admin);
            _logger.LogInformation($"Received a request to restore lead with ID = {id}.");
            await _leadService.RestoreById(id);
            _logger.LogInformation($"Lead successfully deleted with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Lead successfully restored with ID = {id}.");
        }

        //api/Leads/
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(List<LeadResponse>), StatusCodes.Status200OK)]
        [SwaggerOperation("Get all lead. Roles: Admin")]
        public async Task<ActionResult<List<LeadResponse>>> GetAll()
        {
            CheckRole(GetIdentity(), Role.Admin);
            _logger.LogInformation($"Received a request to receive all leads.");
            var leadModels = await _leadService.GetAll();
            var outputs = _autoMapper.Map<List<LeadResponse>>(leadModels);
            _logger.LogInformation($"All leads have been successfully received.");
            return Ok(outputs);
        }

        //api/Leads/auth/
        [HttpGet(CrmEndpoints.Auth)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(List<LeadAuthExchangeModel>), StatusCodes.Status200OK)]
        [SwaggerOperation("Get all lead. Roles: all")]
        public async Task<ActionResult<List<LeadAuthExchangeModel>>> GetAllToAuth()
        {
            var leadIdentity = GetIdentity();
            if (!CheckMicroservice(leadIdentity, Microservice.MarvelousAuth))
                CheckRole(leadIdentity, Role.Admin);
            _logger.LogInformation($"Received a request to receive all leads.");
            var leadModels = await _leadService.GetAllToAuth();
            _logger.LogInformation($"All leads have been successfully received.");
            return Ok(leadModels);
        }

        //api/Leads/42
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Get lead by id. Roles: All")]
        public async Task<ActionResult<LeadResponse>> GetById(int id)
        {
            var leadIdentity = GetIdentity();
            CheckRole(leadIdentity, Role.Admin, Role.Vip, Role.Regular);
            _logger.LogInformation($"Received to get an lead with an ID {id}.");
            var leadModel = await _leadService.GetById(id, leadIdentity);
            var output = _autoMapper.Map<LeadResponse>(leadModel);
            _logger.LogInformation($"Successfully received a lead with ID = {id}.");
            return Ok(output);
        }

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
            var leadIdentity = GetIdentity();
            CheckRole(leadIdentity, Role.Admin, Role.Vip, Role.Regular);
            Validate(changePasswordRequest, _validatorLeadChangePasswordRequest);
            var id = (int)leadIdentity.Id;
            _logger.LogInformation($"Received a request to change the password of a lead with an ID = {id}.");
            await _leadService.ChangePassword(id, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            _logger.LogInformation($"Successfully changed the password of the lead with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id);
            Data2FAModel data2FA = await _leadService.GetData2FA(await _leadService.GetById(id));
            return Ok($"Data to 2FA: {data2FA.LeadId}, {data2FA.EncodedKey}");
        }

    }
}
