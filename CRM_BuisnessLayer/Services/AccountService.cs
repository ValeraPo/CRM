using AutoMapper;
using CRM.DataLayer.Repositories.Interfaces;
using CRM_BuisnessLayer.Services.Interfaces;

namespace CRM_BuisnessLayer.Services
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
    }
}
