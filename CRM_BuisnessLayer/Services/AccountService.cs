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

        public async Task<int> AddAccount(int role, AccountModel accountModel)
        {
            _logger.LogInformation("Zapros na dobavlenie accounta.");
            await CheckDuplicationAccount(accountModel.Lead.Id, accountModel.CurrencyType);
            if (role == (int)Role.Regular && accountModel.CurrencyType != Currency.USD)
            {
                _logger.LogError("Oshibka dobavlenia accounta. Lead c takoi rol'yu ne mozhet sozdavat' valutnye cheta krome dollarovogo.");
                throw new AuthorizationException("Лид с такой ролью не может создавать валютные счета кроме долларового");
            }
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            var id = await _accountRepository.AddAccount(mappedAccount);
            return id;
        }

        public async Task UpdateAccount(int leadId, AccountModel accountModel)
        {
            _logger.LogInformation($"Zapros na obnovlenie accounta id = {accountModel.Id}.");
            var entity = await _accountRepository.GetById(accountModel.Id);

            ExceptionsHelper.ThrowIfEntityNotFound(accountModel.Id, entity);
            ExceptionsHelper.ThrowIfLeadDontHaveAccesToAccount(entity.Lead.Id, leadId);
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            await _accountRepository.UpdateAccountById(mappedAccount);
        }

        public async Task LockById(int id)
        {
            _logger.LogInformation($"Zapros na blokirovku accounta id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            if (entity.CurrencyType == Currency.RUB)
            {
                _logger.LogError("Oshibka blokirovki accounta. Rublevai account nel'zya udalit'.");
                throw new BadRequestException("Рублевый аккаунт нельзя заблокировать");
            }
            await _accountRepository.LockById(id);
        }

        public async Task UnlockById(int id)
        {
            _logger.LogInformation($"Zapros na razblokirovku accounta id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            await _accountRepository.UnlockById(id);
        }

        public async Task<List<AccountModel>> GetByLead(int leadId)
        {
            _logger.LogInformation($"Zapros na poluchenie vseh accountov.");
            var entity = await _leadRepository.GetById(leadId);
            ExceptionsHelper.ThrowIfEntityNotFound(leadId, entity);
            var accounts = await _accountRepository.GetByLead(leadId);
            return _autoMapper.Map<List<AccountModel>>(accounts);
        }

        public async Task<AccountModel> GetById(int id)
        {
            _logger.LogInformation($"Zapros na poluchenie accounta id = {id}.");
            var entity = await _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);

            return _autoMapper.Map<AccountModel>(entity);
        }

        public async Task<AccountModel> GetById(int id, int leadId)
        {
            var accountModel = await GetById(id);
            if (accountModel.Lead.Id != leadId)
            {
                _logger.LogError($"Oshibka zaprosa na poluchenie accounta id = {id}. Net dostupa k chuzhomu accountu.");
                throw new AuthorizationException("Нет доступа к чужому аккаунту.");
            }
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
            var balance = await _transactionService.GetBalance(accountIds);
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
                _logger.LogError("Oshibka dobavlenia accounta. Chet s takoi valutoi uze suchestvuet.");
                throw new DuplicationException("Счет с такой валютой уже существует");
            }
        }
    }
}
