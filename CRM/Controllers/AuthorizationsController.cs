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
        private static Logger _logger;

        public AuthorizationsController(IAuthService authService)
        {
            _authService = authService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [SwaggerOperation("Authentication")]
        public ActionResult Login([FromBody] AuthRequest auth)
        {
            _logger.Info($"Получен запрос на аутентификаию лида с email = {auth.Email.Encryptor()}.");
            var token = _authService.GetToken(auth.Email, auth.Password);
            _logger.Info($"Аутентификаия лида с email = {auth.Email.Encryptor()} прошла успешно.");
            return Json(token);
        }
    }
}
