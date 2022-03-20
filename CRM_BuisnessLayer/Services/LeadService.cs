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
            _logger.LogInformation("Запрос на добавление лида.");
            ExceptionsHelper.ThrowIfEmailRepeat((await _leadRepository.GetAll()).Select(e => e.Email).ToList(), leadModel.Email);
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            mappedLead.Password = PasswordHash.HashPassword(mappedLead.Password);
            var id = await _leadRepository.AddLead(mappedLead);
            mappedLead.Id = id;
            _accountRepository.AddAccount(new Account
            {
                Name = "MyAccount",
                CurrencyType = Currency.RUB,
                Lead = mappedLead
            });
            return id;
        }

        public async void UpdateLead(int id, LeadModel leadModel)
        {
            _logger.LogInformation($"Запрос на обновление лида id = {id}.");
            var entity = _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            _leadRepository.UpdateLeadById(mappedLead);
        }

        public async void ChangeRoleLead(int id, int role)
        {
            _logger.LogInformation($"Запрос на обновление роли лида id = {id}.");
            if (role != 2 || role != 3)
            {
                _logger.LogError($"Ошибка изменения роли. Роль можно изменить только на Vip или Regular.");
                throw new IncorrectRoleException("Роль можно изменить только на Vip или Regular");
            }
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            entity.Role = (Role)role;
            _leadRepository.ChangeRoleLead(entity);
        }

        public async void DeleteById(int id)
        {
            _logger.LogInformation($"Запрос на удаление лида id = {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            if (entity.IsBanned)
            {
                _logger.LogError($"Лид с ID {entity.Id} уже забанен");
                throw new BannedException($"Лид с ID {entity.Id} уже забанен");
            }

            _leadRepository.DeleteById(id);
        }

        public async void RestoreById(int id)
        {
            _logger.LogInformation($"Запрос на восстановление лида id = {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            if (!entity.IsBanned)
            {
                _logger.LogError($"Лид с ID {entity.Id} не забанен");
                throw new BannedException($"Лид с ID {entity.Id} не забанен");
            }

            _leadRepository.RestoreById(id);
        }

        public async Task<List<LeadModel>> GetAll()
        {
            _logger.LogInformation($"Запрос на получение всех лидов.");
            var leads = await _leadRepository.GetAll();
            return _autoMapper.Map<List<LeadModel>>(leads);
        }

        public async Task<LeadModel> GetById(int id)
        {
            _logger.LogInformation($"Запрос на получение аккаунта id = {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            return _autoMapper.Map<LeadModel>(entity);
        }

        public async void ChangePassword(int id, string oldPassword, string newPassword)
        {
            _logger.LogInformation($"Запрос на изменение пароля лида id = {id}.");
            var entity = await _leadRepository.GetById(id);

            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            ExceptionsHelper.ThrowIfPasswordIsIncorrected(oldPassword, entity.Password);

            string hashPassword = PasswordHash.HashPassword(newPassword);
            _leadRepository.ChangePassword(entity.Id, hashPassword);
        }

    }
}
