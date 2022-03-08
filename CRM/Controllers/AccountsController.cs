using AutoMapper;
using CRM.APILayer.Attribites;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/accounts")]

    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _autoMapper;

        public AccountsController(IAccountService accountService, IMapper autoMapper)
        {
            _accountService = accountService;
            _autoMapper = autoMapper;
        }

        //api/accounts
        [HttpPost]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [Description("Create account")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public ActionResult<int> AddAccount([FromBody] AccountInsertRequest accountInsertRequest)
        {
            var accountModel = _autoMapper.Map<AccountModel>(accountInsertRequest);
            int id;
            if (GetLeadRole() == 2)
                id = _accountService.AddVipAccount(accountModel);
            else
                id = _accountService.AddRegularAccount(accountModel);
            return StatusCode(StatusCodes.Status201Created, id);
        }

        //api/accounts/42
        [HttpPut("{id}")]
        [AuthorizeEnum(Role.Vip, Role.Regular)]
        [Description("Update account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateAccount(int id, [FromBody] AccountUpdateRequest accountUpdateRequest)
        {
            var accountModel = _autoMapper.Map<AccountModel>(accountUpdateRequest);
            _accountService.UpdateAccount(id, accountModel);
            return Ok($"Account with id = {id} was updated");
        }

        //api/accounts/42
        [HttpDelete("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [Description("Lock account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult LockById(int id)
        {
            _accountService.LockById(id);
            return Ok($"Account with id = {id} was locked");
        }

        //api/accounts/42
        [HttpPatch("{id}")]
        [AuthorizeEnum(Role.Admin)]
        [Description("UnlockAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UnlockById(int id)
        {
            _accountService.UnlockById(id);
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
            var id = GetLeadId();
            var accountModels = _accountService.GetByLead(id);
            var outputs = _autoMapper.Map<List<AccountResponse>>(accountModels);
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
            var leadId = GetLeadId();
            var accountModel = _accountService.GetById(id, leadId);
            var output = _autoMapper.Map<AccountResponse>(accountModel);
            return Ok(output);
        }

        private int GetLeadRole()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            List<Claim> claims = identity.Claims.ToList();
            var role = int.Parse(claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault());
            return role;
        }

        private int GetLeadId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            List<Claim> claims = identity.Claims.ToList();
            var idUser = int.Parse(claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault());
            return idUser;
        }
    }
}
