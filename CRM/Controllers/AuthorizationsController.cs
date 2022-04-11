using CRM.APILayer.Models;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Services;
using CRM.DataLayer.Extensions;
using Marvelous.Contracts.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthorizationsController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthorizationsController> _logger;
        private readonly IRequestHelper _requestHelper;

        public AuthorizationsController(IAuthService authService, 
            ILogger<AuthorizationsController> logger,
            IRequestHelper requestHelper)
        {
            _authService = authService;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [SwaggerOperation("Authentication")]
        public async Task<ActionResult<string>> Login([FromBody] AuthRequestModel auth)
        {
            _logger.LogInformation($"Lead with email {auth.Email.Encryptor()} tries to log in.");
            var token = await _requestHelper.GetToken(auth);
            //var token = await _authService.GetToken(auth.Email, auth.Password);
            _logger.LogInformation($"Lead with email {auth.Email.Encryptor()} successfully logged in.");
            return Ok(token.Content);
        }
    }
}
