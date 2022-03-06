using AutoMapper;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Security;
using CRM.BusinessLayer.Services.Interfaces;

namespace CRM.BusinessLayer.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _autoMapper;

        public LeadService(IMapper autoMapper, ILeadRepository leadRepository, IAccountRepository accountRepository)
        {
            _leadRepository = leadRepository;
            _autoMapper = autoMapper;
            _accountRepository = accountRepository;

        }

        public int AddLead(LeadModel leadModel)
        {
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            mappedLead.Password = PasswordHash.HashPassword(mappedLead.Password);
            var id = _leadRepository.AddLead(mappedLead);
            _accountRepository.AddAccount(new Account
            {
                Name = "MyAccount",
                CurrencyType = CurrencyEnum.Currency.RUB,
                Lead = mappedLead
            });
            return id;
        }

        public void UpdateLead(LeadModel leadModel)
        {
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            var entity = _leadRepository.GetById(mappedLead.Id);
            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            _leadRepository.UpdateLeadById(mappedLead);
        }

        public void DeleteById(int id)
        {
            var entity = _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            _leadRepository.DeleteById(id);
        }

        public void RestoreById(int id)
        {
            var entity = _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            _leadRepository.RestoreById(id);
        }

        public List<LeadModel> GetAll()
        {
            var leads = _leadRepository.GetAll();
            return _autoMapper.Map<List<LeadModel>>(leads);
        }

        public LeadModel GetById(int id)
        {
            var entity = _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            return _autoMapper.Map<LeadModel>(entity);
        }

        public void ChangePassword(int id, string newPassword)
        {
            var entity = _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            string hashPassword = PasswordHash.HashPassword(newPassword);
            _leadRepository.ChangePassword(entity, hashPassword);
        }

        public bool ValidatePassword(string password)
        {

        }
    }
}
