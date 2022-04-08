using AutoMapper;
using CRM.APILayer.Attribites;
using CRM.APILayer.Extensions;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ExchangeModels;
using Marvelous.Contracts.Urls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route(CrmUrls.LeadApi)]

    public class LeadsController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly IMapper _autoMapper;
        private readonly ILogger<LeadsController> _logger;
        private readonly ICRMProducers _crmProducers;


        public LeadsController(ILeadService leadService,
            IMapper autoMapper,
            ILogger<LeadsController> logger,
            ICRMProducers crmProducers)
        {
            _leadService = leadService;
            _autoMapper = autoMapper;
            _logger = logger;
            _crmProducers = crmProducers;
        }

        //api/Leads
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [SwaggerOperation("Create lead")]
        public async Task<ActionResult<int>> AddLead([FromBody] LeadInsertRequest leadInsertRequest)
        {
            _logger.LogInformation($"Received a request to create a new lead.");
            var leadModel = _autoMapper.Map<LeadModel>(leadInsertRequest);
            var ids = await _leadService.AddLead(leadModel);
            var leadId = ids.Item1;
            var accountId = ids.Item2;
            leadModel.Id = leadId;
            _logger.LogInformation($"Lead successfully created. Received ID = {leadId}");
            await _crmProducers.NotifyLeadAdded(leadModel);
            await _crmProducers.NotifyAccountAdded(accountId);
            return StatusCode(StatusCodes.Status201Created, leadId);
        }

        //api/Leads/42
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Update lead by id. Roles: All")]
        public async Task<ActionResult> UpdateLead(int id, [FromBody] LeadUpdateRequest leadUpdateRequest)
        {
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
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Change lead's role by id. Roles: Admin")]
        public async Task<ActionResult> ChangeRoleLead(int id, int role)
        {
            _logger.LogInformation($"Received a request to update the role of the lead with ID = {id}.");
            await _leadService.ChangeRoleLead(id, (Role)role);
            _logger.LogInformation($"Successfully updated lead role with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Successfully updated lead role with ID = {id}.");
        }

        //api/Leads/42
        [HttpDelete("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Delete lead by id. Roles: Admin")]
        public async Task<ActionResult> DeleteById(int id)
        {
            _logger.LogInformation($"Received a request to delete lead with ID = {id}.");
            await _leadService.DeleteById(id);
            _logger.LogInformation($"Lead successfully deleted with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Lead successfully deleted with ID = {id}.");
        }

        //api/Leads/42
        [HttpPatch("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Restore lead by id. Roles: Admin")]
        public async Task<ActionResult> RestoreById(int id)
        {
            _logger.LogInformation($"Received a request to restore lead with ID = {id}.");
            await _leadService.RestoreById(id);
            _logger.LogInformation($"Lead successfully deleted with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Lead successfully deleted with ID = {id}.");
        }

        //api/Leads/
        [HttpGet()]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(typeof(List<LeadResponse>), StatusCodes.Status200OK)]
        [SwaggerOperation("Get all lead. Roles: Admin")]
        public async Task<ActionResult<List<LeadResponse>>> GetAll()
        {
            _logger.LogInformation($"Received a request to receive all leads.");
            var leadModels = await _leadService.GetAll();
            var outputs = _autoMapper.Map<List<LeadResponse>>(leadModels);
            _logger.LogInformation($"All leads have been successfully received.");
            return Ok(outputs);
        }

        //api/Leads/auth
        [HttpGet(CrmUrls.Auth)]
        [ProducesResponseType(typeof(List<LeadResponse>), StatusCodes.Status200OK)]
        [SwaggerOperation("Get all lead. Roles: all")]
        public async Task<ActionResult<List<LeadAuthExchangeModel>>> GetAllToAuth()
        {
            _logger.LogInformation($"Poluchen zapros na poluchenie vseh leadov.");
            var leadModels = await _leadService.GetAllToAuth();
            _logger.LogInformation($"Vse leady uspeshno polucheny.");
            return Ok(leadModels);
        }

        //api/Leads/42
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Get lead by id. Roles: Admin")]
        public async Task<ActionResult<LeadResponse>> GetById(int id)
        {
            _logger.LogInformation($"Received to get an lead with an ID {id}.");
            var leadModel = await _leadService.GetById(id);
            var output = _autoMapper.Map<LeadResponse>(leadModel);
            _logger.LogInformation($"Successfully received a lead with ID = {id}.");
            return Ok(output);
        }

        //api/Leads/password
        [HttpPut("password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Change lead password. Roles: All")]
        public async Task<ActionResult> ChangePassword([FromBody] LeadChangePasswordRequest changePasswordRequest)
        {
            var id = this.GetLeadFromToken().Id;
            _logger.LogInformation($"Received a request to change the password of a lead with an ID = {id}.");
            await _leadService.ChangePassword(id, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            _logger.LogInformation($"Successfully changed the password of the lead with ID = {id}.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok();
        }


        
        //[HttpPut]
        //[SwaggerOperation("Change lead password. Roles: All")]
        //public async Task<ActionResult> ChangeRoleTemp([FromBody] List<LeadShortExchangeModel> leadChangeRoleRequests)
        //{
        //    await _leadService.ChangeRoleListLead(leadChangeRoleRequests);
        //    return Ok();
        //}

    }
}
