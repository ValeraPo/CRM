using CRM.APILayer.Models;
using CRM.BusinessLayer.Services;
using CRM.DataLayer.Extensions;
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


        public AuthorizationsController(IAuthService authService, ILogger<AuthorizationsController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [SwaggerOperation("Authentication")]
        public async Task<ActionResult> Login([FromBody] AuthRequest auth)
        {
            _logger.LogInformation($"Poluchen zapros na authentikaciu leada c email = {auth.Email.Encryptor()}.");
            var token = await _authService.GetToken(auth.Email, auth.Password);
            _logger.LogInformation($"Authentikacia leada c email = {auth.Email.Encryptor()} proshhla uspeshno.");
            return new JsonResult(token);
        }
    }
}
