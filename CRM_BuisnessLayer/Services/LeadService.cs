using AutoMapper;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Security;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Enums;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("Zapros na dobavlenie leada.");
            ExceptionsHelper.ThrowIfEmailRepeat((await _leadRepository.GetByEmail(leadModel.Email)), leadModel.Email);
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            mappedLead.Password = PasswordHash.HashPassword(mappedLead.Password);
            var id = await _leadRepository.AddLead(mappedLead);
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
            _logger.LogInformation($"Zapros na obnovlenie leada id = {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            await _leadRepository.UpdateLeadById(mappedLead);
        }

        public async Task ChangeRoleLead(int id, int role)
        {
            _logger.LogInformation($"Zapros na obnovlenie roli лиleadaда id = {id}.");
            if (role != 2 && role != 3)
            {
                _logger.LogError($"Oshibka izmenenia roli. Rol' mozhno izmenit' tol'ko na Vip ili Regular.");
                throw new IncorrectRoleException("Роль можно изменить только на Vip или Regular");
            }
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            entity.Role = (Role)role;
            await _leadRepository.ChangeRoleLead(entity);
        }

        public async Task DeleteById(int id)
        {
            _logger.LogInformation($"Zapros na udalenie leada id = {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            if (entity.IsBanned)
            {
                _logger.LogError($"Lead c ID {entity.Id} uze zabanen");
                throw new BannedException($"Лид с ID {entity.Id} уже забанен");
            }

            await _leadRepository.DeleteById(id);
        }

        public async Task RestoreById(int id)
        {
            _logger.LogInformation($"Zapros na vosstanovlenie leada id = {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            if (!entity.IsBanned)
            {
                _logger.LogError($"Lead c ID {entity.Id} ne zabanen");
                throw new BannedException($"Лид с ID {entity.Id} не забанен");
            }

            await _leadRepository.RestoreById(id);
        }

        public async Task<List<LeadModel>> GetAll()
        {
            _logger.LogInformation($"Zapros na poluchenie vseh leadov.");
            var leads = await _leadRepository.GetAll();
            return _autoMapper.Map<List<LeadModel>>(leads);
        }

        public async Task<LeadModel> GetById(int id)
        {
            _logger.LogInformation($"Zapros na poluchenie accounta id = {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            return _autoMapper.Map<LeadModel>(entity);
        }

        public async Task ChangePassword(int id, string oldPassword, string newPassword)
        {
            _logger.LogInformation($"Zapros na izmenenie parolya leada id = {id}.");
            var entity = await _leadRepository.GetById(id);

            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            ExceptionsHelper.ThrowIfPasswordIsIncorrected(oldPassword, entity.Password);

            string hashPassword = PasswordHash.HashPassword(newPassword);
            await _leadRepository.ChangePassword(entity.Id, hashPassword);
        }

    }
}
