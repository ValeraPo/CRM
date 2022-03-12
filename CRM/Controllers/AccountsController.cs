using AutoMapper;
using CRM.APILayer.Attribites;
using CRM.APILayer.Extensions;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.ComponentModel;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/accounts")]

    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _autoMapper;
        private static Logger _logger;

        public AccountsController(IAccountService accountService, IMapper autoMapper)
        {
            _accountService = accountService;
            _autoMapper = autoMapper;
            _logger = LogManager.GetCurrentClassLogger();
        }

        //api/accounts
        [HttpPost]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [Description("Create account")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public ActionResult<int> AddAccount([FromBody] AccountInsertRequest accountInsertRequest)
        {
            var leadId = this.GetLeadId();
            _logger.Info($"Получен запрос на добавление аккаунта лидом с id = {leadId}.");
            var accountModel = _autoMapper.Map<AccountModel>(accountInsertRequest);
            accountModel.Lead.Id = leadId;
            var id = _accountService.AddAccount(this.GetLeadRole(), accountModel);
            _logger.Info($"Аккаунт с id = {id} успешно добавлен.");
            return StatusCode(StatusCodes.Status201Created, id);
        }

        //api/accounts/42
        [HttpPut("{id}")]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [Description("Update account")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateAccount(int id, [FromBody] AccountUpdateRequest accountUpdateRequest)
        {
            _logger.Info($"Получен запрос на обновление аккаунта id = {id} лидом с id = {this.GetLeadId()}.");
            var accountModel = _autoMapper.Map<AccountModel>(accountUpdateRequest);
            _accountService.UpdateAccount(id, accountModel);
            _logger.Info($"Аккаунт с id = {id} успешно обновлен.");
            return Ok($"Account with id = {id} was updated");
        }

        //api/accounts/42
        [HttpDelete("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [Description("Lock account")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult LockById(int id)
        {
            _logger.Info($"Получен запрос на блокировку аккаунта id = {id} лидом с id = {this.GetLeadId()}.");
            _accountService.LockById(id);
            _logger.Info($"Аккаунт с id = {id} успешно заблокирован.");
            return Ok($"Account with id = {id} was locked");
        }

        //api/accounts/42
        [HttpPatch("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [Description("UnlockAccount")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UnlockById(int id)
        {
            _logger.Info($"Получен запрос на разблокировку аккаунта id = {id} лидом с id = {this.GetLeadId()}.");
            _accountService.UnlockById(id);
            _logger.Info($"Аккаунт с id = {id} успешно разблокирован.");
            return Ok($"Account with id = {id} was unlocked");
        }

        //api/accounts
        [HttpGet()]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [Description("Get accounts by lead")]
        [ProducesResponseType(typeof(List<AccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<AccountResponse>> GetByLead()
        {
            var id = this.GetLeadId();
            _logger.Info($"Получен запрос на получение всех аккаунтов лидом с id = {id}.");
            var accountModels = _accountService.GetByLead(id);
            var outputs = _autoMapper.Map<List<AccountResponse>>(accountModels);
            _logger.Info($"Все аккаунты лида с id = {id} успешно получены.");
            return Ok(outputs);
        }

        //api/accounts/42
        [HttpGet("{id}")]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [Description("Get account by id")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<AccountResponse> GetById(int id)
        {
            _logger.Info($"Получен запрос на получение аккаунта с id = {id} лидом с id = {id}.");
            var leadId = this.GetLeadId();
            var accountModel = _accountService.GetById(id, leadId);
            var output = _autoMapper.Map<AccountResponse>(accountModel);
            _logger.Info($"Аккаунт с id = {id} успешно получен.");
            return Ok(output);
        }


    }
}
