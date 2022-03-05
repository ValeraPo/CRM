using AutoMapper;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.APILayer.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CRM.APILayer.Controllers
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
        public ActionResult<int> AddLead([FromBody] LeadInsertRequest leadInsertRequest)
        {
            var leadModel = _autoMapper.Map<LeadModel>(leadInsertRequest);
            var id = _leadService.AddLead(leadModel);
            return StatusCode(StatusCodes.Status201Created, id);
        }

        //api/Leads/42
        [HttpPut("{id}")]
        [Description("Update lead")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult UpdateLead(int id, [FromBody] LeadUpdateRequest leadUpdateRequest)
        {
            var leadModel = _autoMapper.Map<LeadModel>(leadUpdateRequest);
            leadModel.Id = id;
            _leadService.UpdateLead(leadModel);
            return Ok($"Lead with id = {id} was updated");
        }
    }
}
