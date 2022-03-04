using AutoMapper;
using CRM_BuisnessLayer.Services.Interfaces;
using CRM_DataLayer.Repositories.Interfaces;

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
