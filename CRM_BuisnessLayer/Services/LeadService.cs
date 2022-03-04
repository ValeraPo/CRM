using AutoMapper;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using CRM_BuisnessLayer.Models;
using CRM_BuisnessLayer.Security;
using CRM_BuisnessLayer.Services.Interfaces;

namespace CRM_BuisnessLayer.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _autoMapper;

        public LeadService(IMapper autoMapper, ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
            _autoMapper = autoMapper;
        }

        public int AddLead(LeadModel leadModel)
        {
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            mappedLead.Password = PasswordHash.HashPassword(mappedLead.Password);
            var id = _leadRepository.AddLead(mappedLead);
            return id;
        }

        public void UpdateLead(LeadModel leadModel)
        {
            var mappedUser = _autoMapper.Map<Lead>(leadModel);
            _leadRepository.UpdateLeadById(mappedUser);
        }
    }
}
