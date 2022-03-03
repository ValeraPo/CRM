using AutoMapper;
using CRM_BuisnessLayer.Models;
using CRM_BuisnessLayer.Security;
using CRM_BuisnessLayer.Services.Interfaces;
using CRM_DataLayer;
using CRM_DataLayer.Repositories.Interfaces;

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

        public void AddLead(LeadModel leadModel)
        {
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            mappedLead.Password = PasswordHash.HashPassword(mappedLead.Password);
            _leadRepository.AddLead(mappedLead);
        }

        public void UpdateLead(LeadModel leadModel)
        {
            var mappedUser = _autoMapper.Map<Lead>(leadModel);
            _leadRepository.UpdateLeadById(mappedUser);
        }
    }
}
