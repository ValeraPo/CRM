using AutoMapper;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using System.Linq;

namespace CRM.BusinessLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _autoMapper;


        public AccountService(IMapper mapper, IAccountRepository accountRepository, ILeadRepository leadRepository)
        {
            _accountRepository = accountRepository;
            _leadRepository = leadRepository;
            _autoMapper = mapper;
        }

        public int AddVipAccount(AccountModel accountModel)
        {
            CheckDuplicationAccount(accountModel.Lead.Id, accountModel.CurrencyType);
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            var id = _accountRepository.AddAccount(mappedAccount);
            return id;
        }

        public int AddRegularAccount(AccountModel accountModel)
        {
            CheckDuplicationAccount(accountModel.Lead.Id, accountModel.CurrencyType);
            if (accountModel.CurrencyType != MarvelousContracts.Currency.USD)
                throw new AuthorizationException("Лид с такой ролью не может создавать валютные счета кроме долларового");
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            var id = _accountRepository.AddAccount(mappedAccount);
            return id;
        }

        public void UpdateAccount(int id, AccountModel accountModel)
        {
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            _accountRepository.UpdateAccountById(mappedAccount);
        }

        public void LockById(int id)
        {
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            _accountRepository.LockById(id);
        }

        public void UnlockById(int id)
        {
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            _accountRepository.UnlockById(id);
        }

        public List<AccountModel> GetByLead(int leadId)
        {
            var entity = _leadRepository.GetById(leadId);
            ExceptionsHelper.ThrowIfEntityNotFound(leadId, entity);
            var accounts = _accountRepository.GetByLead(leadId);
            return _autoMapper.Map<List<AccountModel>>(accounts);
        }

        public AccountModel GetById(int id, int leadId)
        {
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(id, entity);
            if (entity.Lead.Id != leadId)
                throw new AuthorizationException("Нет доступа к чужому аккаунту.");
            return _autoMapper.Map<AccountModel>(entity);
        }

        private void CheckDuplicationAccount(int leadId, MarvelousContracts.Currency currency)
        {
            var accounts = _accountRepository.GetByLead(leadId);
            var c = accounts.Select(x => x.CurrencyType).ToList();

            if (_accountRepository
                .GetByLead(leadId)
                .Select(a => a.CurrencyType)
                .ToList()
                .Contains(currency))
                throw new DuplicationException("Счет с такой валютой уже существует");
        }
    }
}
