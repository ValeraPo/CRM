using CRM.APILayer.Models;
using CRM.BusinessLayer.Services;
using CRM.DataLayer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthorizationsController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountsController> _logger;


        public AuthorizationsController(IAuthService authService, ILogger<AccountsController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [SwaggerOperation("Authentication")]
        public async Task<ActionResult> Login([FromBody] AuthRequest auth)
        {
            _logger.LogInformation($"Lead with email {auth.Email.Encryptor()} tries to log in.");
            var token = await _authService.GetToken(auth.Email, auth.Password);
            _logger.LogInformation($"Lead with email {auth.Email.Encryptor()} successfully logged in.");
            return new JsonResult(token);
        }
    }
}
