using AutoMapper;
using CRM.APILayer.Models;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CRM.APILayer.Controllers
{
    [ApiController]
    [Route("api/leads")]

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
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateLead(int id, [FromBody] LeadUpdateRequest leadUpdateRequest)
        {
            var leadModel = _autoMapper.Map<LeadModel>(leadUpdateRequest);
            leadModel.Id = id;
            _leadService.UpdateLead(leadModel);
            return Ok($"Lead with id = {id} was updated");
        }

        //api/Leads/42
        [HttpDelete("{id}")]
        [Description("Delete lead")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteById(int id)
        {
            _leadService.DeleteById(id);
            return Ok($"Lead with id = {id} was deleted");
        }

        //api/Leads/42
        [HttpPatch("{id}")]
        [Description("Restore lead")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult RestoreById(int id)
        {
            _leadService.RestoreById(id);
            return Ok($"Lead with id = {id} was restored");
        }

        //api/Leads/
        [HttpGet()]
        [Description("Get all leads")]
        [ProducesResponseType(typeof(List<LeadResponse>), StatusCodes.Status200OK)]
        public ActionResult<List<LeadResponse>> GetAll()
        {
            var leadModels = _leadService.GetAll();
            var outputs = _autoMapper.Map<List<LeadResponse>>(leadModels);
            return Ok(outputs);
        }

        //api/Leads/42
        [HttpGet("{id}")]
        [Description("Get lead by id")]
        [ProducesResponseType(typeof(LeadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<LeadResponse> GetById(int id)
        {
            var leadModel = _leadService.GetById(id);
            var output = _autoMapper.Map<LeadResponse>(leadModel);
            return Ok(output);
        }



        [HttpPut("{id}/password")]
        [Description("Change lead password")]
        public ActionResult ChangePassword(int id, [FromBody] LeadChangePasswordRequest changePasswordRequest)
        {
            _leadService.ChangePassword(id, changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);
            return Ok();
        }

    }
}
