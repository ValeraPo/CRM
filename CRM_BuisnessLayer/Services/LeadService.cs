﻿using AutoMapper;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Extensions;
using CRM.DataLayer.Repositories.Interfaces;
using Google.Authenticator;
using Marvelous.Contracts.Enums;
using Marvelous.Contracts.ExchangeModels;
using Marvelous.Contracts.RequestModels;
using Marvelous.Contracts.ResponseModels;
using Microsoft.Extensions.Logging;

namespace CRM.BusinessLayer.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _autoMapper;
        private readonly ILogger<LeadService> _logger;
        private readonly IRequestHelper _requestHelper;


        public LeadService(IMapper autoMapper, 
            ILeadRepository leadRepository, 
            IAccountRepository accountRepository, 
            ILogger<LeadService> logger,
            IRequestHelper requestHelper)
        {
            _leadRepository = leadRepository;
            _autoMapper = autoMapper;
            _accountRepository = accountRepository;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        public async Task<(int, int)> AddLead(LeadModel leadModel)
        {
            _logger.LogInformation("Received a request to create a new lead.");
            var lead = await _leadRepository.GetByEmail(leadModel.Email);
            if (lead != null)
            {
                _logger.LogError($"Try to singup. Email {leadModel.Email.Encryptor()} is already exists.");
                throw new DuplicationException($"Try to singup. Email {leadModel.Email.Encryptor()} is already exists.");
            }
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            mappedLead.Password = await _requestHelper.HashPassword(leadModel.Password);
            var id = await _leadRepository.AddLead(mappedLead);
            mappedLead.Id = id;
            var accountId = await _accountRepository.AddAccount(new Account
            {
                Name = "MyAccount",
                CurrencyType = Currency.RUB,
                Lead = mappedLead
            });

            return (id, accountId);
        }

        public async Task UpdateLead(int id, LeadModel leadModel)
        {
            _logger.LogInformation($"Received a request to update lead with ID = {id}.");
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            await _leadRepository.UpdateLeadById(mappedLead);
        }

        public async Task ChangeRoleLead(int id, Role role)
        {
            _logger.LogInformation($"Received a request to update the role of the lead with ID = {id}.");
            if (role == Role.Admin)
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

        public async Task<List<LeadAuthExchangeModel>> GetAllToAuth()
        {
            _logger.LogInformation($"Received a request to receive all leads for Auth.");
            var leads = await _leadRepository.GetAllToAuth();
            return leads;
        }

        public async Task<LeadModel> GetById(int id, IdentityResponseModel leadIdentity)
        {
            _logger.LogInformation($"Received to get an lead with an ID {id}.");
            if ((Role)Enum.Parse(typeof(Role), leadIdentity.Role) != Role.Admin)
                ExceptionsHelper.ThrowIfLeadDontHaveAcces(id, (int)leadIdentity.Id);
            return await GetById(id);
        }

        public async Task<LeadModel> GetById(int id)
        {
            var entity = await _leadRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            return _autoMapper.Map<LeadModel>(entity);
        }

        public async Task ChangePassword(int id, string oldPassword, string newPassword)
        {
            _logger.LogInformation($"Received a request to change the password of a lead with an ID = {id}.");
            var entity = await _leadRepository.GetById(id);

            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            //Пробуем зарегистрироваться со старым паролем. Если пароль неверный, будет ошибка
            await _requestHelper.GetToken(new AuthRequestModel { Email = entity.Email, Password = oldPassword });

            string hashPassword = await _requestHelper.HashPassword(newPassword);
            await _leadRepository.ChangePassword(entity.Id, hashPassword);
        }

        public async Task ChangeRoleListLead(LeadShortExchangeModel[] models)
        {
            _logger.LogInformation($"Received a request to change the role of a leads with.");
            await _leadRepository.ChangeRoleListLead(models.ToList());
        }

        public async Task<Data2FAModel> GetData2FA(LeadModel lead)
        {
            TwoFactorAuthenticator tfA = new TwoFactorAuthenticator();
            var setupCode = tfA.GenerateSetupCode(Convert.ToString(lead.Id), Convert.ToString(lead.Id), lead.Password, false, 3);
            Data2FAModel data2FA = new Data2FAModel { LeadId = setupCode.Account, EncodedKey = setupCode.ManualEntryKey };
            
            return data2FA;
        }

    }
}
