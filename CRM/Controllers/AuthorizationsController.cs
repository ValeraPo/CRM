using CRM.APILayer.Models;
using CRM.BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthorizationsController : Controller
    {
        private readonly IAuthService _authService;
        public AuthorizationsController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Description("Authentication")]
        public ActionResult Login([FromBody] AuthRequest auth)
        {
            var token = _authService.GetToken(auth.Email, auth.Password);
            return Json(token);
        }
    }
}
