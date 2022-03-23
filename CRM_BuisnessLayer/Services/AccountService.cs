using AutoMapper;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts;
using Microsoft.Extensions.Logging;
using NLog;

namespace CRM.BusinessLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _autoMapper;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IMapper mapper, IAccountRepository accountRepository, ILeadRepository leadRepository, ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _leadRepository = leadRepository;
            _autoMapper = mapper;
            _logger = logger;
        }

        public async Task<int> AddAccount(int role, AccountModel accountModel)
        {
            _logger.LogInformation("Запрос на добавление аккаунта.");
            await CheckDuplicationAccount(accountModel.Lead.Id, accountModel.CurrencyType);
            if (role == (int)Role.Regular && accountModel.CurrencyType != Currency.USD)
            {
                _logger.LogError("Ошибка добавления аккаунта. Лид с такой ролью не может создавать валютные счета кроме долларового.");
                throw new AuthorizationException("Лид с такой ролью не может создавать валютные счета кроме долларового");
            }
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            var id = await _accountRepository.AddAccount(mappedAccount);
            return id;
        }

        public async Task UpdateAccount(int leadId, AccountModel accountModel)
        {
            _logger.LogInformation($"Запрос на обновление аккаунта id = {accountModel.Id}.");
            var entity = await _accountRepository.GetById(accountModel.Id);

            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            ExceptionsHelper.ThrowIfEntityNotFound(accountModel.Id, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            await _accountRepository.UpdateAccountById(mappedAccount);
        }

        public async Task LockById(int id)
        {
            _logger.LogInformation($"Запрос на блокировку аккаунта id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            await _accountRepository.LockById(id);
        }

        public async Task UnlockById(int id)
        {
            _logger.LogInformation($"Запрос на разблокировку аккаунта id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            await _accountRepository.UnlockById(id);
        }

        public async Task<List<AccountModel>> GetByLead(int leadId)
        {
            _logger.LogInformation($"Запрос на получение всех аккаунтов.");
            var entity = await _leadRepository.GetById(leadId);
            ExceptionsHelper.ThrowIfEntityNotFound(leadId, entity);
            var accounts = await _accountRepository.GetByLead(leadId);
            return _autoMapper.Map<List<AccountModel>>(accounts);
        }

        public async Task<AccountModel> GetById(int id, int leadId)
        {
            _logger.LogInformation($"Запрос на получение аккаунта id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            if (entity.Lead.Id != leadId)
            {
                _logger.LogError($"Ошибка запроса на получение аккаунта id = {id}. Нет доступа к чужому аккаунту.");
                throw new AuthorizationException("Нет доступа к чужому аккаунту.");
            }
            return _autoMapper.Map<AccountModel>(entity);
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
                _logger.LogError("Ошибка добавления аккаунта. Счет с такой валютой уже существует.");
                throw new DuplicationException("Счет с такой валютой уже существует");
            }
        }
    }
}
