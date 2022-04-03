using AutoMapper;
using CRM.APILayer.Attribites;
using CRM.APILayer.Extensions;
using CRM.APILayer.Models;
using CRM.APILayer.Producers;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ExchangeModels;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.Urls;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route(CrmUrls.Api)]

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
            _logger.LogInformation($"Poluchen zapros na sozdanie leada.");
            var leadModel = _autoMapper.Map<LeadModel>(leadInsertRequest);
            var id = await _leadService.AddLead(leadModel);
            _logger.LogInformation($"Lead с id = {id} uspeshno sozdan.");
            leadModel.Id = id;
            await _crmProducers.NotifyLeadAdded(leadModel);
            return StatusCode(StatusCodes.Status201Created, id);
        }

        //api/Leads/42
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Update lead by id. Roles: All")]
        public async Task<ActionResult> UpdateLead(int id, [FromBody] LeadUpdateRequest leadUpdateRequest)
        {
            _logger.LogInformation($"Poluchen zapros na izmenenie leada c id = {id}.");
            var leadModel = _autoMapper.Map<LeadModel>(leadUpdateRequest);
            leadModel.Id = id;
            await _leadService.UpdateLead(id, leadModel);
            _logger.LogInformation($"Lead c id = {id} uspeshno obnovlen.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Lead with id = {id} was updated");
        }

        //api/Leads/42/2
        [HttpPut("{id}/role/{role}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Change lead's role by id. Roles: Admin")]
        public async Task<ActionResult> ChangeRoleLead(int id, int role)
        {
            _logger.LogInformation($"Poluchen zapros na izmenenie roly leada c id = {id}.");
            await _leadService.ChangeRoleLead(id, (Role)role);
            _logger.LogInformation($"Lead c id = {id} uspeshno obnovlen.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Lead with id = {id} was updated");
        }

        //api/Leads/42
        [HttpDelete("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Delete lead by id. Roles: Admin")]
        public async Task<ActionResult> DeleteById(int id)
        {
            _logger.LogInformation($"Poluchen zapros na udalenie leada c id = {id}.");
            await _leadService.DeleteById(id);
            _logger.LogInformation($"Lead c id = {id} uspeshno udalen.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Lead with id = {id} was deleted");
        }

        //api/Leads/42
        [HttpPatch("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Restore lead by id. Roles: Admin")]
        public async Task<ActionResult> RestoreById(int id)
        {
            _logger.LogInformation($"Poluchen zapros na vosstanovlenie leada c id = {id}.");
            await _leadService.RestoreById(id);
            _logger.LogInformation($"Lead c id = {id} uspeshno vosstanovlen.");
            await _crmProducers.NotifyLeadAdded(id);
            return Ok($"Lead with id = {id} was restored");
        }

        //api/Leads/
        [HttpGet()]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(typeof(List<LeadResponse>), StatusCodes.Status200OK)]
        [SwaggerOperation("Get all lead. Roles: Admin")]
        public async Task<ActionResult<List<LeadResponse>>> GetAll()
        {
            _logger.LogInformation($"Poluchen zapros na poluchenie vseh leadov.");
            var leadModels = await _leadService.GetAll();
            var outputs = _autoMapper.Map<List<LeadResponse>>(leadModels);
            _logger.LogInformation($"Vse leady uspeshno polucheny.");
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
            _logger.LogInformation($"Poluchen zapros na poluchenie leada c id = {id}.");
            var leadModel = await _leadService.GetById(id);
            var output = _autoMapper.Map<LeadResponse>(leadModel);
            _logger.LogInformation($"Lead c id = {id} uspeshno poluchen.");
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
            _logger.LogInformation($"Poluchen zapros na izmenenie parolya leada c id = {id}.");
            await _leadService.ChangePassword(id, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            _logger.LogInformation($"Parol' leada c id = {id} uspeshno izmenen.");
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
