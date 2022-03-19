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

        public int AddAccount(int role, AccountModel accountModel)
        {
            _logger.LogInformation("Запрос на добавление аккаунта.");
            CheckDuplicationAccount(accountModel.Lead.Id, accountModel.CurrencyType);
            if (role == (int)Role.Regular && accountModel.CurrencyType != Currency.USD)
            {
                _logger.LogError("Ошибка добавления аккаунта. Лид с такой ролью не может создавать валютные счета кроме долларового.");
                throw new AuthorizationException("Лид с такой ролью не может создавать валютные счета кроме долларового");
            }
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            var id = _accountRepository.AddAccount(mappedAccount);
            return id;
        }

        public void UpdateAccount(int leadId, AccountModel accountModel)
        {
            _logger.LogInformation($"Запрос на обновление аккаунта id = {accountModel.Id}.");
            var entity = _accountRepository.GetById(accountModel.Id);
            ExceptionsHelper.ThrowIfEntityNotFound(accountModel.Id, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            _accountRepository.UpdateAccountById(mappedAccount);
        }

        public void LockById(int id)
        {
            _logger.LogInformation($"Запрос на блокировку аккаунта id = {id}.");
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            _accountRepository.LockById(id);
        }

        public void UnlockById(int id)
        {
            _logger.LogInformation($"Запрос на разблокировку аккаунта id = {id}.");
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            _accountRepository.UnlockById(id);
        }

        public List<AccountModel> GetByLead(int leadId)
        {
            _logger.LogInformation($"Запрос на получение всех аккаунтов.");
            var entity = _leadRepository.GetById(leadId);
            ExceptionsHelper.ThrowIfEntityNotFound(leadId, entity);
            var accounts = _accountRepository.GetByLead(leadId);
            return _autoMapper.Map<List<AccountModel>>(accounts);
        }

        public AccountModel GetById(int id, int leadId)
        {
            _logger.LogInformation($"Запрос на получение аккаунта id = {id}.");
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            if (entity.Lead.Id != leadId)
            {
                _logger.LogError($"Ошибка запроса на получение аккаунта id = {id}. Нет доступа к чужому аккаунту.");
                throw new AuthorizationException("Нет доступа к чужому аккаунту.");
            }
            return _autoMapper.Map<AccountModel>(entity);
        }

        private void CheckDuplicationAccount(int leadId, Currency currency)
        {
            var accounts = _accountRepository.GetByLead(leadId);
            var c = accounts.Select(x => x.CurrencyType).ToList();

            if (_accountRepository
                .GetByLead(leadId)
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
