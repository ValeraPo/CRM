using AutoMapper;
using CRM.APILayer.Attribites;
using CRM.APILayer.Extensions;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using Marvelous.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/leads")]

    public class LeadsController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly IMapper _autoMapper;
        private readonly ILogger<LeadsController> _logger;


        public LeadsController(ILeadService leadService, IMapper autoMapper, ILogger<LeadsController> logger)
        {
            _leadService = leadService;
            _autoMapper = autoMapper;
            _logger = logger;
        }

        //api/Leads
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [SwaggerOperation("Create lead")]
        public ActionResult<int> AddLead([FromBody] LeadInsertRequest leadInsertRequest)
        {
            _logger.LogInformation($"Получен запрос на создание лида.");
            var leadModel = _autoMapper.Map<LeadModel>(leadInsertRequest);
            var id = _leadService.AddLead(leadModel);
            _logger.LogInformation($"Лид с id = {id} успешно создан.");
            return StatusCode(StatusCodes.Status201Created, id);
        }

        //api/Leads/42
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Update lead by id. Roles: All")]
        public ActionResult UpdateLead(int id, [FromBody] LeadUpdateRequest leadUpdateRequest)
        {
            _logger.LogInformation($"Получен запрос изменение лида с id = {id}.");
            var leadModel = _autoMapper.Map<LeadModel>(leadUpdateRequest);
            leadModel.Id = id;
            _leadService.UpdateLead(id, leadModel);
            _logger.LogInformation($"Лид с id = {id} успешно обновлен.");
            return Ok($"Lead with id = {id} was updated");
        }

        //api/Leads/42/2
        [HttpPut("{id}/{role}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Change lead's role by id. Roles: All")]
        public ActionResult ChangeRoleLead(int id, int role)
        {
            _logger.LogInformation($"Получен запрос изменение роли лида с id = {id}.");
            _leadService.ChangeRoleLead(id, role);
            _logger.LogInformation($"Лид с id = {id} успешно обновлен.");
            return Ok($"Lead with id = {id} was updated");
        }

        //api/Leads/42
        [HttpDelete("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Delete lead by id. Roles: Admin")]
        public ActionResult DeleteById(int id)
        {
            _logger.LogInformation($"Получен запрос на удаление лида с id = {id}.");
            _leadService.DeleteById(id);
            _logger.LogInformation($"Лид с id = {id} успешно удален.");
            return Ok($"Lead with id = {id} was deleted");
        }

        //api/Leads/42
        [HttpPatch("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Restore lead by id. Roles: Admin")]
        public ActionResult RestoreById(int id)
        {
            _logger.LogInformation($"Получен запрос на восстановление лида с id = {id}.");
            _leadService.RestoreById(id);
            _logger.LogInformation($"Лид с id = {id} успешно восстановлен.");
            return Ok($"Lead with id = {id} was restored");
        }

        //api/Leads/
        [HttpGet()]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(typeof(List<LeadResponse>), StatusCodes.Status200OK)]
        [SwaggerOperation("Restore all lead. Roles: Admin")]
        public ActionResult<List<LeadResponse>> GetAll()
        {
            _logger.LogInformation($"Получен запрос на получение всех лидов.");
            var leadModels = _leadService.GetAll();
            var outputs = _autoMapper.Map<List<LeadResponse>>(leadModels);
            _logger.LogInformation($"Все лиды успешно получены.");
            return Ok(outputs);
        }

        //api/Leads/42
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation("Get lead by id. Roles: Admin")]
        public ActionResult<LeadResponse> GetById(int id)
        {
            _logger.LogInformation($"Получен запрос на получение лида с id = {id}.");
            var leadModel = _leadService.GetById(id);
            var output = _autoMapper.Map<LeadResponse>(leadModel);
            _logger.LogInformation($"Лид с id = {id} успешно получен.");
            return Ok(output);
        }

        //api/Leads/password
        [HttpPut("password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation("Change lead password. Roles: All")]
        public ActionResult ChangePassword([FromBody] LeadChangePasswordRequest changePasswordRequest)
        {
            var id = this.GetLeadId();
            _logger.LogInformation($"Получен запрос на изменение пароля лида с id = {id}.");
            _leadService.ChangePassword(id, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            _logger.LogInformation($"Пароль лида с id = {id} успешно изменен.");
            return Ok();
        }

    }
}
