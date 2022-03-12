﻿using AutoMapper;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Security;
using CRM.BusinessLayer.Services.Interfaces;
using NLog;

namespace CRM.BusinessLayer.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _autoMapper;
        private static Logger _logger;

        public LeadService(IMapper autoMapper, ILeadRepository leadRepository, IAccountRepository accountRepository)
        {
            _leadRepository = leadRepository;
            _autoMapper = autoMapper;
            _accountRepository = accountRepository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public int AddLead(LeadModel leadModel)
        {
            _logger.Info("Запрос на добавление лида.");
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            mappedLead.Password = PasswordHash.HashPassword(mappedLead.Password);
            var id = _leadRepository.AddLead(mappedLead);
            _accountRepository.AddAccount(new Account
            {
                Name = "MyAccount",
                CurrencyType = MarvelousContracts.Currency.RUB,
                Lead = mappedLead
            });
            return id;
        }

        public void UpdateLead(int id, LeadModel leadModel)
        {
            _logger.Info($"Запрос на обновление лида id = {id}.");
            var entity = _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            _leadRepository.UpdateLeadById(mappedLead);
        }

        public void DeleteById(int id)
        {
            _logger.Info($"Запрос на удаление лида id = {id}.");
            var entity = _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            _leadRepository.DeleteById(id);
        }

        public void RestoreById(int id)
        {
            _logger.Info($"Запрос на восстановление лида id = {id}.");
            var entity = _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            _leadRepository.RestoreById(id);
        }

        public List<LeadModel> GetAll()
        {
            _logger.Info($"Запрос на получение всех лидов.");
            var leads = _leadRepository.GetAll();
            return _autoMapper.Map<List<LeadModel>>(leads);
        }

        public LeadModel GetById(int id)
        {
            _logger.Info($"Запрос на получение аккаунта id = {id}.");
            var entity = _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            return _autoMapper.Map<LeadModel>(entity);
        }

        public void ChangePassword(int id, string oldPassword, string newPassword)
        {
            _logger.Info($"Запрос на изменение пароля лида id = {id}.");
            var entity = _leadRepository.GetById(id);
            
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            ExceptionsHelper.ThrowIfPasswordIsIncorrected(oldPassword, entity.Password);

            string hashPassword = PasswordHash.HashPassword(newPassword);
            _leadRepository.ChangePassword(entity.Id, hashPassword);
        }

        
    }
}
