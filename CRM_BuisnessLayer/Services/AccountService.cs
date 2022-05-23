using AutoMapper;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.Enums;
using Microsoft.Extensions.Logging;

namespace CRM.BusinessLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _autoMapper;
        private readonly ILogger<AccountService> _logger;
        private readonly IRequestHelper _requestHelper;

        public AccountService(IMapper mapper, 
            IAccountRepository accountRepository, 
            ILeadRepository leadRepository, 
            ILogger<AccountService> logger,
            IRequestHelper requestHelper)
        {
            _accountRepository = accountRepository;
            _leadRepository = leadRepository;
            _autoMapper = mapper;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        //Adding account
        public async Task<int> AddAccount(Role role, AccountModel accountModel)
        {
            _logger.LogInformation("Request was received to add an account.");
            //Check if account with this currency exists
            await CheckDuplicationAccount(accountModel.Lead.Id, accountModel.CurrencyType);
            //Check if role is regular and currency is not USD
            if (role == Role.Regular && accountModel.CurrencyType != Currency.USD)
            {
                _logger.LogError("Authorisation error. The lead role does not allow you to create accounts other than dollar.");
                throw new AuthorizationException("Authorization error. The lead role does not allow you to create accounts other than dollar.");
            }
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            var id = await _accountRepository.AddAccount(mappedAccount);
            return id;
        }

        //Updating account
        public async Task UpdateAccount(int leadId, AccountModel accountModel)
        {
            _logger.LogInformation($"Request was received to update an account ID = {accountModel.Id}.");
            //Get account from data base
            var entity = await _accountRepository.GetById(accountModel.Id);
            //Check if account is null
            ExceptionsHelper.ThrowIfEntityNotFound(accountModel.Id, entity);
            //Check if lead's id from token and lead's id from account model aren't same
            ExceptionsHelper.ThrowIfLeadDontHaveAcces(entity.Lead.Id, leadId); 
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            await _accountRepository.UpdateAccountById(mappedAccount);
        }

        //Locking account
        public async Task LockById(int id)
        {
            _logger.LogInformation($"Request was received to lock an account ID =  {id}.");
            //Get account from data base
            var entity = await _accountRepository.GetById(id);
            //Check if account is null
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            // It is not possible to block account with curency RUB
            if (entity.CurrencyType == Currency.RUB)
            {
                _logger.LogError("Error: it is forbidden to block ruble accounts.");
                throw new BadRequestException("Error: it is forbidden to block ruble accounts.");
            }
            await _accountRepository.LockById(id);
        }

        //Unlocking account
        public async Task UnlockById(int id)
        {
            _logger.LogInformation($"Request was received to unlock an account ID =  {id}.");
            //Get account from data base
            var entity = await _accountRepository.GetById(id);
            //Check if account is null
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            await _accountRepository.UnlockById(id);
        }

        //Getting all lead's accounts
        public async Task<List<AccountModel>> GetByLead(int leadId)
        {
            _logger.LogInformation($"Request to get all accounts.");
            //Get account from data base
            var entity = await _leadRepository.GetById(leadId);
            //Check if account is null
            ExceptionsHelper.ThrowIfEntityNotFound(leadId, entity);
            var accounts = await _accountRepository.GetByLead(leadId); //List<Account>
            var accountModels = _autoMapper.Map<List<AccountModel>>(accounts);
            
            return accountModels;
        }

        // Getting an account
        public async Task<AccountModel> GetById(int id)
        {
            _logger.LogInformation($"Request for an account with an ID = {id}.");
            //Get account from data base
            var entity = await _accountRepository.GetById(id);
            //Check if account is null
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            return _autoMapper.Map<AccountModel>(entity);
        }

        // Getting an account
        public async Task<AccountModel> GetById(int id, int leadId)
        {
            _logger.LogInformation($"Request for an account with an ID {id} lead with an ID {leadId}.");
            //Get account from data base with check if account is not found
            var accountModel = await GetById(id);
            //Check if lead's id from token and lead's id from account model aren't same
            if (accountModel.Lead.Id != leadId)
            {
                _logger.LogError($"Authorisation Error. No access to someone else's account.");
                throw new AuthorizationException("Authorisation Error. No access to someone else's account.");
            }
            // Getting balance from transaction service by RestSharp
            accountModel.Balance = await _requestHelper.GetBalance(accountModel.Id, accountModel.CurrencyType);
            return accountModel;
        }

        // Get balance by all lead's accounts 
        public async Task<decimal> GetBalance(int leadId,  Currency currencyType)
        {
            //Get all lead's accounts from data base
            var accounts = await GetByLead(leadId);
            // Check if currency there isn't among accounts
            if (!accounts.Select(a => a.CurrencyType).Contains(currencyType))
            {
                _logger.LogError("Balance receipt error. Currency type should be among accounts.");
                throw new BadRequestException("Currency type should be among accounts.");
            }
            var accountIds = accounts.Select(a => a.Id).ToList();
            // Getting balance from transaction service by RestSharp
            var balance = await _requestHelper.GetBalance(accountIds, currencyType);
            _logger.LogInformation("Balance was received.");
            return balance;
        }

        //Check if account with this currency exists
        private async Task CheckDuplicationAccount(int leadId, Currency currency)
        {
            //Get all lead's accounts from data base
            var accounts = await _accountRepository.GetByLead(leadId);
            // Forming List<CurrencyType>
            var c = accounts.Select(x => x.CurrencyType).ToList();

            if (accounts
                .Select(a => a.CurrencyType)
                .ToList()
                .Contains(currency))
            {
                _logger.LogError("Error: an account with this currency already exists.");
                throw new DuplicationException("Error: an account with this currency already exists.");
            }
        }
    }
}
