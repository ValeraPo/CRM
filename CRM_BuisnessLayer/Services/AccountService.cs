﻿using AutoMapper;
using CRM.BusinessLayer.Exceptions;
using CRM.BusinessLayer.Models;
using CRM.BusinessLayer.Services.Interfaces;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using System.Linq;

namespace CRM.BusinessLayer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _autoMapper;
        public AccountService(IMapper mapper, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _autoMapper = mapper;
        }

        public int AddAccount(AccountModel accountModel)
        {
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            if (mappedAccount.Lead.Role == Role.Admin)
                throw new AuthorizationException("Администратор не может создавать аккаунты");
            if (mappedAccount.Lead.Role == Role.Regular
                && mappedAccount.CurrencyType != CurrencyEnum.Currency.USD)
                throw new AuthorizationException("Лид с такой ролью не может создавать валютные счета кроме долларового");
            if (_accountRepository
                .GetByLead(mappedAccount.Lead.Id)
                .Select(a => a.CurrencyType)
                .ToList()
                .Contains(accountModel.CurrencyType))
                throw new DuplicationException("Счет с такой валютой уже существует");
            var id = _accountRepository.AddAccount(mappedAccount);
            return id;
        }

        public void UpdateAccount(AccountModel accountModel)
        {
            var mappedAccount = _autoMapper.Map<Account>(accountModel);
            var entity = _accountRepository.GetById(mappedAccount.Id);
            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            _accountRepository.UpdateAccountById(mappedAccount);
        }

        public void LockById(int id)
        {
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            _accountRepository.LockById(id);
        }

        public void UnlockById(int id)
        {
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            _accountRepository.UnlockById(id);
        }

        public List<AccountModel> GetByLead(int leadId)
        {
            var accounts = _accountRepository.GetByLead(leadId);
            if (accounts.Count == 0)
                throw new NotFoundException($"Лид с id = {leadId} не найден");
            return _autoMapper.Map<List<AccountModel>>(accounts);
        }

        public AccountModel GetById(int id)
        {
            var entity = _accountRepository.GetById(id);
            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            return _autoMapper.Map<AccountModel>(entity);
        }
    }
}
