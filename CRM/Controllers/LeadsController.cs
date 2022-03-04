using AutoMapper;
using CRM_APILayer.Models;
using CRM_BuisnessLayer.Models;
using CRM_BuisnessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

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

        //api/Leads
        [HttpPost]
        [Description("Create lead")]
        [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status201Created)]
        public ActionResult<LeadResponse> AddLead([FromBody] LeadInsertRequest leadInsertInputModel)
        {
            var leadModel = _autoMapper.Map<LeadModel>(leadInsertInputModel);
            _leadService.AddLead(leadModel);
            return StatusCode(StatusCodes.Status201Created, leadModel);
        }

        //api/Leads
        [HttpPut("{id}")]
        [Description("Update lead")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult UpdateUser(int id, [FromBody] LeadUpdateRequest leadUpdateInputModel)
        {
            var leadModel = _autoMapper.Map<LeadModel>(leadUpdateInputModel);
            _leadService.UpdateLead(leadModel);
            return Ok($"Lead with id = {id} was updated");
        }
    }
}
