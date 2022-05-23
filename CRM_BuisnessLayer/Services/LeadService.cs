using AutoMapper;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Extensions;
using CRM.DataLayer.Repositories.Interfaces;
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

        // Adding lead
        // return (leadId, accountId)
        public async Task<(int, int)> AddLead(LeadModel leadModel)
        {
            _logger.LogInformation("Received a request to create a new lead.");
            // Check in email exists
            var lead = await _leadRepository.GetByEmail(leadModel.Email);
            if (lead != null)
            {
                _logger.LogError($"Try to singup. Email {leadModel.Email.Encryptor()} is already exists.");
                throw new DuplicationException($"Try to singup. Email {leadModel.Email.Encryptor()} is already exists.");
            }
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            // Hashing password
            mappedLead.Password = await _requestHelper.HashPassword(leadModel.Password);
            var id = await _leadRepository.AddLead(mappedLead);
            mappedLead.Id = id;
            // Created account witn currency RUB
            var accountId = await _accountRepository.AddAccount(new Account
            {
                Name = "MyAccount",
                CurrencyType = Currency.RUB,
                Lead = mappedLead
            });
            
            return (id, accountId);
        }

        //Updated lead
        public async Task UpdateLead(int id, LeadModel leadModel)
        {
            _logger.LogInformation($"Received a request to update lead with ID = {id}.");
            //Get lead from data base
            var entity = await _leadRepository.GetById(id);
            //Check if lead is null
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            var mappedLead = _autoMapper.Map<Lead>(leadModel);
            await _leadRepository.UpdateLeadById(mappedLead);
        }

        //Changing lead's role
        public async Task ChangeRoleLead(int id, Role role)
        {
            _logger.LogInformation($"Received a request to update the role of the lead with ID = {id}.");
            // It is forbidden to change admin's role
            if (role == Role.Admin)
            {
                _logger.LogError($"Authorisation error. The role can be changed to Regular or VIP.");
                throw new IncorrectRoleException("Authorisation error. The role can be changed to Regular or VIP.");
            }
            //Get lead from data base
            var entity = await _leadRepository.GetById(id);
            //Check if lead is null
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            entity.Role = (Role)role;
            await _leadRepository.ChangeRoleLead(entity);
        }

        //Deleting lead
        public async Task DeleteById(int id)
        {
            _logger.LogInformation($"Received a request to delete lead with ID =  {id}.");
            var entity = await _leadRepository.GetById(id);
            //Check if lead is null
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            // Check in lead is already deleted
            if (entity.IsBanned)
            {
                _logger.LogError($"Lead witd ID {entity.Id} is already banned.");
                throw new BannedException($"Lead witd ID {entity.Id} is already banned.");
            }

            await _leadRepository.DeleteById(id);
        }

        //Restoring lead
        public async Task RestoreById(int id)
        {
            _logger.LogInformation($"Received a request to restore lead with ID =  {id}.");
            //Get lead from data base
            var entity = await _leadRepository.GetById(id);
            //Check if lead is null
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            //Check if lead is not deleted
            if (!entity.IsBanned)
            {
                _logger.LogError($"Lead with ID {entity.Id} is not banned.");
                throw new BannedException($"Lead with ID {entity.Id} is not banned.");
            }

            await _leadRepository.RestoreById(id);
        }

        // Getting all leads
        public async Task<List<LeadModel>> GetAll()
        {
            _logger.LogInformation($"Received a request to receive all leads.");
            //Get all leads from data base
            var leads = await _leadRepository.GetAll();
            return _autoMapper.Map<List<LeadModel>>(leads);
        }

        //Getting all real leads 
        public async Task<List<LeadAuthExchangeModel>> GetAllToAuth()
        {
            _logger.LogInformation($"Received a request to receive all leads for Auth.");
            //Get all real leads from data base
            var leads = await _leadRepository.GetAllToAuth();
            return leads;
        }

        // Getting a lead by id
        public async Task<LeadModel> GetById(int id, IdentityResponseModel leadIdentity)
        {
            _logger.LogInformation($"Received to get an lead with an ID {id}.");
            //Check if role is not admin
            if ((Role)Enum.Parse(typeof(Role), leadIdentity.Role) != Role.Admin)
                // Check if id and id from token aren't same
                ExceptionsHelper.ThrowIfLeadDontHaveAcces(id, (int)leadIdentity.Id);
            //Get lead from data base and check if lead is null
            return await GetById(id);
        }

        //Getting a lead by id
        public async Task<LeadModel> GetById(int id)
        {
            //Get lead from data base
            var entity = await _leadRepository.GetById(id);
            //Check if lead is null
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            return _autoMapper.Map<LeadModel>(entity);
        }

        //Changing password
        public async Task ChangePassword(int id, string oldPassword, string newPassword)
        {
            _logger.LogInformation($"Received a request to change the password of a lead with an ID = {id}.");
            //Get lead from data base
            var entity = await _leadRepository.GetById(id);

            //Check if lead is null
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            //Try authorization with old password. If password is wrong, error will be
            await _requestHelper.GetToken(new AuthRequestModel { Email = entity.Email, Password = oldPassword });
            // Hashing new password
            string hashPassword = await _requestHelper.HashPassword(newPassword);
            await _leadRepository.ChangePassword(entity.Id, hashPassword);
        }
        
        // Changing roles for array leads
        public async Task ChangeRoleListLead(LeadShortExchangeModel[] models)
        {
            _logger.LogInformation($"Received a request to change the role of a leads with.");
            await _leadRepository.ChangeRoleListLead(models.ToList());
        }

    }
}
