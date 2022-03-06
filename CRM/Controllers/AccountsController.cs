using AutoMapper;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _autoMapper;

        public AccountsController(IAccountService accountService, IMapper autoMapper)
        {
            _accountService = accountService;
            _autoMapper = autoMapper;
        }

        //api/Accounts
        [HttpPost]
        [Description("Create account")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public ActionResult<int> AddAccount([FromBody] AccountInsertRequest accountInsertRequest)
        {
            var accountModel = _autoMapper.Map<AccountModel>(accountInsertRequest);
            var id = _accountService.AddAccount(accountModel);
            return StatusCode(StatusCodes.Status201Created, id);
        }

        //api/Accounts/42
        [HttpPut("{id}")]
        [Description("Update account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateAccount(int id, [FromBody] AccountUpdateRequest accountUpdateRequest)
        {
            var accountModel = _autoMapper.Map<AccountModel>(accountUpdateRequest);
            accountModel.Id = id;
            _accountService.UpdateAccount(accountModel);
            return Ok($"Account with id = {id} was updated");
        }

        //api/Accounts/42
        [HttpDelete("{id}")]
        [Description("Lock account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult LockById(int id)
        {
            _accountService.LockById(id);
            return Ok($"Account with id = {id} was locked");
        }

        //api/Accounts/42
        [HttpPatch("{id}")]
        [Description("UnlockAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UnlockById(int id)
        {
            _accountService.UnlockById(id);
            return Ok($"Account with id = {id} was unlocked");
        }

        //api/Accounts/lead/42
        [HttpGet("lead/{id}")]
        [Description("Get accounts by lead")]
        [ProducesResponseType(typeof(List<AccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<AccountResponse>> GetByLead(int id)
        {
            var accountModels = _accountService.GetByLead(id);
            var outputs = _autoMapper.Map<List<AccountResponse>>(accountModels);
            return Ok(outputs);
        }

        //api/Accounts/42
        [HttpGet("{id}")]
        [Description("Get account by id")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AccountResponse> GetById(int id)
        {
            var accountModel = _accountService.GetById(id);
            var output = _autoMapper.Map<AccountResponse>(accountModel);
            return Ok(output);
        }
    }
}
