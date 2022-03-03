using AutoMapper;
using CRM_APILayer.Models;
using CRM_BuisnessLayer.Models;
using CRM_BuisnessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM_APILayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LeadsController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly IMapper _autoMapper;

        public LeadsController(ILeadService leadService, IMapper autoMapper)
        {
            _leadService = leadService;
            _autoMapper = autoMapper;
        }

        //api/Users
        [HttpPost]
        [ProducesResponseType(typeof(UserOutputModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<LeadOutputModel> AddLead([FromBody] LeadInsertInputModel leadInsertInputModel)
        {
            var leadModel = _autoMapper.Map<LeadModel>(leadInsertInputModel);
            _leadService.AddLead(leadModel);
            return StatusCode(StatusCodes.Status201Created, leadModel);
        }
    }
}
