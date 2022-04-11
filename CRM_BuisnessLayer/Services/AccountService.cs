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
        private readonly ITransactionService _transactionService;

        public AccountService(IMapper mapper, 
            IAccountRepository accountRepository, 
            ILeadRepository leadRepository, 
            ILogger<AccountService> logger,
            ITransactionService transactionService)
        {
            _accountRepository = accountRepository;
            _leadRepository = leadRepository;
            _autoMapper = mapper;
            _logger = logger;
            _transactionService = transactionService;
        }

        public async Task<int> AddAccount(Role role, AccountModel accountModel)
        {
            _logger.LogInformation("Request was received to add an account.");
            await CheckDuplicationAccount(accountModel.Lead.Id, accountModel.CurrencyType);
            if (role == Role.Regular && accountModel.CurrencyType != Currency.USD)
            {
                _logger.LogError("Authorisation error. The lead role does not allow you to create accounts other than dollar.");
                throw new AuthorizationException("Authorization error. The lead role does not allow you to create accounts other than dollar.");
            }
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            var id = await _accountRepository.AddAccount(mappedAccount);
            return id;
        }

        public async Task UpdateAccount(int leadId, AccountModel accountModel)
        {
            _logger.LogInformation($"Request was received to update an account ID = {accountModel.Id}.");
            var entity = await _accountRepository.GetById(accountModel.Id);

            ExceptionsHelper.ThrowIfEntityNotFound(accountModel.Id, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            await _accountRepository.UpdateAccountById(mappedAccount);
        }

        public async Task LockById(int id)
        {
            _logger.LogInformation($"Request was received to lock an account ID =  {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            if (entity.CurrencyType == Currency.RUB)
            {
                _logger.LogError("Error: it is forbidden to block ruble accounts.");
                throw new BadRequestException("Error: it is forbidden to block ruble accounts.");
            }
            await _accountRepository.LockById(id);
        }

        public async Task UnlockById(int id)
        {
            _logger.LogInformation($"Request was received to unlock an account ID =  {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            await _accountRepository.UnlockById(id);
        }

        public async Task<List<AccountModel>> GetByLead(int leadId)
        {
            _logger.LogInformation($"Request to get all accounts.");
            var entity = await _leadRepository.GetById(leadId);
            ExceptionsHelper.ThrowIfEntityNotFound(leadId, entity);
            var accounts = await _accountRepository.GetByLead(leadId);
            var accountModels = _autoMapper.Map<List<AccountModel>>(accounts);
            foreach (var account in accountModels)
            {
                account.Balance = await _transactionService.GetBalance(new List<int> { account.Id}, account.CurrencyType);
            }
            return accountModels;
        }

        public async Task<AccountModel> GetById(int id)
        {
            _logger.LogInformation($"Request for an account with an ID = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            return _autoMapper.Map<AccountModel>(entity);
        }

        public async Task<AccountModel> GetById(int id, int leadId)
        {
            _logger.LogInformation($"Request for an account with an ID {id} lead with an ID {leadId}.");
            var accountModel = await GetById(id);
            if (accountModel.Lead.Id != leadId)
            {
                _logger.LogError($"Authorisation Error. No access to someone else's account.");
                throw new AuthorizationException("Authorisation Error. No access to someone else's account.");
            }
            accountModel.Balance = await _transactionService.GetBalance(new List<int> { accountModel.Id }, accountModel.CurrencyType);
            return accountModel;
        }

        public async Task<decimal> GetBalance(int leadId,  Currency currencyType)
        {
            var accounts = await GetByLead(leadId);
            if (!accounts.Select(a => a.CurrencyType).Contains(currencyType))
            {
                _logger.LogError("Balance receipt error. Currency type should be among accounts.");
                throw new BadRequestException("Currency type should be among accounts.");
            }
            var accountIds = accounts.Select(a => a.Id).ToList();
            var balance = await _transactionService.GetBalance(accountIds, currencyType);
            _logger.LogInformation("Balance was received.");
            return balance;
        }


        private async Task CheckDuplicationAccount(int leadId, Currency currency)
        {
            var accounts = await _accountRepository.GetByLead(leadId);
            var c = accounts.Select(x => x.CurrencyType).ToList();

            if ((await _accountRepository
                .GetByLead(leadId))
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
