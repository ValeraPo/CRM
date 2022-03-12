using AutoMapper;
using CRM.APILayer.Attribites;
using CRM.APILayer.Extensions;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.ComponentModel;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/leads")]

    public class LeadsController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly IMapper _autoMapper;
        private static Logger _logger;

        public LeadsController(ILeadService leadService, IMapper autoMapper)
        {
            _leadService = leadService;
            _autoMapper = autoMapper;
            _logger = LogManager.GetCurrentClassLogger();
        }

        //api/Leads
        [HttpPost]
        [Description("Create lead")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public ActionResult<int> AddLead([FromBody] LeadInsertRequest leadInsertRequest)
        {
            _logger.Info($"Получен запрос на создание лида.");
            var leadModel = _autoMapper.Map<LeadModel>(leadInsertRequest);
            var id = _leadService.AddLead(leadModel);
            _logger.Info($"Лид с id = {id} успешно создан.");
            return StatusCode(StatusCodes.Status201Created, id);
        }

        //api/Leads/42
        [HttpPut("{id}")]
        [Description("Update lead")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateLead(int id, [FromBody] LeadUpdateRequest leadUpdateRequest)
        {
            _logger.Info($"Получен запрос на создание лида с id = {id}.");
            var leadModel = _autoMapper.Map<LeadModel>(leadUpdateRequest);
            leadModel.Id = id;
            _leadService.UpdateLead(id, leadModel);
            _logger.Info($"Лид с id = {id} успешно обновлен.");
            return Ok($"Lead with id = {id} was updated");
        }

        //api/Leads/42
        [HttpDelete("{id}")]
        [Description("Delete lead")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteById(int id)
        {
            _logger.Info($"Получен запрос на удаление лида с id = {id}.");
            _leadService.DeleteById(id);
            _logger.Info($"Лид с id = {id} успешно удален.");
            return Ok($"Lead with id = {id} was deleted");
        }

        //api/Leads/42
        [HttpPatch("{id}")]
        [Description("Restore lead")]
        [AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult RestoreById(int id)
        {
            _logger.Info($"Получен запрос на восстановление лида с id = {id}.");
            _leadService.RestoreById(id);
            _logger.Info($"Лид с id = {id} успешно восстановлен.");
            return Ok($"Lead with id = {id} was restored");
        }

        //api/Leads/
        [HttpGet()]
        [Description("Get all leads")]
        //[AuthorizeEnum(Role.Admin)]
        [ProducesResponseType(typeof(List<LeadResponse>), StatusCodes.Status200OK)]
        public ActionResult<List<LeadResponse>> GetAll()
        {
            _logger.Info($"Получен запрос на получение всех лидов.");
            var leadModels = _leadService.GetAll();
            var outputs = _autoMapper.Map<List<LeadResponse>>(leadModels);
            _logger.Info($"Все лиды успешно получены.");
            return Ok(outputs);
        }

        //api/Leads/42
        [HttpGet("{id}")]
        [Description("Get lead by id")]
        [Authorize]
        [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<LeadResponse> GetById(int id)
        {
            _logger.Info($"Получен запрос на получение лида с id = {id}.");
            var leadModel = _leadService.GetById(id);
            var output = _autoMapper.Map<LeadResponse>(leadModel);
            _logger.Info($"Лид с id = {id} успешно получен.");
            return Ok(output);
        }

        //api/Leads/password
        [HttpPut("password")]
        [Description("Change lead password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult ChangePassword([FromBody] LeadChangePasswordRequest changePasswordRequest)
        {
            var id = this.GetLeadId();
            _logger.Info($"Получен запрос на изменение пароля лида с id = {id}.");
            _leadService.ChangePassword(id, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            _logger.Info($"Пароль лида с id = {id} успешно изменен.");
            return Ok();
        }

    }
}
