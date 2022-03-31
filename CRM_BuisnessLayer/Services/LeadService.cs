using AutoMapper;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Security;
using CRM.BusinessLayer.Services.Interfaces;
using NLog;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts;
using CRM.BusinessLayer.Exceptions;
using Microsoft.Extensions.Logging;
using Marvelous.Contracts.Enums;

namespace CRM.BusinessLayer.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _autoMapper;
        private readonly ILogger<LeadService> _logger;

        public LeadService(IMapper autoMapper, ILeadRepository leadRepository, IAccountRepository accountRepository, ILogger<LeadService> logger)
        {
            _leadRepository = leadRepository;
            _autoMapper = autoMapper;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<int> AddLead(LeadModel leadModel)
        {
            _logger.LogInformation("Received a request to create a new lead.");
            ExceptionsHelper.ThrowIfEmailRepeat((await _leadRepository.GetByEmail(leadModel.Email)), leadModel.Email);
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            mappedLead.Password = PasswordHash.HashPassword(mappedLead.Password);
            var id =  await _leadRepository.AddLead(mappedLead);
            mappedLead.Id = id;
            await _accountRepository.AddAccount(new Account
            {
                Name = "MyAccount",
                CurrencyType = Currency.RUB,
                Lead = mappedLead
            });
            return id;
        }

        public async Task UpdateLead(int id, LeadModel leadModel)
        {
            _logger.LogInformation($"Received a request to update lead with ID = {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            await _leadRepository.UpdateLeadById(mappedLead);
        }

        public async Task ChangeRoleLead(int id, int role)
        {
            _logger.LogInformation($"Received a request to update the role of the lead with ID = {id}.");
            if (role != 2 && role != 3)
            {
                _logger.LogError($"Authorisation error. The role can be changed to Regular or VIP.");
                throw new IncorrectRoleException("Authorisation error. The role can be changed to Regular or VIP.");
            }
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            entity.Role = (Role)role;
            await _leadRepository.ChangeRoleLead(entity);
        }

        public async Task DeleteById(int id)
        {
            _logger.LogInformation($"Received a request to delete lead with ID =  {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            if (entity.IsBanned)
            {
                _logger.LogError($"Lead witd ID {entity.Id} is already banned.");
                throw new BannedException($"Lead witd ID {entity.Id} is already banned.");
            }

            await _leadRepository.DeleteById(id);
        }

        public async Task RestoreById(int id)
        {
            _logger.LogInformation($"Received a request to restore lead with ID =  {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            if (!entity.IsBanned)
            {
                _logger.LogError($"Lead with ID {entity.Id} is not banned.");
                throw new BannedException($"Lead with ID {entity.Id} is not banned.");
            }

            await _leadRepository.RestoreById(id);
        }

        public async Task<List<LeadModel>> GetAll()
        {
            _logger.LogInformation($"Received a request to receive all leads.");
            var leads = await _leadRepository.GetAll();
            return _autoMapper.Map<List<LeadModel>>(leads);
        }

        public async Task<LeadModel> GetById(int id)
        {
            _logger.LogInformation($"Received to get an lead with an ID {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            return _autoMapper.Map<LeadModel>(entity);
        }

        public async Task ChangePassword(int id, string oldPassword, string newPassword)
        {
            _logger.LogInformation($"Received a request to change the password of a lead with an ID = {id}.");
            var entity = await _leadRepository.GetById(id);

            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            ExceptionsHelper.ThrowIfPasswordIsIncorrected(oldPassword, entity.Password);

            string hashPassword = PasswordHash.HashPassword(newPassword);
            await _leadRepository.ChangePassword(entity.Id, hashPassword);
        }

    }
}
